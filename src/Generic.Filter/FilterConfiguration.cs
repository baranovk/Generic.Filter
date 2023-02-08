using System.Linq.Expressions;
using System.Reflection;
using Generic.Filter.Extensions;
using Generic.Filter.Supplementary;

namespace Generic.Filter
{
    public class FilterMappings<TItem, TFilter>
    {
        private Dictionary<string, IFilterPropertyMapping> _mappings = new();

        public FilterMappings<TItem, TFilter> ForMember<TItemMember, TFilterMember>(
            Expression<Func<TItem, TItemMember>> itemMemberExpr, 
            Expression<Func<TFilter, TFilterMember>> filterMemberExpr)
        {
            var itemMemberInfo = ReflectionHelper.FindProperty(itemMemberExpr);
            var filterMemberInfo = ReflectionHelper.FindProperty(filterMemberExpr);

            if (!itemMemberInfo.IsProperty())
                throw new ArgumentException(
                    string.Format(Resources.ErrorUnsupportedMemberTypeForMapping, itemMemberExpr),
                    nameof(itemMemberExpr));

            if (!filterMemberInfo.IsProperty())
                throw new ArgumentException(
                    string.Format(Resources.ErrorUnsupportedMemberTypeForMapping, filterMemberExpr),
                nameof(filterMemberExpr));

            _mappings.Add(filterMemberInfo.Name, new PropertyMapping((PropertyInfo)itemMemberInfo));
            return this;
        }

        public FilterMappings<TItem, TFilter> ForPath<TItemMember, TFilterMember>(
            Expression<Func<TItem, TItemMember>> itemMemberExpr,
            Expression<Func<TFilter, TFilterMember>> filterMemberExpr)
        {
            var filterMemberInfo = ReflectionHelper.FindProperty(filterMemberExpr);
            _mappings.Add(filterMemberInfo.Name, new PathPropertyMapping(itemMemberExpr));
            return this;
        }

        public IFilterPropertyMapping? this[string filterPropertyName]
        {
            get
            {
                return _mappings.ContainsKey(filterPropertyName) ? _mappings[filterPropertyName] : null;
            }
        }
    }

    public interface IFilterPropertyMapping
    {
        MemberExpression MapFor(ParameterExpression item);
    }

    public class PropertyMapping : IFilterPropertyMapping
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyMapping(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public MemberExpression MapFor(ParameterExpression item)
        {
            return Expression.Property(item, _propertyInfo);
        }
    }

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

            foreach (var memberPropertyInfo in _memberExpressions)
            {
                expr = Expression.Property(expr, memberPropertyInfo);
                //expr = Expression.MakeMemberAccess(expr, memberExpr.Member);
            }

            return (MemberExpression)expr;
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
