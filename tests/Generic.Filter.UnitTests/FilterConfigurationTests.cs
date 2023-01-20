using Generic.Filter.Criteria;

namespace Generic.Filter.UnitTests
{
    public class FilterConfigurationTests
    {
        [Test]
        public void CorrectFilter_ShouldBeCreatedFromConfiguration()
        {
            var filterConfig = new ParentFilterConfiguration();

            var parents = new List<Parent> 
            { 
                new Parent { FirstName = "John", LastName = "Smith", Age = 40, Child = new Person { FirstName = "Tom", LastName = "Smith", Age = 15 } },
                new Parent { FirstName = "Peter", LastName = "Parker", Age = 35, Child = new Person { FirstName = "Tina", LastName = "Parker", Age = 5 } }
            };

            var filter = filterConfig.CreateFilter();
            filter.ParentLastName = "Smith";
            filter.ParentAge = 40;
            filter.ChildFirstName = "Tom";

            var filteredParents = parents.AsQueryable().Where(filter).ToList();

            Assert.That(filteredParents, Has.Count.EqualTo(1));
        }

        public class Person
        {
            public string FirstName { get; set; }

            public string LastName { get; set; }

            public int Age { get; set; }
        }

        public class Parent : Person
        {
            public Person? Child { get; set; }
        }

        public class ParentFilter : GenericFilter<Parent, ParentFilter>
        {
            public EqualCriterion ParentFirstName { get; set; }

            public EqualCriterion ParentLastName { get; set; }

            public EqualCriterion ParentAge { get; set; }

            public EqualCriterion ChildFirstName { get; set; }

            public EqualCriterion ChildLastName { get; set; }

            public EqualCriterion ChildAge { get; set; }
        }

        public class ParentFilterConfiguration : IFilterConfiguration<Parent, ParentFilter>
        {
            public ParentFilter CreateFilter()
            {
                throw new NotImplementedException();
            }
        }
    }
}
