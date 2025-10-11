using System.Net;
using System.Xml.Linq;

namespace CnBlogAsync;

public class ClientOption
{
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

    public string BlogURL { get; set; }
    public string MetaWeblogURL { get; set; }
    public string BlogID { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }

    public static ClientOption Load(string filename)
    {
        var doc = XDocument.Load(filename);
        var root = doc.Root;

        var blogurl = root.GetElementString("blogurl");
        var blogId = root.GetElementString("blogid");
        var metaWeblogUrl = root.GetElementString("metaweblog_url");
        var username = root.GetElementString("username");
        var password = root.GetElementString("password");
        var coninfo = new ClientOption(blogurl, metaWeblogUrl, blogId, username, password);
        return coninfo;
    }

    public void Save(string filename)
    {
        var doc = new XDocument();
        var p = new XElement("blogconnectioninfo");
        doc.Add(p);
        p.Add(new XElement("blogurl", BlogURL));
        p.Add(new XElement("blogid", BlogID));
        p.Add(new XElement("metaweblog_url", MetaWeblogURL));
        p.Add(new XElement("username", Username));
        p.Add(new XElement("password", Password));
        doc.Save(filename);
    }
}