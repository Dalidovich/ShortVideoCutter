using ShortVideoCutter.Models;

namespace ShortVideoCutter.Extensions;

public static class ListExtensions
{
    public static int CountPartsConditionStatus(this List<MergeData> mergeDatas, MomentStatus status)
    {
        return mergeDatas.Count(x => x.moment.GetStatus() == status);
    }
}
