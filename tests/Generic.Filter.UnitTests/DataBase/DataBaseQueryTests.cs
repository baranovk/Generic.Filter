using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Generic.Filter.Criteria;
using Microsoft.EntityFrameworkCore;

namespace Generic.Filter.UnitTests.DataBase
{
    internal class DataBaseQueryTests
    {
        private PostgresAirDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var connectionString = Environment.GetEnvironmentVariable("GENERIC_FILTER_PostgresAirDbContext_ConnectionString");
            _dbContext = new PostgresAirDbContext(connectionString!);
        }

        [Test]
        [Explicit]
        public void GenericFilter_ShouldFilterAirportsByCode()
        {
            var filter = new AirportFilter { AirportCode = "HND" };
            var filteredAirports = _dbContext.Airport.Where(filter).ToList();

            Assert.That(filteredAirports, Has.Count.EqualTo(1));
            Assert.That(filteredAirports.First().AirportCode, Is.EqualTo("HND"));
            Assert.That(filteredAirports.First().AirportName, Is.EqualTo("Tokyo Haneda International Airport"));
        }

        [Test]
        [Explicit]
        public void GenericFilter_ShouldFilterAirportsByCodeAndName()
        {
            var filter = new AirportFilter { AirportCode = "HND", AirportName = "Tokyo Haneda International Airport" };
            var filteredAirports = _dbContext.Airport.Where(filter).ToList();

            Assert.That(filteredAirports, Has.Count.EqualTo(1));
            Assert.That(filteredAirports.First().AirportCode, Is.EqualTo("HND"));
            Assert.That(filteredAirports.First().AirportName, Is.EqualTo("Tokyo Haneda International Airport"));
        }

        [Test]
        [Explicit]
        public void GenericFilter_ShouldFilterFlightsByNumberAndAircraftCode()
        {
            var t = _dbContext.Flight.Include(f => f.Aircraft).Where(f => "AAA" == (null != f.Aircraft ? f.Aircraft.Code : null)).ToList();
            var filter = new AirportFilter { AirportCode = "HND" };
            var filteredAirports = _dbContext.Flight.Include(f => f.Aircraft).Take(2).ToList();

            Assert.That(filteredAirports, Has.Count.EqualTo(2));
            //Assert.That(filteredAirports.First().AirportCode, Is.EqualTo("HND"));
            //Assert.That(filteredAirports.First().AirportName, Is.EqualTo("Tokyo Haneda International Airport"));
        }

        private class AirportFilter : GenericFilter<Airport, AirportFilter>
        {
            public EqualCriterion AirportCode { get; set; }

            public EqualCriterion AirportName { get; set; }

            public EqualCriterion City { get; set; }
        }
    }
}
