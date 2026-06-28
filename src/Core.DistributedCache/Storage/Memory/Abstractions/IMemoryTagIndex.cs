
namespace Core.DistributedCache.Storage.Memory.Abstractions;

internal interface IMemoryTagIndex
{
    void AddTags(string key, string[] tags);
    IEnumerable<string> GetKeysByTag(string tag);
    void RemoveKey(string key);
    void RemoveByTag(string tag, Action<string> onKeyRemoved);
}
