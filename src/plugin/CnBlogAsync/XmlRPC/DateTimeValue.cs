using System.Globalization;
using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class DateTimeValue : Value
{
    public readonly DateTime Data;

    public DateTimeValue(DateTime value)
    {
        Data = value;
    }

    public static string TypeString => "dateTime.iso8601";

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        var s = Data.ToString("s", CultureInfo.InvariantCulture);
        s = s.Replace("-", "");
        parent.Value = s;
    }

    public static DateTimeValue XmlToValue(SXL.XElement parent)
    {
        var dt = DateTime.Now;
        if (DateTime.TryParse(parent.Value, out dt)) return new DateTimeValue(dt);

        var date = parent.Value.Trim('Z'); // remove Z from SharePoint date

        var x = DateTime.ParseExact(date, "yyyyMMddTHH:mm:ss", null);
        var y = new DateTimeValue(x);
        return y;
    }

    public static implicit operator DateTimeValue(DateTime v)
    {
        return new DateTimeValue(v);
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as DateTimeValue;
        if (p == null) return false;

        // Return true if the fields match:
        return Data.Day == p.Data.Day && Data.Month == p.Data.Month && Data.Year == p.Data.Year &&
               Data.Hour == p.Data.Hour && Data.Minute == p.Data.Minute && Data.Second == p.Data.Second;
    }

    public override int GetHashCode()
    {
        return Data.GetHashCode();
    }

    protected override string GetTypeString()
    {
        return TypeString;
    }
}