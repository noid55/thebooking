using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace thebooking.Migrations
{
    /// <inheritdoc />
    public partial class UniqueRoomEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomEquipments_RoomId",
                table: "RoomEquipments");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Buildings",
                type: "TEXT",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_RoomId_EquipmentId",
                table: "RoomEquipments",
                columns: new[] { "RoomId", "EquipmentId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RoomEquipments_RoomId_EquipmentId",
                table: "RoomEquipments");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Buildings",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RoomEquipments_RoomId",
                table: "RoomEquipments",
                column: "RoomId");
        }
    }
}
