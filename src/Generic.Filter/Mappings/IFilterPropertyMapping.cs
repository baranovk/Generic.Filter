using System.Linq.Expressions;

namespace Generic.Filter.Mappings
{
    public interface IFilterPropertyMapping
    {
        MemberExpression MapFor(ParameterExpression item);
    }
}
