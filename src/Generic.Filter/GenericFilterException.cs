namespace Generic.Filter
{
    public class GenericFilterException : Exception
    {
        public GenericFilterException() { }

        public GenericFilterException(string message) : base(message) {  }

        public static GenericFilterException Build(string message, params object[] args)
        {
            return args.Any() 
                ? new GenericFilterException(string.Format(message, args)) 
                : new GenericFilterException(message);
        }
    }
}
