using ParkingReservation.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingReservation.Services
{
    // NOT COMPLETE - the services access the database this will allow us to unit test the controllers
    public class ReservationService : IReservationService
    {
        public async Task<IEnumerable<Reservation>> GetAllReservationsAsync() => throw new NotImplementedException();
        public async Task<Reservation> GetByIdAsync(int id) => throw new NotImplementedException();
        public async Task DeleteAsync(int id) => throw new NotImplementedException();
    }
}
