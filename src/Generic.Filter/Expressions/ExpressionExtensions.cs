using System.Linq.Expressions;

namespace Generic.Filter.Expressions
{
    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> left,
               Expression<Func<T, bool>> right)
        {
            if (left == null && right == null)
                throw new ArgumentNullException($"{nameof(left)} & {nameof(right)}");

            if (right == null && left != null)
                return left;

            if (left == null && right != null)
                return right;

            var secondBody = right!.Body.Replace(right.Parameters[0], left!.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>
                  (Expression.AndAlso(left.Body, secondBody!), left.Parameters);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> left,
               Expression<Func<T, bool>> right)
        {
            if (left == null && right == null)
                throw new ArgumentNullException($"{nameof(left)} & {nameof(right)}");

            if (right == null && left != null)
                return left;

            if (left == null && right != null)
                return right;

            var secondBody = right!.Body.Replace(right.Parameters[0], left!.Parameters[0]);

            return Expression.Lambda<Func<T, bool>>
                  (Expression.OrElse(left.Body, secondBody!), left.Parameters);
        }

        public static Expression? Replace(this Expression expression, Expression searchEx,
               Expression replaceEx)
        {
            return new ReplaceVisitor(searchEx, replaceEx).Visit(expression);
        }
    }
}
