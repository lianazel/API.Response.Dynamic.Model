using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Response.Dynamic.Model.DataMigration.Migrations
{
    /// <inheritdoc />
    public partial class MyDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiDemandRequest",
                columns: table => new
                {
                    ID_Demand = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID = table.Column<long>(type: "bigint", nullable: false),
                    DataModelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataModelNumberProperties = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDemandRequest", x => x.ID_Demand);
                });

            migrationBuilder.CreateTable(
                name: "ApiDemandProperties",
                columns: table => new
                {
                    ID_Properties = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID = table.Column<long>(type: "bigint", nullable: false),
                    Fk_ID_DataModel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PropertyType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApiDemandPropertiesID_Properties = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ApiDemandRequestID_Demand = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDemandProperties", x => x.ID_Properties);
                    table.ForeignKey(
                        name: "FK_ApiDemandProperties_ApiDemandProperties_ApiDemandPropertiesID_Properties",
                        column: x => x.ApiDemandPropertiesID_Properties,
                        principalTable: "ApiDemandProperties",
                        principalColumn: "ID_Properties");
                    table.ForeignKey(
                        name: "FK_ApiDemandProperties_ApiDemandRequest_ApiDemandRequestID_Demand",
                        column: x => x.ApiDemandRequestID_Demand,
                        principalTable: "ApiDemandRequest",
                        principalColumn: "ID_Demand");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApiDemandProperties_ApiDemandPropertiesID_Properties",
                table: "ApiDemandProperties",
                column: "ApiDemandPropertiesID_Properties");

            migrationBuilder.CreateIndex(
                name: "IX_ApiDemandProperties_ApiDemandRequestID_Demand",
                table: "ApiDemandProperties",
                column: "ApiDemandRequestID_Demand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiDemandProperties");

            migrationBuilder.DropTable(
                name: "ApiDemandRequest");
        }
    }
}
