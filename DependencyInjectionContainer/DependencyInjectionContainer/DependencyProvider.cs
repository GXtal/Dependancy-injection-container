using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private DependenciesConfiguration _configuration;

        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }

        private object? GetDefaultValue(Type t)
        {
            if (t.IsValueType)
                return Activator.CreateInstance(t);
            else
                return null;
        }

        public TDep? Resolve<TDep>(object enumValue = default)
        {
            Type dependency = typeof(TDep);
            object res=null;
            List<DependencyImplementation> dependencies;
            if (enumValue == null)
                dependencies = _configuration.Dependencies;
            else
            {
                if (!_configuration.EnumDependencies.ContainsKey(enumValue))
                {
                    return (TDep)GetDefaultValue(dependency);
                }                    
                else
                    dependencies = _configuration.EnumDependencies[enumValue];
            }

            if (dependencies.Any(p =>
            {
                return (p.DependencyType == dependency)||((dependency.IsGenericType)&&(dependency.GetGenericTypeDefinition()==p.DependencyType))
                        ||(dependency.IsGenericType && dependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
            }
            ))
            {
                if (dependency.IsGenericType && dependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    var temp = ResolveMany(dependency.GetGenericArguments().FirstOrDefault(),enumValue).Where(r => r != null);

                    res = (TDep)typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(dependency.GetGenericArguments()).Invoke(null, new object[] { temp });
                }
                else
                    res = (TDep)(ResolveMany(dependency, enumValue).FirstOrDefault(r => r != null));
            }
            else
            {
                res=GetDefaultValue(dependency);
            }            
            return (TDep)res;
        }

        public object Resolve(Type dependency, object enumValue = default)
        {
            object res = null;
            List<DependencyImplementation> dependencies;
            if (enumValue == null)
                dependencies = _configuration.Dependencies;
            else
            {
                if (!_configuration.EnumDependencies.ContainsKey(enumValue))
                {
                    return GetDefaultValue(dependency);
                }
                else
                    dependencies = _configuration.EnumDependencies[enumValue];
            }
            if (dependencies.Any(p =>
            {
                return (p.DependencyType == dependency) || ((dependency.IsGenericType) && (dependency.GetGenericTypeDefinition() == p.DependencyType))
                        || (dependency.IsGenericType && dependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)));
            }))
            {
                if (dependency.IsGenericType && dependency.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    var temp = ResolveMany(dependency.GetGenericArguments().FirstOrDefault(), enumValue).Where(r => r != null);

                    res = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(dependency.GetGenericArguments()).Invoke(null, new object[] { temp });
                }
                else
                    res = ResolveMany(dependency, enumValue).FirstOrDefault(r => r != null);
            }
            else
            {
                res = GetDefaultValue(dependency);
            }
            return res;
        }



        private IEnumerable<object?> ResolveMany(Type type, object enumValue = default)
        {
            List<DependencyImplementation> dependencies;
            if (enumValue == null)
                dependencies = _configuration.Dependencies;
            else
            {
                if (!_configuration.EnumDependencies.ContainsKey(enumValue))
                    return null;
                else
                    dependencies = _configuration.EnumDependencies[enumValue];
            }


            return dependencies.Select(c =>
            {
                if (c.DependencyType == type)
                {
                    if (c.IsSingleton)
                    {                        
                        if (c.SingletonImplementation==null)
                        {
                            c.SingletonImplementation = GenerateObject(c.ImplementationType);
                        }

                        return c.SingletonImplementation;
                    }
                    else
                    {
                        return GenerateObject(c.ImplementationType);
                    }
                }
                if (c.DependencyType.IsGenericTypeDefinition && type.IsGenericType && type.GetGenericTypeDefinition() == c.DependencyType)
                {
                    if (c.IsSingleton && c.SingletonImplementation != null)
                    {
                        return c.SingletonImplementation;
                    }
                    return GenerateObject(c.ImplementationType.MakeGenericType(type.GetGenericArguments()));
                }

                return null;
            });
        }

        private object GenerateObject(Type type)
        {
            var constructor = type.GetConstructors().Single();
            var cParams = constructor.GetParameters();

            var genParams = new List<object>();

            foreach(var param in cParams)
            {
                if (param.GetCustomAttributes(false).Any(a => a is DependencyKeyAttribute))
                {
                    var enumValue = (param.GetCustomAttributes(false).FirstOrDefault(a => a is DependencyKeyAttribute) as DependencyKeyAttribute).enumValue;
                    genParams.Add(Resolve(param.ParameterType, enumValue));

                }else                
                genParams.Add(Resolve(param.ParameterType));
            }

            return constructor.Invoke(genParams.ToArray());
        }
    }
}
