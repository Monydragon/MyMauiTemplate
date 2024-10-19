using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyMobileFriends.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationCode",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    ExpirationTime = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Username = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    ProfilePicturePath = table.Column<string>(type: "TEXT", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    TwoFactorSecret = table.Column<string>(type: "TEXT", nullable: true),
                    BackupCodes = table.Column<string>(type: "TEXT", nullable: true),
                    Require2Fa = table.Column<bool>(type: "INTEGER", nullable: false),
                    TrustedDevices = table.Column<string>(type: "TEXT", nullable: true),
                    StoreSavedPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    VerificationCodeId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsGuest = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_VerificationCode_VerificationCodeId",
                        column: x => x.VerificationCodeId,
                        principalTable: "VerificationCode",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_VerificationCodeId",
                table: "Users",
                column: "VerificationCodeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "VerificationCode");
        }
    }
}
