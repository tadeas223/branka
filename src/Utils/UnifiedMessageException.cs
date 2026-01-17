namespace P2PBank.Utils;

class UnifiedMessageException: Exception
{
    public UnifiedMessageException()
        : base("An error occurred.")
    {
    }

    public UnifiedMessageException(string message)
        : base(message)
    {
    }

    public UnifiedMessageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
} 
