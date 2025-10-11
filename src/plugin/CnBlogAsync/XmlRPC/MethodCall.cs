using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class MethodCall
{
    public MethodCall(string name)
    {
        Name = name;
        Parameters = new ParameterList();
    }

    public ParameterList Parameters { get; }
    public string Name { get; }

    public SXL.XDocument CreateDocument()
    {
        var doc = new SXL.XDocument();
        var root = new SXL.XElement("methodCall");

        doc.Add(root);

        var method = new SXL.XElement("methodName");
        root.Add(method);

        method.Add(Name);

        var params_el = new SXL.XElement("params");
        root.Add(params_el);

        foreach (var p in Parameters)
        {
            var param_el = new SXL.XElement("param");
            params_el.Add(param_el);

            p.AddXmlElement(param_el);
        }

        return doc;
    }
}