using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StoreWebApi.Migrations.WalletMigrations
{
    /// <inheritdoc />
    public partial class addWalletCurrency : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "Wallets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 1,
                column: "Currency",
                value: "USD $");

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: 2,
                column: "Currency",
                value: "USD $");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "Wallets");
        }
    }
}
