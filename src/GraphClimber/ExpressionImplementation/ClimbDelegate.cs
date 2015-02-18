namespace GraphClimber
{
    public delegate void ClimbDelegate<T>(object processor, T value);

    public delegate void StructClimbDelegate<T>(object processor, ref T value);

    //public delegate void Setter<T>(ref object instance, T value);

    public class FastBox<T> : IBox
    {

        public FastBox(T value)
        {
            Value = value;
        }

        public T Value;

        object IBox.Value
        {
            get { return Value; }
        }

    }

    public interface IBox

    {

        object Value { get; }

    }
}