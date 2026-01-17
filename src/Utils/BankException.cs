namespace Utils;

class BankException: Exception
{
    // Default constructor
    public BankException()
        : base("An error occurred in the database operation.")
    {
    }

    // Constructor that takes a custom message
    public BankException(string message)
        : base(message)
    {
    }

    // Constructor that takes a custom message and inner exception
    public BankException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    // Constructor that takes an error code (optional) along with a message
    public BankException(string message, int errorCode)
        : base($"{message} Error Code: {errorCode}")
    {
    } 
} 
