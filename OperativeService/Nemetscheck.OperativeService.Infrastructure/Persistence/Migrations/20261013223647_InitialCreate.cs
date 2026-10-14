using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nemetscheck.OperativeService.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiceRolls",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserId = table.Column<long>(type: "INTEGER", nullable: false),
                    FirstDie = table.Column<int>(type: "INTEGER", nullable: false),
                    SecondDie = table.Column<int>(type: "INTEGER", nullable: false),
                    Sum = table.Column<int>(type: "INTEGER", nullable: false),
                    RolledAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiceRolls", x => x.Id);
                    table.CheckConstraint("CK_DiceRolls_FirstDie", "FirstDie BETWEEN 1 AND 6");
                    table.CheckConstraint("CK_DiceRolls_SecondDie", "SecondDie BETWEEN 1 AND 6");
                    table.CheckConstraint("CK_DiceRolls_Sum", "Sum = FirstDie + SecondDie");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiceRolls_UserId_RolledAtUtc",
                table: "DiceRolls",
                columns: new[] { "UserId", "RolledAtUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_DiceRolls_UserId_Sum",
                table: "DiceRolls",
                columns: new[] { "UserId", "Sum" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiceRolls");
        }
    }
}
