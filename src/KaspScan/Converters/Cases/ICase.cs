namespace KaspScan.Converters.Cases
{
    public interface ICase
    {
        bool IsMatched(object value);
        object Value { get; }
    }
}