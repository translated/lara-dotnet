namespace Lara.Sdk;

/// The base exception class for all Lara SDK errors.
public class LaraException : Exception
{
    /// Initializes a new instance of the <see cref="LaraException"/> class.
    protected LaraException(string message) : base(message)
    {
    }

    /// Initializes a new instance of the <see cref="LaraException"/> class with a specified error message and a reference to the inner exception.
    protected LaraException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// Exception thrown when the Lara API returns an error response
public class LaraApiException : LaraException
{
    /// Gets the HTTP status code returned by the API.
    public int StatusCode { get; }

    /// Gets the error type returned by the API.
    public string Type { get; }

    /// Initializes a new instance of the <see cref="LaraApiException"/> class.
    public LaraApiException(int statusCode, string type, string message) : base(message)
    {
        StatusCode = statusCode;
        Type = type ?? string.Empty;
    }
}

/// Exception thrown when an operation times out.
public class LaraTimeoutException : LaraException
{
    /// Initializes a new instance of the <see cref="LaraTimeoutException"/> class.
    public LaraTimeoutException() : base("The operation timed out.")
    {
    }

    /// Initializes a new instance of the <see cref="LaraTimeoutException"/> class with a specified error message.
    public LaraTimeoutException(string message) : base(message)
    {
    }

    /// Initializes a new instance of the <see cref="LaraTimeoutException"/> class with a specified error message and a reference to the inner exception.
    public LaraTimeoutException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// Exception thrown when there are S3 storage-related errors.
public class S3Exception : LaraException
{
    /// Initializes a new instance of the <see cref="S3Exception"/> class.
    public S3Exception(string message) : base(message)
    {
    }

    /// Initializes a new instance of the <see cref="S3Exception"/> class with a specified error message and a reference to the inner exception.
    public S3Exception(string message, Exception innerException) : base(message, innerException)
    {
    }
} 