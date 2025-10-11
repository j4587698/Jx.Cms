namespace Jx.Cms.Common.Exceptions;

public class DbException : Exception
{
    public DbException()
    {
    }

    public DbException(string info) : base(info)
    {
    }
}