using System.Collections;
using SXL = System.Xml.Linq;

namespace CnBlogAsync.XmlRPC
{
    public class Array : Value, IEnumerable<Value>
    {
        private readonly List<Value> items;

        public Array()
        {
            this.items = new List<Value>();
        }

        public Array(int capacity)
        {
            this.items = new List<Value>(capacity);
        }

        public void Add(Value v)
        {
            this.items.Add(v);
        }

        public void Add(int v)
        {
            this.items.Add(new IntegerValue(v));
        }

        public void Add(double v)
        {
            this.items.Add(new DoubleValue(v));
        }

        public void Add(bool v)
        {
            this.items.Add(new BooleanValue(v));
        }

        public void Add(System.DateTime v)
        {
            this.items.Add(new DateTimeValue(v));
        }

        public void AddRange(IEnumerable<Value> items)
        {
            foreach (var item in items)
            {
                this.items.Add(item);
            }
        }

        public Value this[int index]
        {
            get { return this.items[index]; }
        }

        public IEnumerator<Value> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public static string TypeString
        {
            get { return "array"; }
        }

        public int Count
        {
            get { return this.items.Count; }
        }

        protected override void AddToTypeEl(SXL.XElement parent)
        {
            var data_el = new SXL.XElement("data");
            parent.Add(data_el);
            foreach (Value item in this)
            {
                item.AddXmlElement(data_el);
            }
        }

        internal static Array XmlToValue(SXL.XElement type_el)
        {
            var data_el = type_el.GetElement("data");

            var value_els = data_el.Elements("value").ToList();
            var list = new Array();
            foreach (var value_el2 in value_els)
            {
                var o = Value.ParseXml(value_el2);
                list.Add(o);
            }
            return list;
        }

        public override bool Equals(System.Object obj)
        {
            if (obj == null)
            {
                return false;
            }

            var p = obj as Array;
            if (p == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.items != p.items)
            {
                if (this.items.Count!= p.items.Count)
                {
                    return false;
                }

                for (int i = 0; i < this.items.Count; i++)
                {
                    if (!(this.items[i].Equals(p[i])))
                    {
                        return false;
                    }
                }

                return true;
            }
            return true;
        }

        protected override string GetTypeString()
        {
            return Array.TypeString;
        }

        public override int GetHashCode()
        {
            return this.items.GetHashCode();
        }
    }
}