using SXL=System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public class BooleanValue : Value
    {
        public readonly bool Boolean;
        
        public BooleanValue (bool value)
        {
            this.Boolean = value;
        }

        public static string TypeString
        {
            get { return "boolean"; }
        }

        protected override void AddToTypeEl(SXL.XElement parent)
        {
            if (this.Boolean)
            {
                parent.Add("1");
            }
            else
            {
                parent.Add("0");
            }
        }

        public static BooleanValue XmlToValue(SXL.XElement type_el)
        {
            var i = int.Parse(type_el.Value);
            var b = (i != 0);
            var bv = new BooleanValue(b);
            return bv;
        }

        public static implicit operator BooleanValue (bool v)
        {
            return new BooleanValue(v);
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as BooleanValue;
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (this.Boolean == p.Boolean);
        }

        public override int GetHashCode()
        {
            return this.Boolean.GetHashCode();
        }

        protected override string GetTypeString()
        {
            return BooleanValue.TypeString;
        }

    }
}