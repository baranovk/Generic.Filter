using Generic.Filter.Extensions;
using Generic.Filter.Supplementary;
using System.Linq.Expressions;

namespace Generic.Filter.Mappings
{
    public class FilterMappings<TItem, TFilter>
    {
        private readonly Dictionary<string, IFilterPropertyMapping> _mappings = new();

        public FilterMappings<TItem, TFilter> ForMember<TItemMember, TFilterMember>(
            Expression<Func<TItem, TItemMember>> itemMemberExpr,
            Expression<Func<TFilter, TFilterMember>> filterMemberExpr)
        {
            var filterMemberInfo = ReflectionHelper.FindProperty(filterMemberExpr);

            if (!filterMemberInfo.IsProperty())
                throw new ArgumentException(
                    string.Format(Resources.ErrorUnsupportedMemberTypeForMapping, filterMemberExpr),
                nameof(filterMemberExpr));

            _mappings.Add(filterMemberInfo.Name, new PathPropertyMapping<TItem, TItemMember>(itemMemberExpr));
            return this;
        }

        public FilterMappings<TItem, TFilter> ForPath<TItemMember, TFilterMember>(
            Expression<Func<TItem, TItemMember>> itemMemberExpr,
            Expression<Func<TFilter, TFilterMember>> filterMemberExpr)
        {
            var filterMemberInfo = ReflectionHelper.FindProperty(filterMemberExpr);
            _mappings.Add(filterMemberInfo.Name, new PathPropertyMapping<TItem, TItemMember>(itemMemberExpr));
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
}
