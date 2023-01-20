using System.Linq.Expressions;
using System.Reflection;
using Generic.Filter.Criteria;
using Generic.Filter.Expressions;

namespace Generic.Filter
{
    public abstract class GenericFilter<TItem, TFilter> where TFilter : GenericFilter<TItem, TFilter>
    {
        //private Dictionary<string, PropertyInfo> _propertyMap = new();

        private static Lazy<IEnumerable<Func<TFilter, Expression<Func<TItem, bool>>>>> _filteringExpressions 
            = new(() => GetFilteringExpressionsByProperty().Select(_ => _.Compile()));

        public virtual Expression<Func<TItem, bool>> ToExpression()
        {
            var filteringPredicateExpressions = _filteringExpressions.Value.Select(expr => expr((TFilter)this));

            if (!filteringPredicateExpressions.Any())
                throw GenericFilterException.Build(Resources.ErrorFilterDoesNotContainAnyCriteria, typeof(TFilter).Name);

            return filteringPredicateExpressions
                .Skip(1)
                .Aggregate(filteringPredicateExpressions.ElementAt(0), (expr, acc) => acc.And(expr));
        }

        public static implicit operator Expression<Func<TItem, bool>>(GenericFilter<TItem, TFilter> filter)
        {
            return filter.ToExpression();
        }

        private static IEnumerable<Expression<Func<TFilter, Expression<Func<TItem, bool>>>>> GetFilteringExpressionsByProperty()
        {
            var filterType = typeof(TFilter);
            var itemParameterExpr = Expression.Parameter(typeof(TItem), "item");
            var filterParameterExpr = Expression.Parameter(filterType, "filter");

            return filterType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IFilterCriteria).IsAssignableFrom(p.PropertyType))
                .Aggregate(new List<Expression<Func<TFilter, Expression<Func<TItem, bool>>>>>(),
                    (expressions, filterPropertyInfo) => {
                        // if (dictionary[filterPropertyInfgo.Name] != null)
                        //var itemPropertyExpr = _propertyMap.ContainsKey(filterPropertyInfo.Name)
                        //    ? Expression.Property(itemParameterExpr, filterPropertyInfo.Name)
                        //    : Expression.Property(itemParameterExpr, filterPropertyInfo.Name);

                        var itemPropertyExpr = Expression.Property(itemParameterExpr, filterPropertyInfo.Name);
                        var filterPropertyExpr = Expression.Property(filterParameterExpr, filterPropertyInfo);

                        var filteringExpressionGenerator = FilteringExpressionGeneratorFactory.GetFilteringExpressionGenerator(filterPropertyInfo.PropertyType);
                        var filteringExpression = filteringExpressionGenerator.BuildFilteringExpression(itemPropertyExpr, filterPropertyExpr);

                        var lambda = Expression.Lambda<Func<TFilter, Expression<Func<TItem, bool>>>>(
                            Expression.Lambda<Func<TItem, bool>>(filteringExpression, itemParameterExpr),
                            filterParameterExpr
                        );

                        expressions.Add(lambda);
                        return expressions;
                    });
        }
    }
}
