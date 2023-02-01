namespace Generic.Filter.Criteria
{
    public class Criterion : IFilterCriteria
    {
        #region Fields

        protected readonly object? _value;

        #endregion

        #region Constructors

        public Criterion(object value)
        {
            _value = value;
        }

        #endregion

        #region Properties

        public bool IsNull => _value == null;

        #endregion

        #region Public Methods

        public override string? ToString()
        {
            return _value?.ToString();
        }

        #endregion
    }
}
