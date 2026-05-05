namespace ShortVideoCutter.Interfaces;

public interface IStatistic : IService
{
    public Task DelayBeforeStart(int mileseconds);
    public void CreateStatistic(string saveDirectory);
}
