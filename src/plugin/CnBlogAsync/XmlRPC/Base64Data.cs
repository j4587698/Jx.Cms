using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class Base64Data : Value
{
    public Base64Data(byte[] bytes)
    {
        if (bytes == null) throw new ArgumentNullException("bytes");
        Bytes = bytes;
    }

    public byte[] Bytes { get; }

    public static string TypeString => "base64";

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        parent.Add(Convert.ToBase64String(Bytes));
    }

    internal static Base64Data XmlToValue(SXL.XElement type_el)
    {
        var bytes = Convert.FromBase64String(type_el.Value);
        var b = new Base64Data(bytes);
        return b;
    }

    public static implicit operator Base64Data(byte[] v)
    {
        return new Base64Data(v);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as Base64Data;
        if (p == null) return false;

        // Return true if the fields match:
        if (Bytes != p.Bytes)
        {
            if (Bytes.Length != p.Bytes.Length) return false;

            for (var i = 0; i < Bytes.Length; i++)
                if (Bytes[i] != p.Bytes[i])
                    return false;

            return true;
        }

        return true;
    }

    protected override string GetTypeString()
    {
        return TypeString;
    }

    public override int GetHashCode()
    {
        return Bytes.GetHashCode();
    }
}