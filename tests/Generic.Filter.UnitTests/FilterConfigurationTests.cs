using Generic.Filter.Criteria;
using System.Linq.Expressions;

namespace Generic.Filter.UnitTests
{
    public class FilterConfigurationTests
    {
        private List<Parent> _parents;

        [SetUp]
        public void SetUp()
        {
            _parents = new List<Parent>
            {
                new Parent { FirstName = "John", LastName = "Smith", Age = 40, Child = new Person { FirstName = "Tom", LastName = "Smith", Age = 15 } },
                new Parent { FirstName = "Peter", LastName = "Parker", Age = 35, Child = new Person { FirstName = "Tina", LastName = "Parker", Age = 5 } }
            };
        }

        [Test]
        public void FilterWithPropertyMappings_ShouldFilterItemsCorrectly()
        {
            var filter = new ParentFilter(
                new FilterMappings<Parent, ParentFilter>()
                    .ForMember(p => p.FirstName, f => f.ParentFirstName)
                    .ForMember(p => p.LastName, f => f.ParentLastName)
                    .ForMember(p => p.Age, f => f.ParentAge)
            );

            filter.ParentLastName = "Smith";
            filter.ParentAge = 40;

            var filteredParents = _parents.AsQueryable().Where(filter).ToList();
            Assert.That(filteredParents, Has.Count.EqualTo(1));

            Expression<Func<ParentFilter, Func<Parent, bool>>> ex = filter => new Func<Parent, bool>(m => m.Child.FirstName == filter.ChildFirstName);
            ex.Compile();
        }

        [Test]
        public void FilterWithPropertyMappings_ShouldFilterItemsCorrectlyBySecondLevelProperties()
        {
            var filter = new ParentFilter(
                new FilterMappings<Parent, ParentFilter>()
                    .ForPath(p => p.Child.LastName, f => f.ChildLastName)
            );
            
            filter.ChildLastName = "Turner";

            var parents = new List<Parent>
            {
                new Parent { FirstName = "John", LastName = "Smith", Age = 40, Child = new Person { FirstName = "Tina", LastName = "Burner", Age = 15 } },
                new Parent { FirstName = "Peter", LastName = "Parker", Age = 35, Child = new Person { FirstName = "Tina", LastName = "Turner", Age = 5 } }
            };

            var filteredParents = parents.AsQueryable().Where(filter).ToList();
            Assert.That(filteredParents, Has.Count.EqualTo(1));
        }

        [Test]
        public void FilterWithPropertyMappings_ShouldFilterItemsCorrectlyBySecondLevelProperties_IfSecondLevelPropertyIsNull()
        {
            var filter = new ParentFilter(
                new FilterMappings<Parent, ParentFilter>()
                    .ForMember(p => p.Child.LastName, f => f.ChildLastName)
            );

            filter.ChildLastName = "Parker";

            var filteredParents = _parents.AsQueryable().Where(filter).ToList();
            Assert.That(filteredParents, Has.Count.EqualTo(1));
        }

        [Test]
        public void NotMatchingFilterWithPropertyMappings_ShouldNotMatchAnyItem()
        {
            var filter = new ParentFilter(
                new FilterMappings<Parent, ParentFilter>()
                    .ForMember(p => p.FirstName, f => f.ParentFirstName)
                    .ForMember(p => p.LastName, f => f.ParentLastName)
                    .ForMember(p => p.Age, f => f.ParentAge)
            )
            {
                ParentLastName = "Smith",
                ParentAge = 32
            };

            var filteredParents = _parents.AsQueryable().Where(filter).ToList();
            Assert.That(filteredParents, Has.Count.EqualTo(0));
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
            public ParentFilter(FilterMappings<Parent, ParentFilter> propertyMappings) : base(propertyMappings)
            {
            }

            public EqualCriterion ParentFirstName { get; set; }

            public EqualCriterion ParentLastName { get; set; }

            public EqualCriterion ParentAge { get; set; }

            public EqualCriterion ChildFirstName { get; set; }

            public EqualCriterion ChildLastName { get; set; }

            public EqualCriterion ChildAge { get; set; }
        }
    }
}
