using ShortVideoCutter.Extensions;
using ShortVideoCutter.Interfaces.ModuleInterfaces;
using ShortVideoCutter.Models;

namespace ShortVideoCutter.Modules;

public class ConverterVideoProcessor : IConverterVideoProcessor
{
    private readonly IModuleIO _moduleIO;
    private readonly IFFMpegModule _mpegModule;
    private readonly IMapper _mapper;

    public ConverterVideoProcessor(IModuleIO moduleIO, IFFMpegModule mpegModule, IMapper mapper)
    {
        _moduleIO = moduleIO;
        _mpegModule = mpegModule;
        _mapper = mapper;
    }

    public void MergeMoments(Dictionary<int, List<MergeData>> mergeDict, string saveDirectory)
    {
        foreach (var listOfMergeData in mergeDict)
        {
            var mergePartsOfMomentDirectory = 
                Path.Combine(saveDirectory, 
                $"SplitMomentWithId{listOfMergeData.Key}_{listOfMergeData.Value.FirstOrDefault()?.Season.EnName}_" +
                $"({listOfMergeData.Value.Count})");
            _moduleIO.InitDirectory(mergePartsOfMomentDirectory);

            // Check invalid separate moments
            if (!_CheckMergeDataOnCorrectSequence(listOfMergeData.Value))
            {
                _moduleIO.FileWriteAllText(
                    $"{Path.Combine(mergePartsOfMomentDirectory, $"WrongSeq({listOfMergeData.Value.Count}).txt")}",
                    $"Have parts: {string.Join(", ", listOfMergeData.Value.Select(x => x.Part))}\n" +
                    $"Have {listOfMergeData.Value.CountPartsConditionStatus(EMomentStatus.Invalid)} invalid parts");
                continue;
            }

            // If is just one moment in sequence
            if (listOfMergeData.Value.Count == 1 && listOfMergeData.Value.SingleOrDefault() is { } singleMergeMomentData)
            {
                _mpegModule.TrimedVideo(
                    singleMergeMomentData.Episode.GetSavePath(),
                    singleMergeMomentData.Moment.GetSavePath(),
                    singleMergeMomentData.Moment.Start, singleMergeMomentData.Moment.SecondsDuration);
                continue;
            }

            // Trim separate moment
            foreach (var mergeData in listOfMergeData.Value)
            {
                _mpegModule.TrimedVideo(
                    mergeData.Episode.GetSavePath(),
                    Path.Combine(mergePartsOfMomentDirectory, $"{mergeData.SaveName}.mp4"),
                    mergeData.Moment.Start, mergeData.Moment.SecondsDuration);
            }
            var finalPathToSaveMergedMoment = Path.Combine(mergePartsOfMomentDirectory, "Final");
            _moduleIO.InitDirectory(finalPathToSaveMergedMoment);
            _mpegModule.MergeMoments(
                listOfMergeData.Value
                    .OrderBy(x => x.Part)
                    .Select(x => Path.Combine(mergePartsOfMomentDirectory, $"{x.SaveName}.mp4"))
                    .ToArray(),
                finalPathToSaveMergedMoment);
        }
    }

    public bool _CheckMergeDataOnCorrectSequence(List<MergeData> mergeDatas)
    {
        var set = mergeDatas.Select(x => x.Part).ToHashSet();
        // List of moment in line sequence and not contain invalid moment
        return mergeDatas.CountPartsConditionStatus(EMomentStatus.Invalid) == 0
            && set.Count == mergeDatas.Count
            && set.Sum() == set.Count * (set.Count + 1) / 2;
    }

    public bool TryProcessedInvalidMoment(Moment moment)
    {
        if (moment.GetStatus() != EMomentStatus.Invalid)
        {
            return true;
        }
        var momentWillbeCreate = false;
        var invalidTxtCreated = _moduleIO.FileExists(moment.GetSavePath());
        var deleteInvalidTxt = false;
        var correctEpisodePath = moment.GetCorrectEpisodePathOrDefault();

        // Invalid txt already created
        if (invalidTxtCreated)
        {
            // if moment note has path - txt will be delete
            deleteInvalidTxt = correctEpisodePath != null;
            // else overwrite path 
            correctEpisodePath ??= _moduleIO.GetAllTextLines(moment.GetSavePath()).LastOrDefault();
        }
        if (correctEpisodePath == null)
        {
            _moduleIO.FileWriteAllText(moment.GetSavePath(), moment.Note);
            return false;
        }
        if (_moduleIO.FileExists(correctEpisodePath))
        {
            moment.OverwriteTimes(_mapper);
            _mpegModule.TrimedVideo(correctEpisodePath, moment.GetSavePath(true), moment.Start, moment.SecondsDuration);
            moment.RepairMoment();
            momentWillbeCreate = true;
        }
        if (deleteInvalidTxt)
        {
            _moduleIO.DeleteFile(moment.GetSavePath());
        }

        return momentWillbeCreate;
    }

    public void Processed(List<Season> seasons, string saveDirectory)
    {
        ProcessedSimpleMoments(seasons, saveDirectory);
        ProcessedInvalidMoments(seasons, saveDirectory);
        ProcessedPartsMoments(seasons, saveDirectory);
    }

    public void ProcessedSimpleMoments(List<Season> seasons, string saveDirectory)
    {
        var actualStatus = EMomentStatus.Simple;

        foreach (var season in seasons)
        {
            foreach (var episode in season.Episodes)
            {
                foreach (var moment in episode.Moments)
                {
                    if (moment.GetStatus() != actualStatus)
                    {
                        continue;
                    }
                    _mpegModule.TrimedVideo(episode.GetSavePath(), moment.GetSavePath(), moment.Start, moment.SecondsDuration);
                }
            }
        }
    }

    public void ProcessedInvalidMoments(List<Season> seasons, string saveDirectory)
    {
        var actualStatus = EMomentStatus.Invalid;

        foreach (var season in seasons)
        {
            foreach (var episode in season.Episodes)
            {
                foreach (var moment in episode.Moments)
                {
                    if (moment.GetStatus() != actualStatus)
                    {
                        continue;
                    }

                    TryProcessedInvalidMoment(moment);
                }
            }
        }
    }

    public void ProcessedPartsMoments(List<Season> seasons, string saveDirectory)
    {
        var forGlobalMergeMoments = new Dictionary<int, List<MergeData>>();

        foreach (var season in seasons)
        {
            var forMergeInSeasons = new Dictionary<int, List<MergeData>>();
            foreach (var episode in season.Episodes)
            {
                foreach (var moment in episode.Moments)
                {
                    var partData = moment.IsPartMoment();
                    if (!partData.isPart)
                    {
                        continue;
                    }
                    var data = new MergeData(moment, episode, season, partData.data.Part, moment.GetSaveName(season, episode));
                    if (partData.data.GlobalId.HasValue)
                    {
                        forGlobalMergeMoments.AddItemInListInDict(partData.data.GlobalId.Value, data);
                    }
                    else
                    {
                        forMergeInSeasons.AddItemInListInDict(partData.data.Id, data);
                    }
                }
            }
            MergeMoments(forMergeInSeasons, season.GetSeasonPath(saveDirectory));
        }

        MergeMoments(forGlobalMergeMoments, saveDirectory);
    }
}
