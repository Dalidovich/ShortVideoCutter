using ShortVideoCutter.Models;

namespace ShortVideoCutter.Interfaces;

public interface IConverterVideoProcessor : IService
{
    public void MergeMoments(Dictionary<int, List<MergeData>> mergeDict, string saveDirectory);
    public bool _CheckMergeDataOnCorrectSequence(List<MergeData> mergeDatas);
    public bool TryProcessedInvalidMoment(Moment moment);
    public void Processed(List<Season> seasons, string saveDirectory);
    public void ProcessedSimpleMoments(List<Season> seasons, string saveDirectory);
    public void ProcessedInvalidMoments(List<Season> seasons, string saveDirectory);
    public void ProcessedPartsMoments(List<Season> seasons, string saveDirectory);
}
