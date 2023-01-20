using System.Linq.Expressions;

namespace Generic.Filter.Criteria
{
    internal interface IFilteringExpressionGenerator
    {
        Expression BuildFilteringExpression(MemberExpression itemPropertyExpression, MemberExpression filterPropertyExpression);
    }
}
