using System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class Fault
{
    public int FaultCode { get; set; }
    public string FaultString { get; set; }
    public string RawData { get; set; }

    public static Fault ParseXml(XElement fault_el)
    {
        var value_el = fault_el.GetElement("value");
        var fault_value = (Struct)Value.ParseXml(value_el);

        var fault_code = -1;
        var fault_code_val = fault_value.Get("faultCode");
        if (fault_code_val != null)
        {
            if (fault_code_val is StringValue)
            {
                var s = (StringValue)fault_code_val;
                fault_code = int.Parse(s.String);
            }
            else if (fault_code_val is IntegerValue)
            {
                var i = (IntegerValue)fault_code_val;
                fault_code = i.Integer;
            }
            else
            {
                var msg = string.Format("Fault Code value is not int or string {0}", value_el);
                throw new MetaWeblogException(msg);
            }
        }

        var fault_string = fault_value.Get<StringValue>("faultString").String;

        var f = new Fault
        {
            FaultCode = fault_code,
            FaultString = fault_string,
            RawData = fault_el.Document.ToString()
        };
        return f;
    }
}