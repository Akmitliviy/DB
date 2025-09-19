using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ScaffoldDB.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageReports_Rents_RentId",
                table: "DamageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleLicensePlate",
                table: "InsurancePolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Vehicles_VehicleLicencePlate",
                table: "ServiceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReports_Rents_RentId",
                table: "DamageReports",
                column: "RentId",
                principalTable: "Rents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleLicensePlate",
                table: "InsurancePolicies",
                column: "VehicleLicensePlate",
                principalTable: "Vehicles",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Vehicles_VehicleLicencePlate",
                table: "ServiceRecords",
                column: "VehicleLicensePlate",
                principalTable: "Vehicles",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DamageReports_Rents_RentId",
                table: "DamageReports");

            migrationBuilder.DropForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleLicensePlate",
                table: "InsurancePolicies");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceRecords_Vehicles_VehicleLicencePlate",
                table: "ServiceRecords");

            migrationBuilder.AddForeignKey(
                name: "FK_DamageReports_Rents_RentId",
                table: "DamageReports",
                column: "RentId",
                principalTable: "Rents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InsurancePolicies_Vehicles_VehicleLicensePlate",
                table: "InsurancePolicies",
                column: "VehicleLicensePlate",
                principalTable: "Vehicles",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceRecords_Vehicles_VehicleLicencePlate",
                table: "ServiceRecords",
                column: "VehicleLicensePlate",
                principalTable: "Vehicles",
                principalColumn: "LicensePlate",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
