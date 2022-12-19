using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyImplementation
    {
        private object _singletone;
        private object locker;
        public object SingletonImplementation {
            get
            { 
                lock(locker)
                    return _singletone;
                
            }
            set
            {
                lock(locker)
                    _singletone = value;
            }
        }

        public Type DependencyType { get; }

        public Type ImplementationType { get; }

        public bool IsSingleton { get; set; }

        public DependencyImplementation(Type dependency, Type implementation, bool isSingleton)
        {
            locker = new object();
            DependencyType = dependency;
            ImplementationType = implementation;
            IsSingleton = isSingleton;
        }
    }
}
