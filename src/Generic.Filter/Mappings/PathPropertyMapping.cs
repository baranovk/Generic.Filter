using Generic.Filter.Extensions;
using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Filter.Mappings
{
    public class PathPropertyMapping : IFilterPropertyMapping
    {
        /*
            p => p.Child.Property

            Lambda
	            |-- Body[MemberExpression]: NodeType = MemberAccess, Member[MemberInfo] = Property
			            |-- Expression[MemberExpression]: NodeType = MemberAccess, Member[MemberInfo] = Child
				            |-- Expression[TypedParameterExpression]: NodeType = Parameter, Member[MemberInfo] = Child
        */

        private readonly Stack<PropertyInfo> _memberExpressions = new();

        public PathPropertyMapping(LambdaExpression itemMemberExpr)
        {
            Parse(itemMemberExpr);
        }

        public MemberExpression MapFor(ParameterExpression item)
        {
            Expression expr = item;
            return (MemberExpression)_memberExpressions.Aggregate(expr, (acc, prop) => Expression.Property(acc, prop));
        }

        private void Parse(LambdaExpression itemMemberExpr)
        {
            // TODO: check if itemMemberExpr.Body is MemberExpression
            //MemberExpression expressionToCheck = ((itemMemberExpr.Body as MemberExpression)!.Expression as MemberExpression)!;
            Expression? expressionToCheck = (itemMemberExpr.Body as MemberExpression)!;

            while (true)
            {
                switch (expressionToCheck)
                {
                    case MemberExpression { Member: var member, NodeType: ExpressionType.MemberAccess }:
                        if (!member.IsProperty())
                            throw GenericFilterException.Build(Resources.ErrorInvalidMemberType,
                                member.Name, MemberTypes.Property, member.MemberType);
                        _memberExpressions.Push((PropertyInfo)member);
                        expressionToCheck = (expressionToCheck as MemberExpression)!.Expression;
                        break;
                    case Expression { NodeType: ExpressionType.Parameter }:
                        return;
                    default:
                        throw new ArgumentException(
                            string.Format(Resources.ErrorInvalidPropertyPathExpression, itemMemberExpr),
                                nameof(itemMemberExpr));
                }
            }
        }
    }
}
