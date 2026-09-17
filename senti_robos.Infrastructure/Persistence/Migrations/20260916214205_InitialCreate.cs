using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace senti_robos.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Robos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TipoIncidente = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Latitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    Longitud = table.Column<decimal>(type: "decimal(9,6)", nullable: false),
                    FechaHoraIncidente = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UsuarioReportanteId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Estado = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Robos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Robos_FechaHoraIncidente",
                table: "Robos",
                column: "FechaHoraIncidente");

            migrationBuilder.CreateIndex(
                name: "IX_Robos_Latitud_Longitud",
                table: "Robos",
                columns: new[] { "Latitud", "Longitud" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Robos");
        }
    }
}
