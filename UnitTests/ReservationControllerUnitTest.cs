//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Extensions.Logging;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Moq;
//using ParkingReservation.Controllers;
//using ParkingReservation.Model;
//using ParkingReservation.Services;
//using System;
//using System.Threading.Tasks;
//using Xunit;
//using Assert = Xunit.Assert;

//namespace UnitTests
//{
//    [TestClass]
//    public class ReservationControllerUnitTest
//    {
//        private readonly ReservationsController _controller;
//        private readonly IReservationService _service;
//        public ReservationControllerUnitTest()
//        {
//            _service = new ReservationServiceFake();
//            _controller = new ReservationsController(_service);
//        }

//        [Fact]
//        public async Task Should_Return_Reservation_When_Reservation_Found()
//        {
//            //Arrange
//            //ILogger loggerFactory = CreateMockedLogger();
//            Reservation arrangedReservation = new Reservation()
//            {
//                From = new DateTime(2022, 08, 03),
//                To = new DateTime(2022, 08, 15),
//                Id = 1,
//                SpaceId = 1, 
//                Created = new DateTime(2022, 08, 03)
//            };

//            Mock<IReservationService> mockReservationService = new Mock<IReservationService>();
//            mockReservationService.Setup(c => c.GetByIdAsync(arrangedReservation.Id)).ReturnsAsync(arrangedReservation);

//            //Act
//            ReservationsController service = new ReservationsController(mockReservationService.Object);
//            ActionResult<Reservation> result = await service.GetReservationAsync(arrangedReservation.Id);

//            //Assert
//            Assert.NotNull(result);
//            Assert.Equal(result.Value.Id, arrangedReservation.Id);

//        }

//        // Additional tests: make sure 404 is returned when reservation not found
//        // Create reservation
//        // Create reservation with no spaces available returned
//        // Amend reservation

//        //SPACES tests to go in SpacesControllerUnitTest
//        // Get spaces and make sure 10 are returned 
//        // Create space
//        // Get available spaces between dates
//        // Get available spaces today

//    }
//}
