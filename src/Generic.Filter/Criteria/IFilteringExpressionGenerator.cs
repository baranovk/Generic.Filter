using System.Linq.Expressions;

namespace Generic.Filter.Criteria
{
    internal interface IFilteringExpressionGenerator
    {
        Expression BuildFilteringExpression(Expression itemValueExpression, MemberExpression filterPropertyExpression);
    }
}
