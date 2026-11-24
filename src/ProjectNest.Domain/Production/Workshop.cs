namespace ProjectNest.Domain.Production;

// Workshop reference data excludes pre-assembly, packing and logistics.
public class Workshop
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public Workshop(string name)
    {
        Name = RequiredName(name);
        Id = Guid.NewGuid();
        IsActive = true;
    }

    public void Rename(string name)
    {
        Name = RequiredName(name);
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Reactivate()
    {
        IsActive = true;
    }

    private static string RequiredName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Workshop name is required.", nameof(name));
        }
        return name.Trim();
    }
}
