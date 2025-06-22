using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceManager.Application.Migrations
{
    /// <inheritdoc />
    public partial class UniqueNameColumnForTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_TransactionTypes_Name", table: "TransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_TransactionCategories_Name",
                table: "TransactionCategories"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_Name",
                table: "TransactionTypes",
                column: "Name",
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategories_Name",
                table: "TransactionCategories",
                column: "Name",
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(name: "IX_TransactionTypes_Name", table: "TransactionTypes");

            migrationBuilder.DropIndex(
                name: "IX_TransactionCategories_Name",
                table: "TransactionCategories"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TransactionTypes_Name",
                table: "TransactionTypes",
                column: "Name"
            );

            migrationBuilder.CreateIndex(
                name: "IX_TransactionCategories_Name",
                table: "TransactionCategories",
                column: "Name"
            );
        }
    }
}
