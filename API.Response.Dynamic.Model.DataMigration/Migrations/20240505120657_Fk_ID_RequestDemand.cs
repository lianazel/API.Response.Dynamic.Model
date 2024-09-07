using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Response.Dynamic.Model.DataMigration.Migrations
{
    /// <inheritdoc />
    public partial class Fk_ID_RequestDemand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fk_ID_DataModel",
                table: "ApiDemandProperties",
                newName: "Fk_ID_Demand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Fk_ID_Demand",
                table: "ApiDemandProperties",
                newName: "Fk_ID_DataModel");
        }
    }
}
