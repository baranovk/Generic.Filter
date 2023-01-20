namespace Generic.Filter.Criteria
{
    internal class FilteringExpressionGeneratorFactory
    {
        private static Dictionary<Type, IFilteringExpressionGenerator> _generators = new()
        {
            { typeof(EqualCriterion), new EqualFilteringExpressionGenerator() }
        };

        public static IFilteringExpressionGenerator GetFilteringExpressionGenerator(Type filterCriterionType)
        {
            // TODO: add descriptive exception message
            if (!_generators.ContainsKey(filterCriterionType))
                throw new Exception();

            return _generators[filterCriterionType];
        }
    }
}
