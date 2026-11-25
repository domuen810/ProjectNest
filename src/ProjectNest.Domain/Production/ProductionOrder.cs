namespace ProjectNest.Domain.Production;

public class ProductionOrder
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public string OrderNumber { get; private set; } = string.Empty;
    public string Scope { get; private set; } = string.Empty;
    public Guid InitialDrawingRevisionId { get; private set; }
    public Guid CurrentDrawingRevisionId { get; private set; }
    public ProductionOrderStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private ProductionOrder()
    {
    }

    public ProductionOrder(Guid projectId, string orderNumber, string scope,
        Guid initialDrawingRevisionId, Guid createdByUserId)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(initialDrawingRevisionId, nameof(initialDrawingRevisionId));
        RequireId(createdByUserId, nameof(createdByUserId));

        OrderNumber = RequiredText(orderNumber, nameof(orderNumber));
        Scope = RequiredText(scope, nameof(scope));
        // Application validates that the revision belongs to this project and is a production drawing.
        Id = Guid.NewGuid();
        ProjectId = projectId;
        InitialDrawingRevisionId = initialDrawingRevisionId;
        CurrentDrawingRevisionId = initialDrawingRevisionId;
        Status = ProductionOrderStatus.Planned;
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

    public void Start(Guid updatedByUserId)
    {
        RequireStatus(ProductionOrderStatus.Planned);
        Touch(updatedByUserId);
        Status = ProductionOrderStatus.InProduction;
    }

    public void Complete(Guid updatedByUserId)
    {
        RequireStatus(ProductionOrderStatus.InProduction);
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
        if (Status is not (ProductionOrderStatus.Planned or ProductionOrderStatus.InProduction))
        {
            throw new InvalidOperationException("Only planned or in-production orders can be changed.");
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

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("A non-empty value is required.", parameterName);
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
