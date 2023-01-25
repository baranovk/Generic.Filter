using System.Linq.Expressions;
using System.Reflection;
using Generic.Filter.Extensions;
using Generic.Filter.Supplementary;

namespace Generic.Filter
{
    public class FilterConfiguration
    {
        public FilterConfiguration(Action<object> config)
        {
            
        }
    }

    public class FilterMapping<TItem, TFilter>
    {
        private Dictionary<string, PropertyInfo> _mappings = new();

        public FilterMapping<TItem, TFilter> ForMember<TItemMember, TFilterMember>(
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

            _mappings.Add(filterMemberInfo.Name, (PropertyInfo)itemMemberInfo);
            return this;
        }

        public PropertyInfo? this[string filterPropertyName]
        {
            get
            {
                return _mappings.ContainsKey(filterPropertyName) ? _mappings[filterPropertyName] : null;
            }
        }
    }
}
