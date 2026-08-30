namespace ShortVideoCutter.Interfaces.ModuleInterfaces;

public interface IStatistic : IService
{
    public Task DelayBeforeStart(int mileseconds);
    public void CreateStatistic(string saveDirectory);
}
