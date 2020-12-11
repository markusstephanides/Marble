namespace Marble.Core.Serialization.Json
{
    public abstract class ValueContainerBase {
    
    }
    
    public class Int32Container : ValueContainerBase
    {
        public int Value { get; set; }

        public Int32Container(object value)
        {
            this.Value = (int)value;
        }
    }
    
    public class Int16Container : ValueContainerBase
    {
        public short Value { get; set; }
        
        public Int16Container(object value)
        {
            this.Value = (short)value;
        }
    }
    
    public class Int8Container : ValueContainerBase
    {
        public byte Value { get; set; }
        
        public Int8Container(object value)
        {
            this.Value = (byte)value;
        }
    }
    
    public class FloatContainer : ValueContainerBase
    {
        public float Value { get; set; }
        
        public FloatContainer(object value)
        {
            this.Value = (float)value;
        }
    }
    
    public class DoubleContainer : ValueContainerBase
    {
        public double Value { get; set; }
        
        public DoubleContainer(object value)
        {
            this.Value = (double)value;
        }
    }
    
    public class DecimalContainer : ValueContainerBase
    {
        public decimal Value { get; set; }
        
        public DecimalContainer(object value)
        {
            this.Value = (decimal)value;
        }
    }
}