using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingReservation.Dtos
{
    /// <summary>
    /// Space dto.
    /// </summary>
    public class DtSpace
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
    }
}
