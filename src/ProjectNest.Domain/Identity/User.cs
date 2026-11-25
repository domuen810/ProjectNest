namespace ProjectNest.Domain.Identity;

public class User
{
    private readonly List<SystemRole> systemRoles = new();

    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Email { get; private set; }
    public Guid JobPositionId { get; private set; }
    public IReadOnlyList<SystemRole> SystemRoles => systemRoles.AsReadOnly();
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private User()
    {
        Name = string.Empty;
        Email = string.Empty;
    }

    public User(string name, string email, JobPosition jobPosition)
    {
        Name = RequiredText(name, nameof(name));
        Email = RequiredText(email, nameof(email));
        ArgumentNullException.ThrowIfNull(jobPosition);

        Id = Guid.NewGuid();
        JobPositionId = jobPosition.Id;
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void ChangeJobPosition(JobPosition jobPosition)
    {
        ArgumentNullException.ThrowIfNull(jobPosition);
        if (JobPositionId == jobPosition.Id) return;

        JobPositionId = jobPosition.Id;
        Touch();
    }

    public void AssignSystemRole(SystemRole systemRole)
    {
        ArgumentNullException.ThrowIfNull(systemRole);
        if (systemRoles.Any(role => string.Equals(role.Code, systemRole.Code, StringComparison.Ordinal))) return;

        systemRoles.Add(systemRole);
        Touch();
    }

    public void RemoveSystemRole(SystemRole systemRole)
    {
        ArgumentNullException.ThrowIfNull(systemRole);
        var assignedRole = systemRoles.FirstOrDefault(
            role => string.Equals(role.Code, systemRole.Code, StringComparison.Ordinal));
        if (assignedRole is null) return;

        systemRoles.Remove(assignedRole);
        Touch();
    }

    public void Deactivate()
    {
        if (!IsActive) return;
        IsActive = false;
        Touch();
    }

    public void Reactivate()
    {
        if (IsActive) return;
        IsActive = true;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
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
