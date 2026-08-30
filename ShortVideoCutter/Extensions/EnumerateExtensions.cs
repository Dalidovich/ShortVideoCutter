using ShortVideoCutter.Models;

namespace ShortVideoCutter.Extensions;

public static class EnumerateExtensions
{
    public static int CountPartsConditionStatus(this List<MergeData> mergeDatas, EMomentStatus status)
    {
        return mergeDatas.Count(x => x.Moment.GetStatus() == status);
    }

    public static void AddItemInListInDict<TId, TItem>(this Dictionary<TId, List<TItem>> dict, TId id, TItem item)
    {
        if (dict.TryGetValue(id, out var list))
        {
            list.Add(item);
        }
        else
        {
            dict.Add(id, new List<TItem>()
            {
                item
            });
        }
    }
}
