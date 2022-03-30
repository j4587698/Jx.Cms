namespace CnBlogAsync.XmlRPC
{
    public class Fault
    {
        public int FaultCode { get; set; }
        public string FaultString { get; set; }
        public string RawData { get; set; }

        public static Fault ParseXml(System.Xml.Linq.XElement fault_el)
        {
            var value_el = fault_el.GetElement("value");
            var fault_value = (Struct)XmlRPC.Value.ParseXml(value_el);

            int fault_code = -1;
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
                    string msg = string.Format("Fault Code value is not int or string {0}", value_el.ToString());
                    throw new MetaWeblogException(msg);
                }
            }

            string fault_string = fault_value.Get<StringValue>("faultString").String;

            var f = new Fault
            {
                FaultCode = fault_code,
                FaultString = fault_string,
                RawData = fault_el.Document.ToString()
            };
            return f;
        }
    }
}