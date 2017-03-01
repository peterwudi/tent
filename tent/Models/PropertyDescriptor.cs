using System.Collections.Generic;
using System.Dynamic;

namespace Tent
{
    public class PropertyDescriptor
    {
        public ExpandoObject Expando { get; set; }

        public List<PropertyOptions> Options { get; private set; }

        public PropertyDescriptor()
        {
            Options = new List<PropertyOptions>();
        }
    }
}
