namespace CnBlogAsync.XmlRPC
{
    [Serializable]
    public class XmlRPCException : Exception
    {
        public XmlRPCException() { }
        public XmlRPCException(string message) : base(message) { }
        public XmlRPCException(string message, Exception inner) : base(message, inner) { }
        protected XmlRPCException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }

        public Fault Fault;
    }
}