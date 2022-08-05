using ParkingReservation.Model;
using ParkingReservation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// NOT COMPLETE - the services access the database this will allow us to unit test the controllers
public class ReservationServiceFake : IReservationService
{
    private readonly List<Reservation> _reservations;
    public ReservationServiceFake()
    {
        _reservations = new List<Reservation>()
            {
                new Reservation() { Id = 1, From= new DateTime(2022, 08, 03), To = new DateTime(2022, 08, 15), SpaceId = 1 },    
            };
    }
    public async Task<IEnumerable<Reservation>> GetAllReservationsAsync()
    {
        return _reservations;
    }
    public async Task<Reservation> CreateReservationAsync(Reservation newItem)
    {
        _reservations.Add(newItem);
        return newItem;
    }
    public async Task<Reservation> GetByIdAsync(int id)
    {
        return _reservations.Where(a => a.Id == id)
            .FirstOrDefault();
    }
    public async Task DeleteAsync(int id)
    {
        var existing = _reservations.First(a => a.Id == id);
        _reservations.Remove(existing);
    }
}