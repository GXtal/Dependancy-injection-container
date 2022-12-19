using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyImplementation
    {
        public object SingletonImplementation { get; set; }

        public Type DependencyType { get; }

        public Type ImplementationType { get; }

        public bool IsSingleton { get; set; }

        public DependencyImplementation(Type dependency, Type implementation, bool isSingleton)
        {
            DependencyType = dependency;
            ImplementationType = implementation;
            IsSingleton = isSingleton;
        }
    }
}
