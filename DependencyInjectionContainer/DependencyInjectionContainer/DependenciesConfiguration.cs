namespace DependencyInjectionContainer
{
    public class DependenciesConfiguration
    {
        public readonly List<DependencyImplementation> Dependencies;
        public readonly Dictionary<object, List<DependencyImplementation>> EnumDependencies;
        public DependenciesConfiguration()
        {
            Dependencies = new List<DependencyImplementation>();
            EnumDependencies = new Dictionary<object, List<DependencyImplementation>>();
        }

        public void Register<TDep,TImp>(object enumValue = default, bool isSingleton = false)
        {
            ConfRegister(typeof(TDep), typeof(TImp), enumValue, isSingleton);
        }

        public void Register(Type dep, Type imp,object enumValue = default, bool isSingleton = false)
        {
            ConfRegister(dep, imp, enumValue, isSingleton);
        }

        private void ConfRegister(Type dependency, Type implementation, object enumValue, bool isSingleton = false)
        {
            List<DependencyImplementation> dependencies;

            if(!IsValid(dependency, implementation))
            {
                throw new ArgumentException("создание невозможног соответсвия");
            }

            if (enumValue == null)
            {
                dependencies = Dependencies;
            }
            else
            {
                if (!EnumDependencies.ContainsKey(enumValue))
                {
                    EnumDependencies.Add(enumValue, new List<DependencyImplementation>());
                }
                dependencies = EnumDependencies[enumValue];
            }


            if (dependencies.Any(d => d.DependencyType == dependency && d.ImplementationType == implementation))
            {
                if (dependencies.Where(d => d.DependencyType == dependency && d.ImplementationType == implementation).First().IsSingleton == isSingleton)
                    throw new ArgumentException("Такая зависимость уже определена");
                
            }
            else
            {
                if ((dependency.IsGenericType)&&(implementation.IsGenericType))
                {
                    dependency = dependency.GetGenericTypeDefinition();
                    implementation = implementation.GetGenericTypeDefinition();
                }
                    
                var node = new DependencyImplementation(dependency, implementation, isSingleton);
                dependencies.Add(node);
            }
        }

        private bool IsValid(Type dependency,Type implementation)
        {            
            return !(implementation.IsAbstract || implementation.IsInterface)
              && (implementation.IsGenericTypeDefinition || implementation.IsAssignableTo(dependency));
        }
    }
}