using Microsoft.EntityFrameworkCore.Migrations;

namespace ParkingReservation.Migrations
{
    public partial class Data : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("INSERT INTO Spaces (Terminal, IsActive) VALUES('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1), ('T1', 1)");
            migrationBuilder.Sql("  INSERT INTO Reservations (SpaceId, [From], [To], Created) VALUES (1, '2022-08-03', '2022-08-15', '2022-08-03'), (2, '2022-08-03', '2022-08-11', '2022-08-03'), (3, '2022-08-03', '2022-08-14', '2022-08-03'), (4, '2022-08-03', '2022-08-07', '2022-08-03'), (5, '2022-08-03', '2022-08-20', '2022-08-03'), (6, '2022-08-03', '2022-08-18', '2022-08-03'), (7, '2022-08-03', '2022-08-12', '2022-08-03'), (8, '2022-08-03', '2022-08-05', '2022-08-03'), (9, '2022-08-27', '2022-08-30', '2022-08-03')");

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
