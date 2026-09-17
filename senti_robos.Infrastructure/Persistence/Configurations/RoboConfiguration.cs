using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using senti_robos.Domain;

namespace senti_robos.Infrastructure.Persistence.Configurations;

public class RoboConfiguration : IEntityTypeConfiguration<Robo>
{
    public void Configure(EntityTypeBuilder<Robo> builder)
    {
        builder.ToTable("Robos");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.TipoIncidente)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(r => r.Estado)
            .HasConversion<string>()
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(r => r.Descripcion)
            .HasMaxLength(1000);

        // decimal(9,6) gives ~11cm precision at the equator, more than
        // enough for incident geolocation, and keeps range-scan filtering
        // (GET api/robbery_points' bounding box) simple with a plain
        // composite B-tree index — see Q1 of the design discussion. A
        // spatial column (geography + spatial index) is a deliberate
        // non-choice here: at this project's scale a rectangular bounding
        // box is a pure range predicate, which a spatial index doesn't
        // speed up over a composite index, and it would add a
        // NetTopologySuite dependency for no benefit. Revisit only if
        // queries need radius/polygon shapes or the table grows to
        // millions of rows.
        builder.Property(r => r.Latitud).HasColumnType("decimal(9,6)");
        builder.Property(r => r.Longitud).HasColumnType("decimal(9,6)");

        builder.Property(r => r.UsuarioReportanteId)
            .HasMaxLength(128)
            .IsRequired();

        builder.HasIndex(r => new { r.Latitud, r.Longitud })
            .HasDatabaseName("IX_Robos_Latitud_Longitud");

        builder.HasIndex(r => r.FechaHoraIncidente)
            .HasDatabaseName("IX_Robos_FechaHoraIncidente");
    }
}
