using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingReservation.Dtos
{
    /// <summary>
    /// Reservation dto.
    /// </summary>
    public class DtReservation
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
        /// Gets or sets the date and time that the reservation is valid from.
        /// </summary>
        public DateTime From { get; set; }

        /// <summary>
        /// Gets or sets the date and time that the reservation is valid to.
        /// </summary>
        public DateTime To { get; set; }
    }
}
