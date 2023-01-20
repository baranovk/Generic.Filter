using System.Linq.Expressions;
using System.Reflection;
using Generic.Filter.Criteria;

namespace Generic.Filter.UnitTests
{
    public class FilteringTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [Explicit]
        public void TestExpressions()
        {
            var models = new List<Model>
            {
                new Model { FirstName = "John", LastName = "Smith" },
                new Model { FirstName = "Jane", LastName = "Smith" }
            };

            Expression<Func<ModelFilter, Func<Model, bool>>> ex = filter => new Func<Model, bool>(m => filter.FirstName.IsNull || m.FirstName == filter.FirstName);
                

            var filter = new ModelFilter { FirstName = "John", LastName = "Smith" };
            var filteredModels = models.AsQueryable().Where(filter).ToList();

            Assert.That(filteredModels, Has.Count.EqualTo(1));
        }

        [Test]
        public void Models_ShoulMatchExactFilter()
        {
            var models = new List<Model> 
            { 
                new Model { FirstName = "John", LastName = "Smith" }, 
                new Model { FirstName = "Jane", LastName = "Smith" } 
            };

            var filter = new ModelFilter { FirstName = "John", LastName = "Smith" };
            var filteredModels = models.AsQueryable().Where(filter).ToList();
            
            Assert.That(filteredModels, Has.Count.EqualTo(1));

            filter = new ModelFilter { LastName = "Smith" };
            filteredModels = models.AsQueryable().Where(filter).ToList();

            Assert.That(filteredModels, Has.Count.EqualTo(2));
        }

        [Test]
        public void Models_ShoulMatchPartialFilter()
        {
            var models = new List<Model>
            {
                new Model { FirstName = "Tom", LastName = "Perry" },
                new Model { FirstName = "John", LastName = "Smith" },
                new Model { FirstName = "Jane", LastName = "Smith" }
            };

            var filter = new ModelFilter { LastName = "Smith" };
            var filteredModels = models.AsQueryable().Where(filter).ToList();

            Assert.That(filter.FirstName.IsNull, Is.True);
            Assert.That(filteredModels, Has.Count.EqualTo(2));
        }

        private class Model
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }
        }

        private class ModelFilter : GenericFilter<Model, ModelFilter>
        {
            public EqualCriterion FirstName { get; set; }

            public EqualCriterion LastName { get; set; }
        }
    }
}