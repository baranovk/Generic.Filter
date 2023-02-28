using System.Linq.Expressions;
using Generic.Filter.Extensions;

namespace Generic.Filter.Criteria
{
    internal class EqualFilteringExpressionGenerator : IFilteringExpressionGenerator
    {
        public Expression BuildFilteringExpression(Expression itemValueExpression, MemberExpression filterPropertyExpression)
        {
            var filterPropertyType = filterPropertyExpression.Member.GetUnderlyingType()!;
            var equalExpr = Expression.Equal(itemValueExpression, filterPropertyExpression);
                
            /*
             * This solution does not check for null it invokes operator ==! This might or might not be the same. 
             * If you really want to check for null you must use Expression.ReferenceEqual rather than 'Expression.Equal`
             */
            var isNullPropertyExpr = Expression.ReferenceEqual(filterPropertyExpression, Expression.Constant(null, filterPropertyType));

            return Expression.OrElse(isNullPropertyExpr, equalExpr);
        }
    }
}
