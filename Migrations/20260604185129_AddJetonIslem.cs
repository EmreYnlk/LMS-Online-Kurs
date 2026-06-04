using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LMSPlatform.Migrations
{
    /// <inheritdoc />
    public partial class AddJetonIslem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JetonIslemleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    KullaniciId = table.Column<string>(type: "TEXT", nullable: false),
                    Tur = table.Column<int>(type: "INTEGER", nullable: false),
                    Miktar = table.Column<int>(type: "INTEGER", nullable: false),
                    KursId = table.Column<int>(type: "INTEGER", nullable: true),
                    TLTutar = table.Column<decimal>(type: "TEXT", nullable: true),
                    Aciklama = table.Column<string>(type: "TEXT", nullable: true),
                    Tarih = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JetonIslemleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JetonIslemleri_AspNetUsers_KullaniciId",
                        column: x => x.KullaniciId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JetonIslemleri_Kurslar_KursId",
                        column: x => x.KursId,
                        principalTable: "Kurslar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JetonIslemleri_KullaniciId",
                table: "JetonIslemleri",
                column: "KullaniciId");

            migrationBuilder.CreateIndex(
                name: "IX_JetonIslemleri_KursId",
                table: "JetonIslemleri",
                column: "KursId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JetonIslemleri");
        }
    }
}
