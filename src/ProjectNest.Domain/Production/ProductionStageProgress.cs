namespace ProjectNest.Domain.Production;

public class ProductionStageProgress
{
    private readonly List<ProductionProgressHistory> statusHistory = new();
    private readonly List<ReworkRecord> reworkHistory = new();

    public Guid Id { get; private set; }
    public Guid ProductionOrderId { get; private set; }
    public Guid WorkshopId { get; private set; }
    public ProductionStageStatus Status { get; private set; }
    public IReadOnlyList<ProductionProgressHistory> StatusHistory => statusHistory.AsReadOnly();
    public IReadOnlyList<ReworkRecord> ReworkHistory => reworkHistory.AsReadOnly();

    public ProductionStageProgress(Guid productionOrderId, Guid workshopId)
    {
        RequireId(productionOrderId, nameof(productionOrderId));
        RequireId(workshopId, nameof(workshopId));
        Id = Guid.NewGuid();
        ProductionOrderId = productionOrderId;
        WorkshopId = workshopId;
        Status = ProductionStageStatus.NotStarted;
    }

    public void Start(Guid changedByUserId)
    {
        RequireStatus(ProductionStageStatus.NotStarted);
        ChangeStatus(ProductionStageStatus.InProgress, changedByUserId);
    }

    public void Complete(Guid changedByUserId)
    {
        RequireStatus(ProductionStageStatus.InProgress);
        ChangeStatus(ProductionStageStatus.Completed, changedByUserId);
    }

    public void Block(Guid changedByUserId)
    {
        RequireStatus(ProductionStageStatus.InProgress);
        ChangeStatus(ProductionStageStatus.Blocked, changedByUserId);
    }

    public void ResumeFromBlocked(Guid changedByUserId)
    {
        RequireStatus(ProductionStageStatus.Blocked);
        ChangeStatus(ProductionStageStatus.InProgress, changedByUserId);
    }

    public void RecordRework(ReworkReasonType reasonType, Guid recordedByUserId, string? note = null)
    {
        if (Status == ProductionStageStatus.NotStarted)
        {
            throw new InvalidOperationException("Workshop work must have started before recording rework.");
        }

        var record = new ReworkRecord(Id, reasonType, recordedByUserId, note);
        // Rework reopens completed workshop work and appends to, rather than replaces, its history.
        reworkHistory.Add(record);
        ChangeStatus(ProductionStageStatus.InProgress, recordedByUserId);
    }

    private void ChangeStatus(ProductionStageStatus newStatus, Guid changedByUserId)
    {
        if (Status == newStatus) return;

        RequireId(changedByUserId, nameof(changedByUserId));
        var changedAt = DateTime.UtcNow;
        statusHistory.Add(new ProductionProgressHistory(Id, Status, newStatus, changedAt, changedByUserId));
        Status = newStatus;
    }

    private void RequireStatus(ProductionStageStatus status)
    {
        if (Status != status)
        {
            throw new InvalidOperationException($"This transition requires a {status} production stage.");
        }
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
