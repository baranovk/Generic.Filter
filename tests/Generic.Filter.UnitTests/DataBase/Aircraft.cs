namespace Generic.Filter.UnitTests.DataBase
{
    public class Aircraft
    {
        public string Code { get; set; }

        public string Model { get; set; }

        public virtual Flight Flight { get; set; }
    }
}
