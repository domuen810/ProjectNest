namespace ProjectNest.Domain.Projects;

public class OpeningDateChange
{
    public DateOnly PreviousValue { get; }
    public DateOnly NewValue { get; }
    public Guid ChangedByUserId { get; }
    public DateTime ChangedAt { get; }

    internal OpeningDateChange(DateOnly previousValue, DateOnly newValue, Guid changedByUserId, DateTime changedAt)
    {
        PreviousValue = previousValue;
        NewValue = newValue;
        ChangedByUserId = changedByUserId;
        ChangedAt = changedAt;
    }
}
