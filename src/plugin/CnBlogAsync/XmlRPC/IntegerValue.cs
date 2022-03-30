using SXL=System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public class IntegerValue : Value
    {
        public readonly int Integer;

        public IntegerValue(int i)
        {
            this.Integer = i;
        }

        public static string TypeString
        {
            get { return "int"; }
        }

        protected override void AddToTypeEl(SXL.XElement parent)
        {
            parent.Value = this.Integer.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }

        public static IntegerValue XmlToValue(SXL.XElement parent)
        {
            var bv = new IntegerValue(int.Parse(parent.Value));
            return bv;
        }

        public static string AlternateTypeString
        {
            get { return "i4"; }            
        }

        public static implicit operator IntegerValue(int v)
        {
            return new IntegerValue(v);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as IntegerValue;
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.Integer== p.Integer);
        }

        public override int GetHashCode()
        {
            return this.Integer.GetHashCode();
        }

        protected override string GetTypeString()
        {
            return IntegerValue.TypeString;
        }

    }
}