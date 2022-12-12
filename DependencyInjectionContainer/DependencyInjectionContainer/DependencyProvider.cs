using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        public DependencyProvider(DependenciesConfiguration configuration)
        {

        }

        public object? Resolve<TDep>()
        {
            return null;
        }
    }
}
