namespace TestProject
{
    public class ConfigTest
    {
        private DependenciesConfiguration configuration = null!;

        [SetUp]
        public void Setup()
        {
            configuration = new DependenciesConfiguration();
        }

        [Test]
        public void SimpleDependancy_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();

            var a = configuration.Dependencies.ElementAt(0);

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.IsSingleton, Is.EqualTo(false));
                Assert.That(a.DependencyType,Is.EqualTo(typeof(InputData.IService)));
                Assert.That(a.ImplementationType,Is.EqualTo(typeof(InputData.Service1)));
            });        
            
        }

        [Test]
        public void GenericDependancy_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();
            configuration.Register<IEnumerable<InputData.IService>,List<InputData.IService>>();

            var a = configuration.Dependencies.ElementAt(1);

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.IsSingleton, Is.EqualTo(false));
                Assert.That(a.DependencyType, Is.EqualTo(typeof(IEnumerable<InputData.IService>).GetGenericTypeDefinition()));
                Assert.That(a.ImplementationType, Is.EqualTo(typeof(List<InputData.IService>).GetGenericTypeDefinition()));
            });

        }

        [Test]
        public void TypeOfGenericDependancy_Test()
        {
            configuration.Register<InputData.IService, InputData.Service1>();
            configuration.Register(typeof(IEnumerable<>), typeof(List<>));

            var a = configuration.Dependencies.ElementAt(1);

            Assert.Multiple(() =>
            {
                Assert.That(a, Is.Not.Null);
                Assert.That(a.IsSingleton, Is.EqualTo(false));
                Assert.That(a.DependencyType, Is.EqualTo(typeof(IEnumerable<InputData.IService>).GetGenericTypeDefinition()));
                Assert.That(a.ImplementationType, Is.EqualTo(typeof(List<InputData.IService>).GetGenericTypeDefinition()));
            });

        }
    }
}
