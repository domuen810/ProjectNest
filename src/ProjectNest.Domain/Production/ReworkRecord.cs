namespace ProjectNest.Domain.Production;

public class ReworkRecord
{
    public Guid Id { get; private set; }
    public Guid ProductionStageProgressId { get; private set; }
    public ReworkReasonType ReasonType { get; private set; }
    public string? Note { get; private set; }
    public Guid RecordedByUserId { get; private set; }
    public DateTime RecordedAt { get; private set; }

    private ReworkRecord()
    {
    }

    internal ReworkRecord(Guid productionStageProgressId, ReworkReasonType reasonType,
        Guid recordedByUserId, string? note)
    {
        if (productionStageProgressId == Guid.Empty)
        {
            throw new ArgumentException("Production stage progress is required.", nameof(productionStageProgressId));
        }
        if (!Enum.IsDefined(reasonType))
        {
            throw new ArgumentException("Invalid rework reason type.", nameof(reasonType));
        }
        if (recordedByUserId == Guid.Empty)
        {
            throw new ArgumentException("Recording user is required.", nameof(recordedByUserId));
        }

        Id = Guid.NewGuid();
        ProductionStageProgressId = productionStageProgressId;
        ReasonType = reasonType;
        Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim();
        RecordedByUserId = recordedByUserId;
        RecordedAt = DateTime.UtcNow;
        // Quality-related photos and file associations will be handled later through StoredFile.
    }
}
