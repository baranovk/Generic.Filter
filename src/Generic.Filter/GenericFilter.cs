﻿using Generic.Filter.Criteria;
using Generic.Filter.Expressions;
using Generic.Filter.Mappings;
using System.Linq.Expressions;
using System.Reflection;

namespace Generic.Filter
{
    public abstract class GenericFilter<TItem, TFilter> where TFilter : GenericFilter<TItem, TFilter>
    {
        #region Fields

        private readonly FilterMappings<TItem, TFilter>? _propertyMappings;

        #endregion

        #region Constructors

        public GenericFilter()
        {
        }

        public GenericFilter(FilterMappings<TItem, TFilter> propertyMappings) : this()
        {
            _propertyMappings = propertyMappings;
        }

        #endregion

        #region Properties

        protected virtual Expression<Func<TItem, bool>> ToExpression() => BuildFilteringExprression(this);

        #endregion

        #region Private Methods

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
            var itemParameterExpr = Expression.Parameter(typeof(TItem), "p");

            return filterType
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(p => typeof(IFilterCriteria).IsAssignableFrom(p.PropertyType))
                .Aggregate(new List<Expression<Func<TItem, bool>>>(),
                    (expressions, filterPropertyInfo) =>
                    {
                        if (filterPropertyInfo.GetValue(filter) is null) return expressions;

                        Expression itemValueExpr = null != _propertyMappings?[filterPropertyInfo.Name]
                            ? _propertyMappings[filterPropertyInfo.Name]!.MapFor(itemParameterExpr)
                            : Expression.Property(itemParameterExpr, filterPropertyInfo.Name);

                        var filterPropertyExpr = Expression.Property(Expression.Constant(filter), filterPropertyInfo);

                        var filteringExpressionGenerator = FilteringExpressionGeneratorFactory.GetFilteringExpressionGenerator(filterPropertyInfo.PropertyType);
                        var filteringExpression = filteringExpressionGenerator.BuildFilteringExpression(itemValueExpr, filterPropertyExpr);

                        var lambda = Expression.Lambda<Func<TItem, bool>>(filteringExpression, itemParameterExpr);
                        expressions.Add(lambda);
                        return expressions;
                    });
        }

        #endregion

        #region Operators

        public static implicit operator Expression<Func<TItem, bool>>(GenericFilter<TItem, TFilter> filter)
        {
            return filter.ToExpression();
        }

        #endregion
    }
}
