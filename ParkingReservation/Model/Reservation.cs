using System;

namespace ParkingReservation.Model
{
    /// <summary>
    /// Reservation.
    /// </summary>
    public class Reservation
    {
        /// <summary>
        /// Gets or sets the reservation id (we could make this the booking reference instead i.e REF#ID)
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the space id.
        /// </summary>
        public int SpaceId { get; set; }

        /// <summary>
        /// Gets or sets the space.
        /// </summary>
        public Space Space { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the reservation is valid from.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the reservation is valid to.
        /// </summary>
        public DateTime To { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the reservation was intially created.
        /// </summary>
        public DateTime Created { get; set; }
    }
}
