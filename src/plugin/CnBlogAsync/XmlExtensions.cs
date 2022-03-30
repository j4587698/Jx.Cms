namespace CnBlogAsync
{
    static class XmlExtensions
    {
        public static string GetElementString(this System.Xml.Linq.XElement parent, string name)
        {
            var child_el = parent.GetElement(name);
            return child_el.Value;
        }

        public static System.Xml.Linq.XElement GetElement(this System.Xml.Linq.XElement parent, string name)
        {
            var child_el = parent.Element(name);
            if (child_el == null)
            {
                string msg = string.Format("Xml Error: <{0}/> element does not contain <{0}/> element",
                                           parent.Name, name);
                throw new MetaWeblogException(msg);
            }

            return child_el;
        }
    }
}