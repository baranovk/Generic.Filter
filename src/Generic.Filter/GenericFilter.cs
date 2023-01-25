using System.Linq.Expressions;
using System.Reflection;
using Generic.Filter.Criteria;
using Generic.Filter.Expressions;

namespace Generic.Filter
{
    public abstract class GenericFilter<TItem, TFilter> where TFilter : GenericFilter<TItem, TFilter>
    {
        private Dictionary<string, PropertyInfo> _propertyMap = new();

        private FilterMapping<TItem, TFilter>? _mappings;

        protected readonly Lazy<Expression<Func<TItem, bool>>> _filteringExpression;

        public GenericFilter()
        {
            var filter = this;
            _filteringExpression = new Lazy<Expression<Func<TItem, bool>>>(() => BuildFilteringExprression(filter));
        }

        public GenericFilter(FilterMapping<TItem, TFilter> mappings) : this()
        {
            _mappings = mappings;
        }

        protected virtual Expression<Func<TItem, bool>> ToExpression() => _filteringExpression.Value;

        public static implicit operator Expression<Func<TItem, bool>>(GenericFilter<TItem, TFilter> filter)
        {
            return filter.ToExpression();
        }

        private Expression<Func<TItem, bool>> BuildFilteringExprression(GenericFilter<TItem, TFilter> filter)
        {
            var filteringPredicateExpressions = GetFilteringExpressionsByProperty(filter);

            if (!filteringPredicateExpressions.Any())
                throw GenericFilterException.Build(Resources.ErrorFilterDoesNotContainAnyCriteria, typeof(TFilter).Name);

            return filteringPredicateExpressions
                .Skip(1)
                .Aggregate(filteringPredicateExpressions.ElementAt(0), (expr, acc) => acc.And(expr));
        }

        private IEnumerable<Expression<Func<TItem, bool>>> GetFilteringExpressionsByProperty(GenericFilter<TItem, TFilter> filter)
        {
            var filterType = typeof(TFilter);
            var itemParameterExpr = Expression.Parameter(typeof(TItem), "item");

            return filterType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IFilterCriteria).IsAssignableFrom(p.PropertyType))
                .Aggregate(new List<Expression<Func<TItem, bool>>>(),
                    (expressions, filterPropertyInfo) => {
                        var itemPropertyExpr = _propertyMap.ContainsKey(filterPropertyInfo.Name)
                            ? Expression.Property(itemParameterExpr, _propertyMap[filterPropertyInfo.Name])
                            : Expression.Property(itemParameterExpr, filterPropertyInfo.Name);

                        var filterPropertyExpr = Expression.Property(Expression.Constant(filter), filterPropertyInfo);

                        var filteringExpressionGenerator = FilteringExpressionGeneratorFactory.GetFilteringExpressionGenerator(filterPropertyInfo.PropertyType);
                        var filteringExpression = filteringExpressionGenerator.BuildFilteringExpression(itemPropertyExpr, filterPropertyExpr);

                        var lambda = Expression.Lambda<Func<TItem, bool>>(filteringExpression, itemParameterExpr);
                        expressions.Add(lambda);
                        return expressions;
                    });
        }
    }
}
