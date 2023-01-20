using System.Linq.Expressions;
using Generic.Filter.Criteria;

namespace Generic.Filter.UnitTests.Criteria
{
    public class ConvertionTests
    {
        [Test]
        public void EqualCriterion_ShouldSupportConvertionToInt32()
        {
            Expression<Func<EqualCriterion, string, bool>> ex = (i, s) => i == s;

            var criterion = new EqualCriterion(10);
            Assert.That((int)criterion, Is.EqualTo(10));
        }
    }
}
