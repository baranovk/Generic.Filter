using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Filter.Supplementary
{
    public static class ReflectionHelper
    {
        public static MemberInfo FindProperty(LambdaExpression lambdaExpression)
        {
            Expression expressionToCheck = lambdaExpression.Body;

            while (true)
            {
                switch (expressionToCheck)
                {
                    case MemberExpression { Member: var member, Expression.NodeType: ExpressionType.Parameter or ExpressionType.Convert }:
                        return member;
                    case UnaryExpression { Operand: var operand }:
                        expressionToCheck = operand;
                        break;
                    default:
                        throw new ArgumentException(
                            string.Format(Resources.ErrorInvalidPropertyExpression, lambdaExpression),
                                nameof(lambdaExpression));
                }
            }
        }
    }
}
