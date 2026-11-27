
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thebooking.Migrations
{
    public partial class SyncBuildingEquipmentModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Opprett byggene først
            migrationBuilder.CreateTable(
                name: "Buildings",
                columns: table => new
                {
                    BuildingId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Address = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Buildings", x => x.BuildingId);
                });

            // 2. Opprett utstyrstabellene
            migrationBuilder.CreateTable(
                name: "Equipment",
                columns: table => new
                {
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Equipment", x => x.EquipmentId);
                });

            // 3. Legg til BuildingId midlertidig med verdien 0
            migrationBuilder.AddColumn<int>(
                name: "BuildingId",
                table: "Rooms",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // 4. Opprett Bygg 1–5 med faste ID-er
            migrationBuilder.Sql(
                """
                INSERT INTO Buildings (BuildingId, Name) VALUES
                (1, 'Bygg 1'),
                (2, 'Bygg 2'),
                (3, 'Bygg 3'),
                (4, 'Bygg 4'),
                (5, 'Bygg 5');
                """);

            // 5. Koble eksisterende rom til riktig bygg
            migrationBuilder.Sql(
                """
                UPDATE Rooms
                SET BuildingId = CASE Building
                    WHEN 'P35' THEN 1
                    WHEN 'P48' THEN 2
                    WHEN '52' THEN 3
                    WHEN 'p52' THEN 4
                    ELSE 5
                END;
                """);

            // 6. Nå er byggkoblingen overført – fjern gammel kolonne
            migrationBuilder.DropColumn(
                name: "Building",
                table: "Rooms");

            // 7. Oppdater Booking-modellen
            migrationBuilder.RenameColumn(
                name: "BookedBy",
                table: "Bookings",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Bookings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            // 8. Opprett koblingstabellen mellom rom og utstyr
            migrationBuilder.CreateTable(
                name: "RoomEquipments",
                columns: table => new
                {
                    RoomEquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    EquipmentId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoomEquipments", x => x.RoomEquipmentId);
                    table.ForeignKey(
                        name: "FK_RoomEquipments_Equipment_EquipmentId",
                        column: x => x.EquipmentId,
                        principalTable: "Equipment",
                        principalColumn: "EquipmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoomEquipments_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Cascade);
                });

            // 9. Opprett indekser
            migrationBuilder.CreateIndex(
                name: "IX_Rooms_BuildingId_Name",
                table: "Rooms",
                columns: new[] { "BuildingId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_EquipmentId",
                table: "RoomEquipments",
                column: "EquipmentId");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_RoomId_EquipmentId",
                table: "RoomEquipments",
                columns: new[] { "RoomId", "EquipmentId" },
                unique: true);

            // 10. Koble rom til bygg
            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms",
                column: "BuildingId",
                principalTable: "Buildings",
                principalColumn: "BuildingId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Buildings_BuildingId",
                table: "Rooms");

            migrationBuilder.DropTable(name: "RoomEquipments");
            migrationBuilder.DropTable(name: "Equipment");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_BuildingId_Name",
                table: "Rooms");

            migrationBuilder.AddColumn<string>(
                name: "Building",
                table: "Rooms",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql(
                """
                UPDATE Rooms
                SET Building = CASE BuildingId
                    WHEN 1 THEN 'P35'
                    WHEN 2 THEN 'P48'
                    WHEN 3 THEN '52'
                    WHEN 4 THEN 'p52'
                    ELSE 'Bygg 5'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "BuildingId",
                table: "Rooms");

            migrationBuilder.DropTable(name: "Buildings");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Bookings");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Bookings",
                newName: "BookedBy");
        }
    }
}

