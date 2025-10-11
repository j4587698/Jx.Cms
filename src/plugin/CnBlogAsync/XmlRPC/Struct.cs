using System.Collections;
using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC;

public class Struct : Value, IEnumerable<KeyValuePair<string, Value>>
{
    private readonly Dictionary<string, Value> dic;

    public Struct()
    {
        dic = new Dictionary<string, Value>();
    }

    public Value this[string index]
    {
        get => Get(index);
        set => dic[index] = value;
    }

    public int Count => dic.Count;

    public static string TypeString => "struct";

    public IEnumerator<KeyValuePair<string, Value>> GetEnumerator()
    {
        return dic.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private bool TryGet(string name, out Value v)
    {
        return dic.TryGetValue(name, out v);
    }

    public Value TryGet(string name)
    {
        Value v = null;
        var b = dic.TryGetValue(name, out v);
        return v;
    }

    private void checktype<T>(Value v)
    {
        var expected = typeof(T);
        var actual = v.GetType();
        if (expected != actual)
        {
            var msg = string.Format("Expected type {0} instead got {1}", expected.Name, actual.Name);
            throw new XmlRPCException(msg);
        }
    }

    public T TryGet<T>(string name) where T : Value
    {
        var v = TryGet(name);
        if (v == null) return null;

        checktype<T>(v);

        return (T)v;
    }

    public T Get<T>(string name, T defval) where T : Value
    {
        var v = TryGet(name);
        if (v == null) return defval;

        checktype<T>(v);

        return (T)v;
    }

    public Value Get(string name)
    {
        var v = TryGet(name);
        if (v == null)
        {
            var msg = string.Format("Struct does not contains {0}", name);
            throw new XmlRPCException(msg);
        }

        return v;
    }

    public T Get<T>(string name) where T : Value
    {
        var v = Get(name);
        checktype<T>(v);
        return (T)v;
    }

    public bool ContainsKey(string name)
    {
        return dic.ContainsKey(name);
    }

    protected override void AddToTypeEl(SXL.XElement parent)
    {
        foreach (var pair in this)
        {
            var member_el = new SXL.XElement("member");
            parent.Add(member_el);

            var name_el = new SXL.XElement("name");
            member_el.Add(name_el);
            name_el.Value = pair.Key;

            pair.Value.AddXmlElement(member_el);
        }
    }

    public static Struct XmlToValue(SXL.XElement type_el)
    {
        var member_els = type_el.Elements("member").ToList();
        var struct_ = new Struct();
        foreach (var member_el in member_els)
        {
            var name_el = member_el.GetElement("name");
            var name = name_el.Value;

            var value_el2 = member_el.GetElement("value");
            var o = ParseXml(value_el2);

            struct_[name] = o;
        }

        return struct_;
    }

    public override bool Equals(object obj)
    {
        if (obj == null) return false;

        var p = obj as Struct;
        if (p == null) return false;

        // Return true if the fields match:
        if (dic != p.dic)
        {
            if (dic.Count != p.dic.Count) return false;

            foreach (var src_pair in this)
            {
                Value des_val = null;
                var b = p.TryGet(src_pair.Key, out des_val);

                if (!b) return false;

                if (!src_pair.Value.Equals(des_val)) return false;
            }

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
        return dic.GetHashCode();
    }
}