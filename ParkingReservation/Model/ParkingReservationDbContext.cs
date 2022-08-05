using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ParkingReservation.Model
{
    /// <summary>
    /// Parking reservation db context.
    /// </summary>
    public class ParkingReservationDbContext : DbContext
    {
        private const string ConnectionStringName = "ParkingReservationDatabase";
        private readonly string connectionString;
        private readonly ILogger<ParkingReservationDbContext> log;
        private readonly ILoggerFactory loggerFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParkingReservationDbContext"/> class.
        /// </summary>
        /// <param name="configuration">Configuration.</param>
        /// <param name="loggerFactory">Logger factory.</param>
        public ParkingReservationDbContext(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            this.log = loggerFactory.CreateLogger<ParkingReservationDbContext>();
            this.connectionString = configuration.GetConnectionString(ConnectionStringName);
            this.loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Gets or sets the reservation db set.
        /// </summary>
        public DbSet<Reservation> Reservations { get; set; } = null!;

        /// <summary>
        /// Gets ot sets the spaces db set.
        /// </summary>
        public DbSet<Space> Spaces { get; set; } = null!;

        /// <summary>
        /// Override to customise database configuration.
        /// </summary>
        /// <param name="optionsBuilder">EF core options builder.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlServer(this.connectionString)
                .UseLoggerFactory(this.loggerFactory);
        }

        /// <summary>
        /// Configures the model.
        /// </summary>
        /// <param name="modelBuilder">The model builder.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            this.log.LogTrace("OnModelCreating Start");

            base.OnModelCreating(modelBuilder);

            this.ConfigureReservation(modelBuilder.Entity<Reservation>());
        }

        /// <summary>
        /// Configure reservation.
        /// </summary>
        /// <param name="link"></param>
        private void ConfigureReservation(EntityTypeBuilder<Reservation> link)
        {
            link.HasOne(r => r.Space)
                .WithMany(s => s.Reservations)
                .HasForeignKey(r => r.SpaceId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}