using Generic.Filter.Extensions;
using System.Diagnostics.Metrics;
using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Filter.Mappings
{
    public class PathPropertyMapping<TItem, TItemMember> : IFilterPropertyMapping
    {
        /*
            p => p.Child.Property

            Lambda
	            |-- Body[MemberExpression]: NodeType = MemberAccess, Member[MemberInfo] = Property
			            |-- Expression[MemberExpression]: NodeType = MemberAccess, Member[MemberInfo] = Child
				            |-- Expression[TypedParameterExpression]: NodeType = Parameter, Member[MemberInfo] = Child
        */
        private string? _parameterName;
        private readonly Stack<MemberExpression> _memberExpressions = new();

        public PathPropertyMapping(LambdaExpression itemMemberExpr)
        {
            Parse(itemMemberExpr);
        }

        public Expression MapFor(ParameterExpression itemParameterExpr)
        {
            /*
             * p.Child.Property
             * 
             * return null == p.Child ? null : p.Child
             * 
             */
            Expression expr = itemParameterExpr;
            //return (MemberExpression)_memberExpressions.Aggregate(expr, (acc, prop) => Expression.Property(acc, prop));

            foreach (var memberExpr in _memberExpressions)
            {
                expr = memberExpr.Member.GetUnderlyingType().IsClass
                    ? Expression.Condition(Expression.ReferenceEqual(memberExpr, Expression.Constant(null)), 
                        Expression.Constant(null, memberExpr.Member.GetUnderlyingType()), 
                            Expression.Property(expr, (PropertyInfo)memberExpr.Member))
                    : Expression.Property(expr, (PropertyInfo)memberExpr.Member);
            }

            //return Expression.Invoke(Expression.Constant(Expression.Lambda<Func<TItem, TItemMember>>(expr, Expression.Parameter(typeof(TItem), _parameterName))), Expression.Parameter(typeof(TItem), _parameterName));
            return Expression.Invoke(Expression.Lambda<Func<TItem, TItemMember>>(expr, Expression.Parameter(typeof(TItem), _parameterName)), Expression.Parameter(typeof(TItem), _parameterName));
        }

        private void Parse(LambdaExpression itemMemberExpr)
        {
            Expression? expressionToCheck = (itemMemberExpr.Body as MemberExpression);

            if(expressionToCheck is null)
                throw new ArgumentException(
                    string.Format(Resources.ErrorUnsupportedLambdaExpressionBodyForPathPropertyMapping, 
                        itemMemberExpr.Body, itemMemberExpr.Body.NodeType),
                    nameof(itemMemberExpr)
                );

            while (true)
            {
                switch (expressionToCheck)
                {
                    case MemberExpression { Member: var member, NodeType: ExpressionType.MemberAccess }:
                        if (!member.IsProperty())
                            throw GenericFilterException.Build(Resources.ErrorInvalidMemberType,
                                member.Name, MemberTypes.Property, member.MemberType
                            );
                        //_memberExpressions.Push((PropertyInfo)member);
                        _memberExpressions.Push((expressionToCheck as MemberExpression)!);
                        expressionToCheck = (expressionToCheck as MemberExpression)!.Expression;
                        break;
                    case ParameterExpression { NodeType: ExpressionType.Parameter, Name : var name }:
                        _parameterName = name;
                        return;
                    default:
                        throw new ArgumentException(
                            string.Format(Resources.ErrorInvalidPropertyPathExpression, itemMemberExpr),
                            nameof(itemMemberExpr)
                        );
                }
            }
        }
    }
}
