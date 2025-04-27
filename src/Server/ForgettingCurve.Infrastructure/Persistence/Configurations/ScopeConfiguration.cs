using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgettingCurve.Infrastructure.Persistence.Configurations;

public class ScopeConfiguration : IEntityTypeConfiguration<Scope>
{
    public void Configure(EntityTypeBuilder<Scope> builder)
    {
        builder.ToTable("Scopes");
        
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Id)
            .HasColumnName("ScopeId")
            .ValueGeneratedOnAdd();
            
        builder.Property(s => s.OwnerUserId)
            .IsRequired();
            
        builder.Property(s => s.Name)
            .HasMaxLength(150)
            .IsRequired();
            
        builder.Property(s => s.CreationDate)
            .HasDefaultValueSql("GETUTCDATE()")
            .IsRequired();
            
        builder.Property(s => s.LastModifiedDate)
            .IsRequired(false);
            
        // Indeksy
        builder.HasIndex(s => s.OwnerUserId);
        builder.HasIndex(s => s.CreationDate);
        builder.HasIndex(s => s.Name);
    }
} 