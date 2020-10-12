using System;

namespace Jx.Cms.Common.Exceptions
{
    public class DbException: Exception
    {

        public DbException(): base()
        {
        }

        public DbException(string info):base(info)
        {
            
        }
    }
}