using System.Collections;
using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class Array : Value, IEnumerable<Value>
{
    private readonly List<Value> items;

    public Array()
    {
        items = new List<Value>();
    }

    public Array(int capacity)
    {
        items = new List<Value>(capacity);
    }

    public Value this[int index] => items[index];

    public static string TypeString => "array";

    public int Count => items.Count;

    public IEnumerator<Value> GetEnumerator()
    {
        return items.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Value v)
    {
        items.Add(v);
    }

    public void Add(int v)
    {
        items.Add(new IntegerValue(v));
    }

    public void Add(double v)
    {
        items.Add(new DoubleValue(v));
    }

    public void Add(bool v)
    {
        items.Add(new BooleanValue(v));
    }

    public void Add(DateTime v)
    {
        items.Add(new DateTimeValue(v));
    }

    public void AddRange(IEnumerable<Value> items)
    {
        foreach (var item in items) this.items.Add(item);
    }

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        var data_el = new SXL.XElement("data");
        parent.Add(data_el);
        foreach (var item in this) item.AddXmlElement(data_el);
    }

    internal static Array XmlToValue(SXL.XElement type_el)
    {
        var data_el = type_el.GetElement("data");

        var value_els = data_el.Elements("value").ToList();
        var list = new Array();
        foreach (var value_el2 in value_els)
        {
            var o = ParseXml(value_el2);
            list.Add(o);
        }

        return list;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as Array;
        if (p == null) return false;

        // Return true if the fields match:
        if (items != p.items)
        {
            if (items.Count != p.items.Count) return false;

            for (var i = 0; i < items.Count; i++)
                if (!items[i].Equals(p[i]))
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
        return items.GetHashCode();
    }
}