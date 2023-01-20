namespace Generic.Filter.UnitTests.DataBase
{
    public class Flight
    {
        public int FlightId { get; set; }

        public string FlightNo { get; set; }

        public string AircraftCode { get; set; }

        public virtual Aircraft Aircraft { get; set; }
    }
}
