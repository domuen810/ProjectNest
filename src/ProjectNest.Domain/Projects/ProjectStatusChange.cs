namespace ProjectNest.Domain.Projects;

public class ProjectStatusChange
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public ProjectStatus PreviousStatus { get; private set; }
    public ProjectStatus NewStatus { get; private set; }
    public string? Reason { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid ChangedByUserId { get; private set; }

    private ProjectStatusChange()
    {
    }

    internal ProjectStatusChange(Guid projectId, ProjectStatus previousStatus, ProjectStatus newStatus,
        string? reason, DateTime changedAt, Guid changedByUserId)
    {
        if (projectId == Guid.Empty) throw new ArgumentException("Project is required.", nameof(projectId));
        if (!Enum.IsDefined(previousStatus)) throw new ArgumentException("Invalid previous status.", nameof(previousStatus));
        if (!Enum.IsDefined(newStatus)) throw new ArgumentException("Invalid new status.", nameof(newStatus));
        if (previousStatus == newStatus) throw new ArgumentException("A status change must change the status.", nameof(newStatus));
        if (changedByUserId == Guid.Empty) throw new ArgumentException("Changing user is required.", nameof(changedByUserId));

        Id = Guid.NewGuid();
        ProjectId = projectId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        Reason = string.IsNullOrWhiteSpace(reason) ? null : reason.Trim();
        ChangedAt = changedAt;
        ChangedByUserId = changedByUserId;
    }
}
