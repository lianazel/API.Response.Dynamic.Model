using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Response.Dynamic.Model.DataMigration.Migrations
{
    /// <inheritdoc />
    public partial class SqlCode_NewTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "DataModelNumberProperties",
                table: "ApiDemandRequest",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DataModelNumberProperties",
                table: "ApiDemandRequest",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
