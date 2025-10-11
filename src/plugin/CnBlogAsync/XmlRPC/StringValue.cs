using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class StringValue : Value
{
    private static StringValue ns;
    private static StringValue es;
    public string String;

    public StringValue(string s)
    {
        String = s;
    }

    public static string TypeString => "string";

    public static StringValue NullString
    {
        get
        {
            if (ns == null) ns = new StringValue(null);
            return ns;
        }
    }

    public static StringValue EmptyString
    {
        get
        {
            if (es == null) es = new StringValue(string.Empty);
            return es;
        }
    }

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        parent.Value = String;
    }

    public static StringValue XmlToValue(SXL.XElement parent)
    {
        var bv = new StringValue(parent.Value);
        return bv;
    }

    public static implicit operator StringValue(string v)
    {
        return new StringValue(v);
    }

    protected override string GetTypeString()
    {
        return TypeString;
    }
}