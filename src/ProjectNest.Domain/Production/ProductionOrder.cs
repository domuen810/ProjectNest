namespace ProjectNest.Domain.Production;

public class ProductionOrder
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string Scope { get; private set; }
    public Guid InitialDrawingRevisionId { get; private set; }
    public Guid CurrentDrawingRevisionId { get; private set; }
    public ProductionOrderStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public ProductionOrder(Guid projectId, string scope, Guid initialDrawingRevisionId, Guid createdByUserId)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(initialDrawingRevisionId, nameof(initialDrawingRevisionId));
        RequireId(createdByUserId, nameof(createdByUserId));
        if (string.IsNullOrWhiteSpace(scope))
        {
            throw new ArgumentException("Production scope is required.", nameof(scope));
        }

        Id = Guid.NewGuid();
        ProjectId = projectId;
        Scope = scope.Trim();
        // Application validates that the revision belongs to this project and is a production drawing.
        InitialDrawingRevisionId = initialDrawingRevisionId;
        CurrentDrawingRevisionId = initialDrawingRevisionId;
        Status = ProductionOrderStatus.Active;
        CreatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedByUserId = createdByUserId;
        UpdatedAt = CreatedAt;
    }

    public void ChangeCurrentDrawingRevision(Guid drawingRevisionId, Guid updatedByUserId)
    {
        RequireOpen();
        RequireId(drawingRevisionId, nameof(drawingRevisionId));
        Touch(updatedByUserId);
        // The originally issued drawing reference is retained even when production adopts a newer revision.
        CurrentDrawingRevisionId = drawingRevisionId;
    }

    public void Pause(Guid updatedByUserId)
    {
        RequireStatus(ProductionOrderStatus.Active);
        Touch(updatedByUserId);
        Status = ProductionOrderStatus.Paused;
    }

    public void Resume(Guid updatedByUserId)
    {
        RequireStatus(ProductionOrderStatus.Paused);
        Touch(updatedByUserId);
        // Resuming preserves the same order identity and history.
        Status = ProductionOrderStatus.Active;
    }

    public void Complete(Guid updatedByUserId)
    {
        RequireStatus(ProductionOrderStatus.Active);
        Touch(updatedByUserId);
        Status = ProductionOrderStatus.Completed;
    }

    public void Cancel(Guid updatedByUserId)
    {
        RequireOpen();
        Touch(updatedByUserId);
        Status = ProductionOrderStatus.Cancelled;
    }

    private void RequireOpen()
    {
        if (Status != ProductionOrderStatus.Active && Status != ProductionOrderStatus.Paused)
        {
            throw new InvalidOperationException("Only active or paused production orders can be changed.");
        }
    }

    private void RequireStatus(ProductionOrderStatus status)
    {
        if (Status != status)
        {
            throw new InvalidOperationException($"This transition requires a {status} production order.");
        }
    }

    private void Touch(Guid updatedByUserId)
    {
        RequireId(updatedByUserId, nameof(updatedByUserId));
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
