using ProjectNest.Domain.Files;

namespace ProjectNest.Domain.Drawings;

// Formal submitted/usable versions are retained; working saves are not revisions.
public class DrawingRevision
{
    private readonly List<DrawingRevisionFile> fileAssociations = new();

    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public DrawingStage DrawingStage { get; private set; }
    public string RevisionNumber { get; private set; } = string.Empty;
    public DrawingChangeScope ChangeScope { get; private set; }
    public IReadOnlyList<DrawingRevisionFile> FileAssociations => fileAssociations.AsReadOnly();
    public DrawingChangeReason ChangeReason { get; private set; }
    public string? ChangeSummary { get; private set; }
    public string? FeedbackOrChangeRequest { get; private set; }
    public string? AffectedPagesOrAreas { get; private set; }
    public bool IsCurrent { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private DrawingRevision()
    {
    }

    public DrawingRevision(
        Guid projectId, DrawingStage drawingStage, string revisionNumber,
        DrawingChangeScope changeScope, DrawingChangeReason changeReason,
        Guid createdByUserId,
        IEnumerable<(StoredFile File, DrawingRevisionFileRole FileRole)> fileAssociations,
        string? changeSummary = null, string? feedbackOrChangeRequest = null,
        string? affectedPagesOrAreas = null)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(createdByUserId, nameof(createdByUserId));
        ArgumentNullException.ThrowIfNull(fileAssociations);
        if (!Enum.IsDefined(drawingStage))
        {
            throw new ArgumentException("Invalid drawing stage.", nameof(drawingStage));
        }
        if (!Enum.IsDefined(changeScope))
        {
            throw new ArgumentException("Invalid drawing change scope.", nameof(changeScope));
        }
        if (!Enum.IsDefined(changeReason))
        {
            throw new ArgumentException("Invalid drawing change reason.", nameof(changeReason));
        }

        // Sequence allocation and uniqueness are scoped to Project + DrawingStage outside this entity.
        RevisionNumber = RequiredText(revisionNumber, nameof(revisionNumber));
        var requiredFileAssociations = fileAssociations.ToList();
        if (requiredFileAssociations.Count == 0)
        {
            throw new ArgumentException("A drawing revision must have at least one associated file.", nameof(fileAssociations));
        }
        foreach (var (file, fileRole) in requiredFileAssociations)
        {
            ArgumentNullException.ThrowIfNull(file);
            if (file.Id == Guid.Empty)
            {
                throw new ArgumentException("An associated file must have an identifier.", nameof(fileAssociations));
            }
            if (!Enum.IsDefined(fileRole))
            {
                throw new ArgumentException("Invalid drawing revision file role.", nameof(fileAssociations));
            }
        }

        var normalizedAffectedPagesOrAreas = OptionalText(affectedPagesOrAreas);
        if (changeScope == DrawingChangeScope.Partial && normalizedAffectedPagesOrAreas is null)
        {
            throw new ArgumentException("A partial update requires affected pages or areas.", nameof(affectedPagesOrAreas));
        }

        Id = Guid.NewGuid();
        ProjectId = projectId;
        DrawingStage = drawingStage;
        ChangeScope = changeScope;
        ChangeReason = changeReason;
        ChangeSummary = OptionalText(changeSummary);
        FeedbackOrChangeRequest = OptionalText(feedbackOrChangeRequest);
        AffectedPagesOrAreas = normalizedAffectedPagesOrAreas;
        // Application/database logic must retire the previous current revision for this project and stage.
        IsCurrent = true;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;

        foreach (var (file, fileRole) in requiredFileAssociations)
        {
            AddFile(file, fileRole);
        }
    }

    public void AddFile(StoredFile file, DrawingRevisionFileRole fileRole)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (!Enum.IsDefined(fileRole))
        {
            throw new ArgumentException("Invalid drawing revision file role.", nameof(fileRole));
        }
        fileAssociations.Add(new DrawingRevisionFile(Id, file, fileRole));
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

    private static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
