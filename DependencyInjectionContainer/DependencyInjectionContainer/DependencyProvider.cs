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

        public TDep? Resolve<TDep>()
        {
            Type dependancy = typeof(TDep);
            object res=null;
            if (_configuration.Dependencies.Any(p =>
            {
                return p.DependencyType == dependancy;
            }))
            {
                if (dependancy.IsGenericType && dependancy.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    var temp = ResolveMany(dependancy.GetGenericArguments().FirstOrDefault()).Where(r => r != null);

                    res = (TDep)typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(dependancy.GetGenericArguments()).Invoke(null, new object[] { temp });
                }
                else
                    res = (TDep)(ResolveMany(dependancy).FirstOrDefault(r => r != null);
            }
            else
            {
                res=GetDefaultValue(dependancy);
            }            
            return (TDep)res;
        }

        public object Resolve(Type dependancy)
        {
            object res = null;
            if (_configuration.Dependencies.Any(p =>
            {
                return p.DependencyType == dependancy;
            }))
            {
                if (dependancy.IsGenericType && dependancy.GetGenericTypeDefinition().Equals(typeof(IEnumerable<>)))
                {
                    var temp = ResolveMany(dependancy.GetGenericArguments().FirstOrDefault()).Where(r => r != null);

                    res = typeof(Enumerable).GetMethod("Cast").MakeGenericMethod(dependancy.GetGenericArguments()).Invoke(null, new object[] { temp });
                }
                else
                    res = ResolveMany(dependancy).FirstOrDefault(r => r != null);
            }
            else
            {
                res = GetDefaultValue(dependancy);
            }
            return res;
        }



        private IEnumerable<object?> ResolveMany(Type type)
        {
            List<DependencyImplementation> dependencies;
            dependencies = _configuration.Dependencies;


            return dependencies.Select(c =>
            {
                if (c.DependencyType == type)
                {
                    if (c.IsSingleton)
                    {                        
                        if (c.SingletonImplementation==null)
                        {
                            c.SingletonImplementation = GenerateObject(c.DependencyType);
                        }

                        return c.SingletonImplementation;
                    }
                    else
                    {
                        GenerateObject(c.ImplementationType);
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
                genParams.Add(Resolve(param.ParameterType));
            }

            return constructor.Invoke(genParams.ToArray());
        }
    }
}
