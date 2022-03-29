namespace CnBlogAsync
{
    [System.Serializable]
    public class MetaWeblogException : System.Exception
    {
        public MetaWeblogException() { }
        public MetaWeblogException(string message) : base(message) { }
        public MetaWeblogException(string message, System.Exception inner) : base(message, inner) { }
        protected MetaWeblogException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}