using ForgettingCurve.Domain.Exceptions;

namespace ForgettingCurve.Domain.Entities;

public class Scope : BaseEntity
{
    public Guid OwnerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public DateTime CreationDate { get; private set; }
    public DateTime? LastModifiedDate { get; private set; }
    
    public ICollection<Topic> Topics { get; private set; } = new List<Topic>();

    private Scope() { }

    public Scope(Guid ownerId, string name)
    {
        ValidateName(name);

        Id = Guid.NewGuid();
        OwnerUserId = ownerId;
        Name = name;
        CreationDate = DateTime.UtcNow;
    }

    public void UpdateName(string name)
    {
        ValidateName(name);

        Name = name;
        LastModifiedDate = DateTime.UtcNow;
    }
    
    private static void ValidateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ValidationException("Name cannot be empty");
        }
        
        if (name.Length > 150)
        {
            throw new ValidationException("Name cannot exceed 150 characters");
        }
    }
} 