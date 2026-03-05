using System.Net;
using System.Net.Http;
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
        using var handler = new HttpClientHandler();
        if (Cookies != null)
        {
            handler.UseCookies = true;
            handler.CookieContainer = Cookies;
        }

        using var client = new HttpClient(handler);
        using var request = new HttpRequestMessage(HttpMethod.Post, URL)
        {
            Content = new StringContent(doc.ToString(), Encoding.UTF8, "text/xml")
        };
        request.Headers.ExpectContinue = EnableExpect100Continue;

        using var response = client.Send(request);
        response.EnsureSuccessStatusCode();
        var webpageContent = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
        return new MethodResponse(webpageContent);
    }
}
