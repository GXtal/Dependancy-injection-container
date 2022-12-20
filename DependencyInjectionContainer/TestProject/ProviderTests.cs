using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestProject
{
    public class ProviderTests
    {
        private DependenciesConfiguration configuration = null!;
        private DependencyProvider provider = null!;

        [SetUp]
        public void Setup()
        {
            configuration = new DependenciesConfiguration();
        }

        [Test]
        public void SimpleDependancy_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();
            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<InputData.IService>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.GetType(), Is.EqualTo(typeof(InputData.Service1)));
            });
        }

        [Test]
        public void GenericDependancy_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();
            configuration.Register<InputData.IService, InputData.Service2>();
            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<IEnumerable<InputData.IService>>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.Count,Is.EqualTo(2));
            });

        }
        [Test]
        public void SingleTone_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>(isSingleton:true);
            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<InputData.IService>();
            var b = provider.Resolve<InputData.IService>();
            var c = provider.Resolve<InputData.IService>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(b, Is.Not.Null);
                Assert.That(c, Is.Not.Null);

                Assert.That(a, Is.EqualTo(b));
                Assert.That(a, Is.EqualTo(c));
            });

        }

        [Test]
        public void IEnumerable_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();
            configuration.Register<InputData.IService, InputData.Service2>();
            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<IEnumerable<InputData.IService>>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.Count(), Is.EqualTo(2));
            });
        }

        [Test]
        public void Generic_Test()
        {
            configuration.Register<InputData.IRepository, InputData.RepositoryImpl>();
            configuration.Register<InputData.IService<InputData.IRepository>, InputData.ServiceImpl<InputData.IRepository>>();

            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<InputData.IService<InputData.IRepository>>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.GetType(), Is.EqualTo(typeof(InputData.ServiceImpl<InputData.IRepository>)));
                Assert.That((a as InputData.ServiceImpl<InputData.IRepository>).Repository, Is.Not.Null);
                Assert.That((a as InputData.ServiceImpl<InputData.IRepository>).Repository.GetType(), Is.EqualTo(typeof(InputData.RepositoryImpl)));
            });
        }

        [Test]
        public void Generic2_Test()
        {
            configuration.Register<InputData.IRepository, InputData.RepositoryImpl>();
            configuration.Register(typeof(InputData.IService<>), typeof(InputData.ServiceImpl<>));

            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<InputData.IService<InputData.RepositoryImpl>>();

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.GetType(), Is.EqualTo(typeof(InputData.ServiceImpl<InputData.RepositoryImpl>)));
            });
        }

        [Test]
        public void Enum_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>(InputData.ServiceImplementations.First);
            configuration.Register<InputData.IService, InputData.Service2>(InputData.ServiceImplementations.Second);
            provider = new DependencyProvider(configuration);

            var a = provider.Resolve<InputData.IService>(InputData.ServiceImplementations.First);
            var b = provider.Resolve<InputData.IService>(InputData.ServiceImplementations.Second);

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.GetType(), Is.EqualTo(typeof(InputData.Service1)));

                Assert.That(b, Is.Not.Null);
                Assert.That(b.GetType(), Is.EqualTo(typeof(InputData.Service2)));
            });
        }
    }
}
