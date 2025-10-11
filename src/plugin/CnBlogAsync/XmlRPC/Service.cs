using System.Net;
using System.Text;

namespace CnBlogAsync.XmlRPC;

public class Service
{
    private readonly bool EnableExpect100Continue = false;

    public CookieContainer Cookies = null;

    public Service(string url)
    {
        URL = url;
    }

    public string URL { get; }

    public MethodResponse Execute(MethodCall methodcall)
    {
        var doc = methodcall.CreateDocument();
        var request = WebRequest.Create(URL);

        if (Cookies != null)
        {
            var hRequest = request as HttpWebRequest;
            hRequest.CookieContainer = Cookies;
        }

        var wr = (HttpWebRequest)request;
        wr.ServicePoint.Expect100Continue = EnableExpect100Continue;
        request.Method = "POST";
        var content = doc.ToString();
        var byteArray = Encoding.UTF8.GetBytes(content);
        request.ContentType = "text/xml;charset=utf-8";
        request.ContentLength = byteArray.Length;

        using (var webpageStream = request.GetRequestStream())
        {
            webpageStream.Write(byteArray, 0, byteArray.Length);
        }

        using (var webResponse = (HttpWebResponse)request.GetResponse())
        {
            using (var responseStream = webResponse.GetResponseStream())
            {
                if (responseStream == null) throw new XmlRPCException("Response Stream is unexpectedly null");

                using (var reader = new StreamReader(responseStream))
                {
                    var webpageContent = reader.ReadToEnd();
                    var response = new MethodResponse(webpageContent);
                    return response;
                }
            }
        }
    }
}