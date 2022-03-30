using System.Net;

namespace CnBlogAsync
{
    public class ClientOption
    {
        public string BlogURL { get; set; }
        public string MetaWeblogURL { get; set; }
        public string BlogID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        public static ClientOption Load(string filename)
        {
            var doc = System.Xml.Linq.XDocument.Load(filename);
            var root = doc.Root;

            string blogurl = root.GetElementString("blogurl");
            string blogId = root.GetElementString("blogid");
            string metaWeblogUrl = root.GetElementString("metaweblog_url");
            string username = root.GetElementString("username");
            string password = root.GetElementString("password");
            var coninfo = new ClientOption(blogurl, metaWeblogUrl, blogId, username, password);
            return coninfo;
        }

        public void Save(string filename)
        {
            var doc = new System.Xml.Linq.XDocument();
            var p = new System.Xml.Linq.XElement("blogconnectioninfo");
            doc.Add(p);
            p.Add(new System.Xml.Linq.XElement("blogurl", BlogURL));
            p.Add(new System.Xml.Linq.XElement("blogid", BlogID));
            p.Add(new System.Xml.Linq.XElement("metaweblog_url", MetaWeblogURL));
            p.Add(new System.Xml.Linq.XElement("username", Username));
            p.Add(new System.Xml.Linq.XElement("password", Password));
            doc.Save(filename);
        }
        
        public CookieContainer Cookies = null;

        public ClientOption(string blogurl, string metaweblogurl, string blogid, string username, string password)
        {
            BlogURL = blogurl;
            BlogID = blogid;
            MetaWeblogURL = metaweblogurl;
            Username = username;
            Password = password;
        }

        public ClientOption()
        {
        }
    }
}