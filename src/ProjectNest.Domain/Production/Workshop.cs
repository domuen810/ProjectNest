namespace ProjectNest.Domain.Production;

// The MVP Workshop catalog contains the seven defined production trades.
public class Workshop
{
    private static readonly HashSet<string> allowedNames = new(StringComparer.Ordinal)
    {
        "Carpentry",
        "Painting",
        "Glass",
        "Metal",
        "Electrical",
        "Pre-assembly",
        "Packing & Shipping"
    };

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }

    public Workshop(string name)
    {
        Name = RequiredName(name);
        Id = Guid.NewGuid();
        IsActive = true;
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

        var normalizedName = name.Trim();
        if (!allowedNames.Contains(normalizedName))
        {
            throw new ArgumentException("Workshop must be one of the predefined MVP trades.", nameof(name));
        }

        return normalizedName;
    }
}
