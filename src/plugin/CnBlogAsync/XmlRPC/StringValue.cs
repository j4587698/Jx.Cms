using SXL=System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public class StringValue : Value
    {
        public string String;

        public StringValue(string s)
        {
            this.String = s;
        }

        public static string TypeString
        {
            get { return "string"; }
        }

        protected override void AddToTypeEl(SXL.XElement parent)
        {
            parent.Value = this.String;
        }

        public static StringValue XmlToValue(SXL.XElement parent)
        {
            var bv = new StringValue(parent.Value);
            return bv;
        }

        public static implicit operator StringValue(string v)
        {
            return new StringValue( v);
        }

        private static StringValue ns;
        private static StringValue es;

        public static StringValue NullString
        {
            get
            {
                if (StringValue.ns == null)
                {
                    ns = new StringValue(null);
                }
                return ns;
            }
        }

        public static StringValue EmptyString
        {
            get
            {
                if (StringValue.es == null)
                {
                    es = new StringValue(string.Empty);
                }
                return es;
            }
        }

        protected override string GetTypeString()
        {
            return StringValue.TypeString;
        }
    }
}