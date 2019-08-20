using Microsoft.EntityFrameworkCore.Migrations;

namespace bank_accounts.Migrations
{
    public partial class ammtdec : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "AmmountDecimal",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AmmountDecimal",
                table: "Transactions",
                nullable: false,
                oldClrType: typeof(int));
        }
    }
}
