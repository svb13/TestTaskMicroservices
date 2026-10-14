namespace Nemetscheck.Shared.ErrorHandling;

public sealed class ExceptionMappingOptions
{
    private readonly List<(Type ExceptionType, int Status, string Title)> _mappings = [];

    public ExceptionMappingOptions Map<TException>(int status, string title)
        where TException : Exception
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        _mappings.Add((typeof(TException), status, title));
        
        return this;
    }

    internal (int Status, string Title)? Find(Exception exception)
    {
        foreach (var (type, status, title) in _mappings)
        {
            if (type.IsInstanceOfType(exception))
                return (status, title);
        }

        return null;
    }
}
