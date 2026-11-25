namespace ProjectNest.Domain.Identity;

public sealed class JobPosition
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }

    internal JobPosition(string code, string name)
    {
        Code = RequiredText(code, nameof(code));
        Name = RequiredText(name, nameof(name));
        Id = Guid.NewGuid();
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty value is required.", parameterName);
        }
        return value.Trim();
    }
}
