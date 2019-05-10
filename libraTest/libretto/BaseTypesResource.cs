using libra.core;

namespace libraTest.libretto
{
    public class BaseTypesResource : Resource
    {
        public int IntField { get; set; }
        public long LongField { get; set; }
        public byte ByteField { get; set; }

        public float FloatField { get; set; }
        public double DoubleField { get; set; }

        public string StringField { get; set; }
        
        public bool BoolField { get; set; }
    }
}