using SXL=System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public abstract class Value
    {
        protected abstract void AddToTypeEl(SXL.XElement parent);
        protected abstract string GetTypeString();

        public static Value ParseXml(SXL.XElement value_el)
        {
            if (value_el.Name != "value")
            {
                string msg = string.Format("XML Element should have name \"value\" instead found \"{0}\"", value_el.Name);
                throw new XmlRPCException();
            }

            var input_value = value_el.Value;
            if (value_el.HasElements)
            {
                var type_el = value_el.Elements().First();

                string typename = type_el.Name.ToString();
                if (typename == Array.TypeString)
                {
                    return Array.XmlToValue(type_el);
                }
                else if (typename == Struct.TypeString)
                {
                    return Struct.XmlToValue(type_el);
                }
                else if (typename == StringValue.TypeString)
                {
                    return StringValue.XmlToValue(type_el);
                }
                else if (typename == DoubleValue.TypeString)
                {
                    return DoubleValue.XmlToValue(type_el);
                }
                else if (typename == Base64Data.TypeString)
                {
                    return Base64Data.XmlToValue(type_el);
                }
                else if (typename == DateTimeValue.TypeString)
                {
                    return DateTimeValue.XmlToValue(type_el);
                }
                else if (typename == IntegerValue.TypeString || typename == IntegerValue.AlternateTypeString)
                {
                    return IntegerValue.XmlToValue(type_el);
                }
                else if (typename == BooleanValue.TypeString )
                {
                    return BooleanValue.XmlToValue(type_el);
                }
                else
                {
                    string msg = string.Format("Unsupported type: {0}", typename);
                    throw new XmlRPCException(msg);
                }
            }
            else
            {
                // no <type> element provided. Treat the content as a string
                return new StringValue(input_value);
            }
        }

        public SXL.XElement AddXmlElement(SXL.XElement parent)
        {
            var value_el = new SXL.XElement("value");
            var type_el = new SXL.XElement(this.GetTypeString());
            value_el.Add(type_el);

            this.AddToTypeEl(type_el);

            parent.Add(value_el);

            return value_el;

        }
    }
}