using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Response.Dynamic.Model.DataMigration.Migrations
{
    /// <inheritdoc />
    public partial class TypeBDD_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiDemandProperties_ApiDemandProperties_ApiDemandPropertiesFk_ID_Demand",
                table: "ApiDemandProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiDemandProperties",
                table: "ApiDemandProperties");

            migrationBuilder.RenameColumn(
                name: "ApiDemandPropertiesFk_ID_Demand",
                table: "ApiDemandProperties",
                newName: "ApiDemandPropertiesID_Properties");

            migrationBuilder.RenameIndex(
                name: "IX_ApiDemandProperties_ApiDemandPropertiesFk_ID_Demand",
                table: "ApiDemandProperties",
                newName: "IX_ApiDemandProperties_ApiDemandPropertiesID_Properties");

            migrationBuilder.AddColumn<string>(
                name: "BDDType",
                table: "ApiDemandRequest",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "ID_Properties",
                table: "ApiDemandProperties",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Fk_ID_Demand",
                table: "ApiDemandProperties",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiDemandProperties",
                table: "ApiDemandProperties",
                column: "ID_Properties");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiDemandProperties_ApiDemandProperties_ApiDemandPropertiesID_Properties",
                table: "ApiDemandProperties",
                column: "ApiDemandPropertiesID_Properties",
                principalTable: "ApiDemandProperties",
                principalColumn: "ID_Properties");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApiDemandProperties_ApiDemandProperties_ApiDemandPropertiesID_Properties",
                table: "ApiDemandProperties");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApiDemandProperties",
                table: "ApiDemandProperties");

            migrationBuilder.DropColumn(
                name: "BDDType",
                table: "ApiDemandRequest");

            migrationBuilder.RenameColumn(
                name: "ApiDemandPropertiesID_Properties",
                table: "ApiDemandProperties",
                newName: "ApiDemandPropertiesFk_ID_Demand");

            migrationBuilder.RenameIndex(
                name: "IX_ApiDemandProperties_ApiDemandPropertiesID_Properties",
                table: "ApiDemandProperties",
                newName: "IX_ApiDemandProperties_ApiDemandPropertiesFk_ID_Demand");

            migrationBuilder.AlterColumn<string>(
                name: "Fk_ID_Demand",
                table: "ApiDemandProperties",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ID_Properties",
                table: "ApiDemandProperties",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApiDemandProperties",
                table: "ApiDemandProperties",
                column: "Fk_ID_Demand");

            migrationBuilder.AddForeignKey(
                name: "FK_ApiDemandProperties_ApiDemandProperties_ApiDemandPropertiesFk_ID_Demand",
                table: "ApiDemandProperties",
                column: "ApiDemandPropertiesFk_ID_Demand",
                principalTable: "ApiDemandProperties",
                principalColumn: "Fk_ID_Demand");
        }
    }
}
