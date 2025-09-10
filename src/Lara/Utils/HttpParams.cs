namespace Lara;

/// A fluent builder for HTTP parameters that automatically filters null values
public class HttpParams<TValue>
{
    private readonly Dictionary<string, TValue> _values = new();

    /// Sets a parameter value, automatically filtering null values.
    public HttpParams<TValue> Set(string key, TValue? value)
    {
        if (value != null)
        {
            _values[key] = value;
        }
        return this;
    }

    /// Builds the final parameter dictionary, returning null if empty.
    public Dictionary<string, TValue>? Build()
    {
        return _values.Count == 0 ? null : _values;
    }
}