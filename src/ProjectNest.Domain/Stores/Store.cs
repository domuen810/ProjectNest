namespace ProjectNest.Domain.Stores;

public class Store
{
    public Guid Id { get; private set; }
    public Guid BrandId { get; private set; }
    public Guid MallId { get; private set; }
    public StoreStatus Status { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    // New Stores normally start in the PreOpening stage.
    // Existing Stores that are already trading can be created directly as Open.
    public Store(Guid brandId, Guid mallId, StoreStatus status = StoreStatus.PreOpening)
    {
        if (brandId == Guid.Empty)
        {
            throw new ArgumentException("Brand is required.", nameof(brandId));
        }

        if (mallId == Guid.Empty)
        {
            throw new ArgumentException("Mall is required.", nameof(mallId));
        }

        if (status == StoreStatus.Closed)
        {
            throw new ArgumentException("A Store cannot be created as Closed.", nameof(status));
        }

        Id = Guid.NewGuid();
        BrandId = brandId;
        MallId = mallId;
        Status = status;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // A Store can move to Open only after the pre-opening stage is complete.
    public void MarkAsOpen()
    {
        if (Status != StoreStatus.PreOpening)
        {
            throw new InvalidOperationException("Only a PreOpening Store can be marked as Open.");
        }

        Status = StoreStatus.Open;
        UpdatedAt = DateTime.UtcNow;
    }
    public void MarkAsClosed()
    {
        if (Status != StoreStatus.Open)
        {
            throw new InvalidOperationException("Only an Open Store can be marked as Closed.");
        }

        Status = StoreStatus.Closed;
        UpdatedAt = DateTime.UtcNow;
    }
}