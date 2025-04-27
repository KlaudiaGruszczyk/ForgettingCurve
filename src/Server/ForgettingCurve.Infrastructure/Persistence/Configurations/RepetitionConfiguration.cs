using ForgettingCurve.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ForgettingCurve.Infrastructure.Persistence.Configurations;

public class RepetitionConfiguration : IEntityTypeConfiguration<Repetition>
{
    public void Configure(EntityTypeBuilder<Repetition> builder)
    {
        builder.ToTable("Repetitions");
        
        builder.HasKey(r => r.Id);
        
        builder.Property(r => r.Id)
            .HasColumnName("RepetitionId")
            .ValueGeneratedOnAdd();
            
        builder.Property(r => r.TopicId)
            .IsRequired();
            
        builder.Property(r => r.ScheduledDate)
            .IsRequired();
            
        builder.Property(r => r.CompletedDate)
            .IsRequired(false);
            
        builder.Property(r => r.IntervalDays)
            .IsRequired(false);
            
        builder.HasOne(r => r.Topic)
            .WithMany(t => t.Repetitions)
            .HasForeignKey(r => r.TopicId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.HasIndex(r => r.TopicId);
        
        builder.HasIndex(r => r.ScheduledDate)
            .HasFilter("[CompletedDate] IS NULL")
            .HasDatabaseName("IX_Repetitions_ScheduledDate_NotCompleted");
            
        builder.HasIndex(r => new { r.TopicId, r.CompletedDate })
            .HasFilter("[CompletedDate] IS NOT NULL")
            .HasDatabaseName("IX_Repetitions_TopicId_CompletedDate_Desc");
    }
} 