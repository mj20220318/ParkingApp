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
    /// Spaces Controller.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SpacesController : ControllerBase
    {
        private readonly ParkingReservationDbContext context;
        private readonly ILogger<SpacesController> log;

        /// <summary>
        /// Initializes a new instance of the <see cref="SpacesController"/> class.
        /// </summary>
        /// <param name="log">Logger.</param>
        /// <param name="context">Db context.</param>
        public SpacesController(ILogger<SpacesController> log, ParkingReservationDbContext context)
        {
            this.log = log;
            this.context = context;
        }

        /// <summary>
        /// Create space.
        /// </summary>
        /// <param name="dtSpace">Space dto.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpPost]
        [Route("/api/spaces")]
        public async Task<ActionResult<Space>> CreateSpaceAsync(DtSpace dtSpace)
        {
            this.log.LogTrace("CreateSpaceAsync start");

            // Create new space 
            Space space = new Space
            {
                IsActive = dtSpace.IsActive,
                Terminal = dtSpace.Terminal,
            };

            context.Spaces.Add(space);

            await context.SaveChangesAsync();

            this.log.LogTrace("CreateSpaceAsync done");
            return CreatedAtAction("GetSpace", new { id = dtSpace.Id }, dtSpace);
        }

        /// <summary>
        /// Delete the space.
        /// </summary>
        /// <param name="spaceId">Space id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpDelete]
        [Route("/api/spaces/{spaceId}")]
        public async Task<IActionResult> DeleteSpaceAsync([FromRoute][Required]int spaceId)
        {
            this.log.LogTrace("DeleteSpaceAsync start");

            Space space = await context.Spaces.FindAsync(spaceId);
            if (space == null)
            {
                return NotFound();
            }

            context.Spaces.Remove(space);
            await context.SaveChangesAsync();

            this.log.LogTrace("DeleteSpaceAsync done");
            return NoContent();
        }

        /// <summary>
        /// Get available spaces from providing a start and end datetime.
        /// </summary>
        /// <param name="from">Start datetime.</param>
        /// <param name="to">End datetime.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/available-spaces")]
        public async Task<ActionResult<List<DtSpace>>> GetAvailableSpacesAsync([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            this.log.LogTrace("GetAvailableSpacesAsync start");
            List<Space> availableSpaces = await this.GetAvailableSpacesFromDatesAsync(from, to);

            List<DtSpace> dtos = availableSpaces.Select(s => this.PackDto(s)).ToList();
            if (!availableSpaces.Any())
            {
                string message = $"No spaces available";
                this.log.LogWarning(message);
                return this.BadRequest(message);
            }

            return this.Ok(dtos);
        }

        /// <summary>
        /// Get the space by id.
        /// </summary>
        /// <param name="spaceId">Space id.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/spaces/{spaceId}")]
        public async Task<ActionResult<DtSpace>> GetSpaceAsync([FromRoute][Required] int spaceId)
        {
            this.log.LogTrace("GetSpaceAsync start {0}", spaceId);

            if (spaceId <= 0)
            {
                throw new ArgumentNullException(nameof(spaceId));
            }
            Space space = await context.Spaces.FindAsync(spaceId);

            if (space == null)
            {
                return NotFound();
            }

            this.log.LogTrace("GetSpaceAsync done {0}", spaceId);

            return this.PackDto(space);
        }

        /// <summary>
        /// Get the spaces.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [HttpGet]
        [Route("/api/spaces")]
        public async Task<ActionResult<IEnumerable<Space>>> GetSpacesAsync()
        {
            this.log.LogTrace("GetSpacesAsync start");
            List<DtSpace> dtos = await this.context.Spaces.Select(s => this.PackDto(s)).ToListAsync();
            return this.Ok(dtos);
        }

        /// <summary>
        /// Convert the space object to the dto.
        /// </summary>
        /// <param name="space">Space.</param>
        /// <returns>Space dto.</returns>
        private DtSpace PackDto(Space space)
        {
            return new DtSpace
            {
                IsActive = space.IsActive,
                Id = space.Id,
                Terminal = space.Terminal,
            };
        }

        /// <summary>
        /// Get available spaces between two given dates.
        /// </summary>
        /// <param name="from">Start date.</param>
        /// <param name="to">End date.</param>
        /// <returns>List of available spaces.</returns>
        private async Task<List<Space>> GetAvailableSpacesFromDatesAsync(DateTime from, DateTime to)
        {
            this.log.LogTrace("GetAvailableSpacesFromDatesAsync start");
            return await this.context.Spaces.Include(s => s.Reservations)
                .Where(space => space.Reservations.All(res => res.To < from || res.From > to))
                .ToListAsync();
        }
    }
}
