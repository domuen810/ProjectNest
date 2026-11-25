namespace ProjectNest.Domain.Contacts;

public sealed class SavedContact
{
    public Guid Id { get; private set; }
    public Guid? BrandId { get; private set; }
    public Guid? MallId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Position { get; private set; }
    public string? Phone { get; private set; }
    public string? Email { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SavedContact()
    {
    }

    public SavedContact(Guid? brandId, Guid? mallId, string name,
        string? position = null, string? phone = null, string? email = null)
    {
        ValidateOwner(brandId, mallId);
        Name = RequiredText(name, nameof(name));
        BrandId = brandId;
        MallId = mallId;
        Position = OptionalText(position);
        Phone = OptionalText(phone);
        Email = OptionalText(email);
        Id = Guid.NewGuid();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void UpdateDetails(string name, string? position, string? phone, string? email)
    {
        Name = RequiredText(name, nameof(name));
        Position = OptionalText(position);
        Phone = OptionalText(phone);
        Email = OptionalText(email);
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reactivate()
    {
        if (IsActive) return;
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void ValidateOwner(Guid? brandId, Guid? mallId)
    {
        if (brandId.HasValue == mallId.HasValue)
        {
            throw new ArgumentException("A SavedContact must belong to exactly one Brand or Mall.");
        }
        if (brandId == Guid.Empty) throw new ArgumentException("Brand is required.", nameof(brandId));
        if (mallId == Guid.Empty) throw new ArgumentException("Mall is required.", nameof(mallId));
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameterName);
        return value.Trim();
    }

    private static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
