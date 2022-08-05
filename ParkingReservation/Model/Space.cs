using System.Collections.Generic;

namespace ParkingReservation.Model
{
    /// <summary>
    /// Space.
    /// </summary>
    public class Space
    {
        /// <summary>
        /// Gets or sets the id.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the terminal.
        /// </summary>
        public string Terminal { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the space is active or not.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the reservations associated with the space.
        /// </summary>
        public List<Reservation> Reservations { get; set; } = new List<Reservation>();
    }
}
