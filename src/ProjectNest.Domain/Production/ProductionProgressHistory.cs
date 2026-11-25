namespace ProjectNest.Domain.Production;

public class ProductionProgressHistory
{
    public Guid Id { get; private set; }
    public Guid ProductionStageProgressId { get; private set; }
    public ProductionStageStatus PreviousStatus { get; private set; }
    public ProductionStageStatus NewStatus { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid ChangedByUserId { get; private set; }

    private ProductionProgressHistory()
    {
    }

    internal ProductionProgressHistory(Guid productionStageProgressId,
        ProductionStageStatus previousStatus, ProductionStageStatus newStatus,
        DateTime changedAt, Guid changedByUserId)
    {
        RequireId(productionStageProgressId, nameof(productionStageProgressId));
        RequireStatus(previousStatus, nameof(previousStatus));
        RequireStatus(newStatus, nameof(newStatus));
        RequireId(changedByUserId, nameof(changedByUserId));

        Id = Guid.NewGuid();
        ProductionStageProgressId = productionStageProgressId;
        PreviousStatus = previousStatus;
        NewStatus = newStatus;
        ChangedAt = changedAt;
        ChangedByUserId = changedByUserId;
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }

    private static void RequireStatus(ProductionStageStatus value, string parameterName)
    {
        if (!Enum.IsDefined(value))
        {
            throw new ArgumentException("Invalid production stage status.", parameterName);
        }
    }
}
