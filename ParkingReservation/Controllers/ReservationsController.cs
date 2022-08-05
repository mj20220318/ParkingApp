using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ParkingReservation.Dtos;
using ParkingReservation.Model;

namespace ParkingReservation.Controllers
{
    /// <summary>
    /// Reservation controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly ParkingReservationDbContext context;
        private readonly ILogger<ReservationsController> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReservationsController"/> class.
        /// </summary>
        /// <param name="log">Logger.</param>
        /// <param name="context">Db context.</param>
        public ReservationsController(ILogger<ReservationsController> log, ParkingReservationDbContext context)
        {
            this.log = log;
            this.context = context;
        }

        /// <summary>
        /// Amend a reservation.
        /// </summary>
        /// <param name="reservationId">Reservation id.</param>
        /// <param name="dtReservation">Reservation dto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPut]
        [Route("/api/reservations/{reservationId}")]
        public async Task<IActionResult> AmendReservationAsync([FromRoute][Required] int reservationId, [FromBody] DtReservation dtReservation)
        {
            this.log.LogTrace("AmendReservationAsync start");
            if (reservationId <= 0)
            {
                string message = "Invalid reservation id";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            // Find original reservation
            Reservation reservation = await this.context.Reservations.Where(r => r.Id == reservationId).Include(r => r.Space).ThenInclude(s => s.Reservations).FirstAsync();
            if (null == reservation)
            {
                this.log.LogDebug("Original reservation not found");
                return this.NotFound();
            }

            // Update details
            try
            {
                this.log.LogTrace("Save amended reservation");
                await this.UpdateOriginalReservationAsync(reservation, dtReservation);
            }
            catch (InvalidOperationException exception)
            {
                return this.BadRequest(exception.Message);

            }

            this.context.Entry(reservation).State = EntityState.Modified;

            try
            {
                this.log.LogTrace("Save amended reservation");
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReservationExists(reservationId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (DbUpdateException exp)
            {
                string message = $"Failed to amend reservation ({exp.Message})";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            DtReservation dto = this.PackDto(reservation);

            this.log.LogTrace("AmendReservationAsync done");

            return this.Ok(dto);
        }

        /// <summary>
        /// Create the reservation.
        /// </summary>
        /// <param name="dtReservation">Reservation dto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        [Route("/api/reservations")]
        public async Task<ActionResult<Reservation>> CreateReservationAsync([FromBody] DtReservation dtReservation)
        {
            this.log.LogTrace("CreateReservationAsync start");

            // Make sure we can take the reservation
            List<Space> availableSpaces = await this.GetAvailableSpacesAsync(dtReservation.From, dtReservation.To);
            if (!availableSpaces.Any())
            {
                string message = $"No spaces available";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            // If we get here, then the reservation is valid
            // Create the reservation
            Reservation reservation = new Reservation
            {
                From = dtReservation.From,
                To = dtReservation.To,
            };

            this.log.LogTrace("Allocating space to new reservation");

            this.AllocateParkingSpace(reservation, availableSpaces.First());

            this.log.LogTrace("Add new reservation to context");

            this.context.Reservations.Add(reservation);

            try
            {
                this.log.LogTrace("Save new reservation");
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateException exp)
            {
                string message = $"Failed to create reservation ({exp.Message})";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            DtReservation dto = this.PackDto(reservation);

            this.log.LogTrace("CreateReservation done");

            return this.Ok(dto);
        }

        /// <summary>
        /// Convert Reservation object to Reservation DTO.
        /// </summary>
        /// <param name="reservation">Reservation.</param>
        /// <returns>Reservation dto.</returns>
        private DtReservation PackDto(Reservation reservation)
        {
            this.log.LogTrace("PackDto start");
            return new DtReservation
            {
                From = reservation.From,
                To = reservation.To,
                //Price = reservation.Price,
                Id = reservation.Id,
                SpaceId = reservation.SpaceId
            };
        }

        /// <summary>
        /// Delete the reservation.
        /// </summary>
        /// <param name="reservationId">Reservation id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpDelete]
        [Route("/api/reservations/{reservationId}")]
        public async Task<IActionResult> DeleteReservationAsync([FromRoute][Required] int reservationId)
        {
            this.log.LogTrace("DeleteReservationAsync start");

            Reservation reservation = await this.context.Reservations.FindAsync(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }

            this.context.Reservations.Remove(reservation);

            try
            {
                this.log.LogTrace("Save deleted reservation");
                await this.context.SaveChangesAsync();
            }
            catch (DbUpdateException exp)
            {
                string message = $"Failed to delete reservation ({exp.Message})";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            this.log.LogTrace("DeleteReservationAsync done");

            return this.Ok();
        }

        /// <summary>
        /// Get the reservation by id.
        /// </summary>
        /// <param name="reservationId">Reservation id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/reservations/{reservationId}")]
        public async Task<ActionResult<Reservation>> GetReservationAsync([FromRoute][Required] int reservationId)
        {
            this.log.LogTrace("GetReservationAsync start {0}", reservationId);

            if (reservationId <= 0)
            {
                throw new ArgumentNullException(nameof(reservationId));
            }

            Reservation reservation = await this.context.Reservations.Include(r => r.Space).FirstOrDefaultAsync(r => r.Id == reservationId);

            if (reservation == null)
            {
                return NotFound();
            }

            this.log.LogTrace("GetReservatiosAsync done {0}", reservationId);

            return reservation;
        }

        /// <summary>
        /// Get reservation price.
        /// </summary>
        /// <param name="from">The start date time for the reservation.</param>
        /// <param name="to">The end date time for the reservation.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/reservations-price")]
        public decimal GetReservationPrice([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            this.log.LogTrace("GetReservationPrice start");

            decimal priceExcVat = this.CalculateReservationPrice(from, to);

            this.log.LogTrace("GetReservationPrice done ");

            return priceExcVat;
        }

        /// <summary>
        /// Get the reservations.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/reservations")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservationsAsync()
        {
            this.log.LogTrace("GetReservationsAsync start");
            return await this.context.Reservations.Include(r => r.Space).ToListAsync();
        }

        /// <summary>
        /// Allocate a parking space to the reservation.
        /// </summary>
        /// <param name="reservation">Reservation.</param>
        /// <param name="spaceToAllocate">Space to allocate to the reservation.</param>
        private void AllocateParkingSpace(Reservation reservation, Space spaceToAllocate)
        {
            this.log.LogTrace("AllocateParkingSpace start");
            reservation.Space = spaceToAllocate;
            reservation.SpaceId = spaceToAllocate.Id;
            reservation.Created = DateTime.Now;
            this.log.LogTrace("AllocateParkingSpace done");
        }

        /// <summary>
        /// Calculate the price of the reservation.
        /// </summary>
        /// <param name="from">Date and time the reservation is from</param>
        /// <param name="to">Date and time the reservation is to</param>
        /// <returns>Price for the reservation.</returns>
        private decimal CalculateReservationPrice(DateTime from, DateTime to)
        {
            this.log.LogTrace("CalculateReservationPrice start");

            decimal price;
            // Simple calculation to check if the reservation is summer/winter booking
            if (from.Month <= 9 && from.Month >= 3)
            {
                // Summer booking
                // decimal price = this.SummerPricingModel.CalculatePrice(reservation);
                price = 200;
            }
            else
            {
                // Winter booking
                // decimal price = this.WinterrPricingModel.CalculatePrice(reservation);
                price = 100;
            }

            this.log.LogTrace("CalculateReservationPrice done");
            return price;
        }

        /// <summary>
        /// Get available parking space for the reservation.
        /// </summary>
        /// <param name="from">The start date time for the reservation.</param>
        /// <param name="to">The end date time for the reservation.</param>
        private async Task<List<Space>> GetAvailableSpacesAsync(DateTime from, DateTime to)
        {
            this.log.LogTrace("GetAvailableSpacesAsync start");

            // We want to get the parking spaces available
            return await this.context.Spaces.Include(s => s.Reservations)
                .Where(space => space.Reservations.All(res => res.To < from && res.From > to))
                .ToListAsync();
        }

        /// <summary>
        /// Check if the reservation already exists.
        /// </summary>
        /// <param name="reservationId">Reservation id.</param>
        /// <returns>True, if the reservation already exists.</returns>
        private bool ReservationExists(int reservationId)
        {
            this.log.LogTrace("ReservationExists start");
            return this.context.Reservations.Any(e => e.Id == reservationId);
        }

        /// <summary>
        /// Update the original reservation.
        /// </summary>
        /// <param name="originalReservation">Original reservation.</param>
        /// <param name="dtReservation">Reservation dto.</param>
        private async Task UpdateOriginalReservationAsync(Reservation originalReservation, DtReservation dtReservation)
        {
            this.log.LogTrace("UpdateOriginalReservation start");

            if (dtReservation.From != originalReservation.From)
            {
                this.log.LogTrace("Update reservation from time");
                originalReservation.From = dtReservation.From;
            }

            if (dtReservation.To != originalReservation.To)
            {
                this.log.LogTrace("Update reservation to time");
                originalReservation.To = dtReservation.To;
            }

            //Check if we can use the original space
            if (!originalReservation.Space.Reservations.All(res => res.To < dtReservation.From && res.From > dtReservation.To))
            {
                // The space is not available now for the amended dates - reallocate
                List<Space> availableSpaces = await this.GetAvailableSpacesAsync(dtReservation.From, dtReservation.To);
                if (!availableSpaces.Any())
                {
                    string message = $"No spaces available";
                    this.log.LogWarning(message);
                    throw new InvalidOperationException(message);
                }

                this.log.LogTrace("Update reservation allocated space");
                originalReservation.Space = availableSpaces.First();
                originalReservation.SpaceId = availableSpaces.First().Id;
            }
            this.log.LogTrace("UpdateOriginalReservation done");
        }
    }
}
