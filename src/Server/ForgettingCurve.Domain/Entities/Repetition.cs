using System;

namespace ForgettingCurve.Domain.Entities;

public class Repetition : BaseEntity
{
    public Guid TopicId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public DateTime? CompletedDate { get; set; }
    public int? IntervalDays { get; set; }
    
    public Topic Topic { get; set; } = null!;
} 