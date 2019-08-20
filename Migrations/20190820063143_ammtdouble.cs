using Microsoft.EntityFrameworkCore.Migrations;

namespace bank_accounts.Migrations
{
    public partial class ammtdouble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AmmountDecimal",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AmmountDecimal",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
