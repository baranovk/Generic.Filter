using Microsoft.EntityFrameworkCore;

namespace Generic.Filter.UnitTests.DataBase
{
    internal class PostgresAirDbContext : DbContext
    {
        private readonly string _connectionString;

        public PostgresAirDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Airport> Airport { get; set; }

        public DbSet<Flight> Flight { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options
                .UseNpgsql(_connectionString)
                .UseSnakeCaseNamingConvention();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("postgres_air");
            modelBuilder.Entity<Airport>().HasKey(a => a.AirportCode);

            modelBuilder.Entity<Aircraft>()
                .HasKey(a => a.Code);

            modelBuilder.Entity<Aircraft>()
                .HasOne(a => a.Flight)
                .WithOne(f => f.Aircraft)
                .HasForeignKey<Flight>(f => f.AircraftCode);

            base.OnModelCreating(modelBuilder);
        }
    }
}
