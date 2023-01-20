using System.ComponentModel.DataAnnotations.Schema;

namespace Generic.Filter.UnitTests.DataBase
{
    internal class Airport
    {
        public string AirportCode { get; set; }

        public string AirportName { get; set; }

        public string City { get; set; }
    }
}
