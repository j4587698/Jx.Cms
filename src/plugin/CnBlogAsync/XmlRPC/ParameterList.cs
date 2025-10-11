using System.Collections;

namespace CnBlogAsync.XmlRPC;

public class ParameterList : IEnumerable<Value>
{
    private readonly List<Value> Parameters;

    public ParameterList()
    {
        Parameters = new List<Value>();
    }

    public Value this[int index] => Parameters[index];

    public int Count => Parameters.Count;

    public IEnumerator<Value> GetEnumerator()
    {
        return Parameters.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(Value value)
    {
        if (value == null) throw new ArgumentNullException("value");
        Parameters.Add(value);
    }

    public void Add(int value)
    {
        Add(new IntegerValue(value));
    }

    public void Add(bool value)
    {
        Add(new BooleanValue(value));
    }

    public void Add(DateTime value)
    {
        Add(new DateTimeValue(value));
    }

    public void Add(double value)
    {
        Add(new DoubleValue(value));
    }

    public void Add(Array value)
    {
        Parameters.Add(value);
    }

    public void Add(Struct value)
    {
        Parameters.Add(value);
    }

    public void Add(Base64Data value)
    {
        Parameters.Add(value);
    }

    public void Add(string value)
    {
        Add(new StringValue(value));
    }
}