using System;
using System.Collections.Generic;

namespace ForgettingCurve.Domain.Entities;

public class Scope : BaseEntity
{
    public Guid OwnerUserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreationDate { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    
    public ICollection<Topic> Topics { get; set; } = new List<Topic>();
} 