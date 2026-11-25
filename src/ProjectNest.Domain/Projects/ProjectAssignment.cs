namespace ProjectNest.Domain.Projects;

public class ProjectAssignment
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public Guid DesignerUserId { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? EndedAt { get; private set; }
    public Guid AssignedByUserId { get; private set; }

    private ProjectAssignment()
    {
    }

    internal ProjectAssignment(Guid projectId, Guid designerUserId, DateTime assignedAt, Guid assignedByUserId)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(designerUserId, nameof(designerUserId));
        RequireId(assignedByUserId, nameof(assignedByUserId));

        Id = Guid.NewGuid();
        ProjectId = projectId;
        DesignerUserId = designerUserId;
        AssignedAt = assignedAt;
        AssignedByUserId = assignedByUserId;
    }

    internal void End(DateTime endedAt)
    {
        if (EndedAt.HasValue)
        {
            throw new InvalidOperationException("An ended ProjectAssignment cannot be ended again.");
        }

        EndedAt = endedAt;
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
