namespace CoreSystem.Samples.Core.Services;

public interface IMyService
{
    Task<string> GetDataAsync(string id);
}