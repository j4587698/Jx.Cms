using System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class MethodResponse
{
    public MethodResponse(string content)
    {
        Parameters = new ParameterList();

        var lo = new LoadOptions();

        var doc = XDocument.Parse(content, lo);
        var root = doc.Root;
        var fault_el = root.Element("fault");
        if (fault_el != null)
        {
            var f = Fault.ParseXml(fault_el);

            var msg = string.Format("XMLRPC FAULT [{0}]: \"{1}\"", f.FaultCode, f.FaultString);
            var exc = new XmlRPCException(msg)
            {
                Fault = f
            };

            throw exc;
        }

        var params_el = root.GetElement("params");
        var param_els = params_el.Elements("param").ToList();

        foreach (var param_el in param_els)
        {
            var value_el = param_el.GetElement("value");

            var val = Value.ParseXml(value_el);
            Parameters.Add(val);
        }
    }

    public ParameterList Parameters { get; }
}