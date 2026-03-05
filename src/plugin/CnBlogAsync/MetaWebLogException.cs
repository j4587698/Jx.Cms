namespace CnBlogAsync;

[Serializable]
public class MetaWeblogException : Exception
{
    public MetaWeblogException()
    {
    }

    public MetaWeblogException(string message) : base(message)
    {
    }

    public MetaWeblogException(string message, Exception inner) : base(message, inner)
    {
    }
}
