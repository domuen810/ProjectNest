namespace ProjectNest.Domain.Brands;

public class Brand
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Code { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Brand(string name, string code)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Brand name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Brand code is required.", nameof(code));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        Code = code.Trim().ToUpperInvariant();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        // A brand must always have a valid name.
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Brand name is required.", nameof(name));
        }

        // Brand name can be changed or corrected without changing Brand identity or history.
        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        // When cooperation stops, keep the Brand for historical projects.
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Reactivate()
    {
        // A previously inactive brand can be used again if the company resumes working with it.
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}