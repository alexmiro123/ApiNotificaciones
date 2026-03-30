namespace ApiCartera.Utils;

public class ServiceResponse<T>
{
    public T? Data { get; set; }
    public bool Success { get; set; }
    public String Message { get; set; } = String.Empty;
    public int Status { get; set; }
    

}

public class NotFoundException: Exception
{
    public NotFoundException(string message): base(message) { }
}

public class DatabaseException : Exception
{
    public DatabaseException(string message, Exception innerException): base(message, innerException) { }
}

