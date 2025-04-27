using System;
using System.Collections.Generic;

namespace ForgettingCurve.Domain.Entities;

public class Topic : BaseEntity
{
    public Guid ScopeId { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public string? Notes { get; set; }
    public bool IsMastered { get; set; }
    public DateTime CreationDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    public Scope Scope { get; set; } = null!;
    public ICollection<Repetition> Repetitions { get; set; } = new List<Repetition>();
} 