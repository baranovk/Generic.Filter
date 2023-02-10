using System.Linq.Expressions;
using Generic.Filter.Extensions;

namespace Generic.Filter.Criteria
{
    internal class EqualFilteringExpressionGenerator : IFilteringExpressionGenerator
    {
        public Expression BuildFilteringExpression(MemberExpression itemPropertyExpression, MemberExpression filterPropertyExpression)
        {
            var itemPropertyType = itemPropertyExpression.Member.GetUnderlyingType();
            var filterPropertyType = filterPropertyExpression.Member.GetUnderlyingType();

            var equalExpr = itemPropertyType == filterPropertyType
                ? Expression.Equal(itemPropertyExpression, filterPropertyExpression)
                : Expression.Equal(Expression.Convert(filterPropertyExpression, itemPropertyType!), itemPropertyExpression);

            var isNullPropertyExpr = Expression.Equal(filterPropertyExpression, Expression.Constant(null));

            return Expression.OrElse(isNullPropertyExpr, equalExpr);
        }
    }
}
