namespace ProjectNest.Domain.Projects;

public class MeasuredAreaChange
{
    public decimal? PreviousValue { get; }
    public decimal? NewValue { get; }
    public Guid ChangedByUserId { get; }
    public DateTime ChangedAt { get; }
    public string? Reason { get; }

    internal MeasuredAreaChange(decimal? previousValue, decimal? newValue, Guid changedByUserId, DateTime changedAt, string? reason)
    {
        PreviousValue = previousValue;
        NewValue = newValue;
        ChangedByUserId = changedByUserId;
        ChangedAt = changedAt;
        Reason = reason;
    }
}
