namespace ProjectNest.Domain.Drawings;

// Formal submitted/usable versions are retained; working saves are not revisions.
public class DrawingRevision
{
    private readonly List<string> affectedDrawingNumbers = new();

    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public DrawingStage DrawingStage { get; private set; }
    public string RevisionNumber { get; private set; }
    public DrawingUpdateType DrawingUpdateType { get; private set; }
    public IReadOnlyList<string> AffectedDrawingNumbers => affectedDrawingNumbers.AsReadOnly();
    public string? ChangeReason { get; private set; }
    public bool IsCurrent { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public DrawingRevision(
        Guid projectId, DrawingStage drawingStage, string revisionNumber,
        DrawingUpdateType drawingUpdateType, Guid createdByUserId,
        IEnumerable<string>? affectedDrawingNumbers = null, string? changeReason = null)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(createdByUserId, nameof(createdByUserId));
        if (!Enum.IsDefined(drawingStage))
        {
            throw new ArgumentException("Invalid drawing stage.", nameof(drawingStage));
        }
        if (!Enum.IsDefined(drawingUpdateType))
        {
            throw new ArgumentException("Invalid drawing update type.", nameof(drawingUpdateType));
        }

        // Sequence allocation and uniqueness are scoped to Project + DrawingStage outside this entity.
        RevisionNumber = RequiredText(revisionNumber, nameof(revisionNumber));
        if (affectedDrawingNumbers is not null)
        {
            foreach (var drawingNumber in affectedDrawingNumbers)
            {
                this.affectedDrawingNumbers.Add(RequiredText(drawingNumber, nameof(affectedDrawingNumbers)));
            }
        }
        if (drawingUpdateType == DrawingUpdateType.Partial && this.affectedDrawingNumbers.Count == 0)
        {
            throw new ArgumentException("A partial update requires at least one affected drawing number.", nameof(affectedDrawingNumbers));
        }

        Id = Guid.NewGuid();
        ProjectId = projectId;
        DrawingStage = drawingStage;
        DrawingUpdateType = drawingUpdateType;
        ChangeReason = string.IsNullOrWhiteSpace(changeReason) ? null : changeReason.Trim();
        // Application/database logic must retire the previous current revision for this project and stage.
        IsCurrent = true;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        // Required CAD and PDF associations and validation await the StoredFile workflow.
    }

    public void MarkAsNotCurrent()
    {
        IsCurrent = false;
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty value is required.", parameterName);
        }
        return value.Trim();
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
