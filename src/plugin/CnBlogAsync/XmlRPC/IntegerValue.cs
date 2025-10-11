using System.Globalization;
using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class IntegerValue : Value
{
    public readonly int Integer;

    public IntegerValue(int i)
    {
        Integer = i;
    }

    public static string TypeString => "int";

    public static string AlternateTypeString => "i4";

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        parent.Value = Integer.ToString(CultureInfo.InvariantCulture);
    }

    public static IntegerValue XmlToValue(SXL.XElement parent)
    {
        var bv = new IntegerValue(int.Parse(parent.Value));
        return bv;
    }

    public static implicit operator IntegerValue(int v)
    {
        return new IntegerValue(v);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as IntegerValue;
        if (p == null) return false;

        // Return true if the fields match:
        return Integer == p.Integer;
    }

    public override int GetHashCode()
    {
        return Integer.GetHashCode();
    }

    protected override string GetTypeString()
    {
        return TypeString;
    }
}