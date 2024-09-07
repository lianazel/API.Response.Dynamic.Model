using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Response.Dynamic.Model.DataMigration.Migrations
{
    /// <inheritdoc />
    public partial class ApiDemandSqlCode_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ApiDemandSqlCode",
                columns: table => new
                {
                    ID_SqlCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ID = table.Column<long>(type: "bigint", nullable: false),
                    Fk_ID_Demand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SqlCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SqlWhereClause = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApiDemandSqlCode", x => x.ID_SqlCode);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ApiDemandSqlCode");
        }
    }
}
