using SXL=System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public class MethodCall
    {
        public ParameterList Parameters { get; private set; }
        public string Name { get; private set; }

        public MethodCall(string name)
        {
            this.Name = name;
            this.Parameters = new ParameterList();
        }

        public SXL.XDocument CreateDocument()
        {
            var doc = new SXL.XDocument();
            var root = new SXL.XElement("methodCall");

            doc.Add(root);

            var method = new SXL.XElement("methodName");
            root.Add(method);

            method.Add(this.Name);

            var params_el = new SXL.XElement("params");
            root.Add(params_el);

            foreach (var p in this.Parameters)
            {
                var param_el = new SXL.XElement("param");
                params_el.Add(param_el);

                p.AddXmlElement(param_el);
            }

            return doc;
        }
    }
}