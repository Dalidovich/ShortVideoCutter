using ShortVideoCutter.Extensions;
using ShortVideoCutter.Models;

namespace ShortVideoCutter.Modules;

public class ConverterVideo
{
    public void MergeMoments(Dictionary<int, List<MergeData>> mergeDict, string saveDirectory)
    {
        foreach (var listOfMergeData in mergeDict)
        {
            var mergePartsOfMomentDirectory = Path.Combine(saveDirectory, $"SplitMomentWithId{listOfMergeData.Key}({listOfMergeData.Value.Count}" +
                $")_{DateTimeOffset.Now.ToUnixTimeMilliseconds()}");
            StaticDI.ModuleIO.InitDirectory(mergePartsOfMomentDirectory);
            if (!_CheckMergeDataOnCorrectSequence(listOfMergeData.Value))
            {
                StaticDI.ModuleIO.FileWriteAllText(
                    $"{Path.Combine(mergePartsOfMomentDirectory, $"WrongSeq({listOfMergeData.Value.Count}).txt")}",
                    $"Have parts: {string.Join(", ", listOfMergeData.Value.Select(x => x.part))}\n" +
                    $"Have {listOfMergeData.Value.CountPartsConditionStatus(MomentStatus.Invalid)} invalid parts");
                continue;
            }

            if (listOfMergeData.Value.Count == 1 && listOfMergeData.Value.SingleOrDefault() is { } singleMergeMomentData)
            {
                StaticDI.FFMpegModule.TrimedVideo(
                    singleMergeMomentData.episode.GetSavePath(), 
                    singleMergeMomentData.moment.GetSavePath(), 
                    singleMergeMomentData.moment.Start, singleMergeMomentData.moment.SecondsDuration);
                continue;
            }

            foreach (var mergeData in listOfMergeData.Value)
            {
                StaticDI.FFMpegModule.TrimedVideo(
                    mergeData.episode.GetSavePath(), 
                    Path.Combine(mergePartsOfMomentDirectory, $"{mergeData.saveName}.mp4"),
                    mergeData.moment.Start, mergeData.moment.SecondsDuration);
            }
            var finalPathToSaveMergedMoment = Path.Combine(mergePartsOfMomentDirectory, "Final");
            StaticDI.ModuleIO.InitDirectory(finalPathToSaveMergedMoment);
            StaticDI.FFMpegModule.MergeMoments(
                listOfMergeData.Value
                    .OrderBy(x=>x.part)
                    .Select(x=> Path.Combine(mergePartsOfMomentDirectory, $"{x.saveName}.mp4"))
                    .ToArray(), 
                finalPathToSaveMergedMoment);
        }
    }

    private bool _CheckMergeDataOnCorrectSequence(List<MergeData> mergeDatas)
    {
        var set = mergeDatas.Select(x => x.part).ToHashSet();

        return mergeDatas.CountPartsConditionStatus(MomentStatus.Invalid) == 0
            && set.Count == mergeDatas.Count
            && set.Sum() == set.Count * (set.Count + 1) / 2;
    }

    public bool TryProcessedInvalidMoment(Moment moment)
    {
        if (moment.GetStatus() != MomentStatus.Invalid)
        {
            return true;
        }

        if (StaticDI.ModuleIO.FileExists(moment.GetSavePath()))
        {
            var correctEpisodePath = StaticDI.ModuleIO.GetAllTextLines(moment.GetSavePath()).LastOrDefault();
            if (correctEpisodePath != null && StaticDI.ModuleIO.FileExists(correctEpisodePath))
            {
                StaticDI.FFMpegModule.TrimedVideo(correctEpisodePath, moment.GetSavePath(true), moment.Start, moment.SecondsDuration);
                moment.RepairMoment();
                return true;
            }
            return false;
        }
        StaticDI.ModuleIO.FileWriteAllText(moment.GetSavePath(), moment.Note);
        return false;
    }

    public void Processed(List<Season> seasons, string saveDirectory)
    {
        ProcessedSimpleMoments(seasons, saveDirectory);
        ProcessedInvalidMoments(seasons, saveDirectory);
        ProcessedPartsMoments(seasons, saveDirectory);
    }

    private void ProcessedSimpleMoments(List<Season> seasons, string saveDirectory)
    {
        var actualStatus = MomentStatus.Simple;

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
                    StaticDI.FFMpegModule.TrimedVideo(episode.GetSavePath(), moment.GetSavePath(), moment.Start, moment.SecondsDuration);
                }
            }
        }
    }

    private void ProcessedInvalidMoments(List<Season> seasons, string saveDirectory)
    {
        var actualStatus = MomentStatus.Invalid;

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

    private void ProcessedPartsMoments(List<Season> seasons, string saveDirectory)
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
                    var data = new MergeData(moment, episode, partData.data.part, moment.GetSaveName(season, episode));
                    if (partData.data.globalId.HasValue)
                    {
                        AddedToDict(forGlobalMergeMoments, partData.data.globalId.Value, data);
                    }
                    else
                    {
                        AddedToDict(forMergeInSeasons, partData.data.id, data);
                    }
                }
            }
            MergeMoments(forMergeInSeasons, saveDirectory);
        }

        MergeMoments(forGlobalMergeMoments, saveDirectory);
    }

    private void AddedToDict(Dictionary<int, List<MergeData>> src, int id, MergeData data)
    {
        if (src.TryGetValue(id, out var list))
        {
            list.Add(data);
        }
        else
        {
            src.Add(id, new List<MergeData>()
            {
                data
            });
        }
    }
}
