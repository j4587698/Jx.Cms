using System.Xml.Linq;

namespace CnBlogAsync;

internal static class XmlExtensions
{
    public static string GetElementString(this XElement parent, string name)
    {
        var child_el = parent.GetElement(name);
        return child_el.Value;
    }

    public static XElement GetElement(this XElement parent, string name)
    {
        var child_el = parent.Element(name);
        if (child_el == null)
        {
            var msg = string.Format("Xml Error: <{0}/> element does not contain <{0}/> element",
                parent.Name, name);
            throw new MetaWeblogException(msg);
        }

        return child_el;
    }
}