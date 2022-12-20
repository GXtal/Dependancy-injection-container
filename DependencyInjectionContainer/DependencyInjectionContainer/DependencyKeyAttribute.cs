using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DependencyKeyAttribute : Attribute
    {
        public object enumValue { get; }

        public DependencyKeyAttribute(object enumValue)
        {
            this.enumValue = enumValue;
        }
    }
}
