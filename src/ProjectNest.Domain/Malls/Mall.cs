namespace ProjectNest.Domain.Malls;

public class Mall
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string City { get; private set; } = string.Empty;
    public string Address { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public Mall(string name, string city, string address)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Mall name is required.", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(city))
        {
            throw new ArgumentException("City is required.", nameof(city));
        }

        if (string.IsNullOrWhiteSpace(address))
        {
            throw new ArgumentException("Address is required.", nameof(address));
        }

        Id = Guid.NewGuid();
        Name = name.Trim();
        City = city.Trim();
        Address = address.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Mall name is required.", nameof(name));
        }

        // Mall name may change after refurbishment without creating a new Mall.
        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
    public void Deactivate()
    {
        // Keep the Mall for historical projects while preventing it from being used for new work.
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
    public void Reactivate()
    {
        // A previously inactive mall can be used again if the mall reopens.
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }
}