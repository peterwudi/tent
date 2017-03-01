namespace Tent
{
    public class PropertyOptions
    {
        public string Name { get; set; }

        public string OriginalString { get; set; }

        public PropertyKind Kind { get; set; }

        public object DefaultValue { get; set; }
    }
}
