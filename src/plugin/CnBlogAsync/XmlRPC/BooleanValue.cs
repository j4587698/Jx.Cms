using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class BooleanValue : Value
{
    public readonly bool Boolean;

    public BooleanValue(bool value)
    {
        Boolean = value;
    }

    public static string TypeString => "boolean";

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        if (Boolean)
            parent.Add("1");
        else
            parent.Add("0");
    }

    public static BooleanValue XmlToValue(SXL.XElement type_el)
    {
        var i = int.Parse(type_el.Value);
        var b = i != 0;
        var bv = new BooleanValue(b);
        return bv;
    }

    public static implicit operator BooleanValue(bool v)
    {
        return new BooleanValue(v);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as BooleanValue;
        if (p == null) return false;

        // Return true if the fields match:
        return Boolean == p.Boolean;
    }

    public override int GetHashCode()
    {
        return Boolean.GetHashCode();
    }

    protected override string GetTypeString()
    {
        return TypeString;
    }
}