namespace Core.Cache.Abstractions;

internal interface IPrimaryHealthStateWriter
{
    void MarkUnavailable();
}