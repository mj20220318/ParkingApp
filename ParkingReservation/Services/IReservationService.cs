using ParkingReservation.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ParkingReservation.Services
{
    // NOT COMPLETE - the services access the database this will allow us to unit test the controllers
    public interface IReservationService
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetByIdAsync(int id);
        Task DeleteAsync(int id);
    }
}