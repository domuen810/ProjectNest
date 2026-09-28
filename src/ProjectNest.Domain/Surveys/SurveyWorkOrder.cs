namespace ProjectNest.Domain.Surveys;

public class SurveyWorkOrder
{
    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public DateOnly PlannedSurveyDate { get; private set; }
    public SurveyWorkOrderStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public SurveyWorkOrder(Guid projectId, DateOnly plannedSurveyDate, Guid createdByUserId)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(createdByUserId, nameof(createdByUserId));
        RequirePlannedDate(plannedSurveyDate);

        Id = Guid.NewGuid();
        ProjectId = projectId;
        PlannedSurveyDate = plannedSurveyDate;
        // Application checks the Project's Mall contact details before dispatching survey work.
        Status = SurveyWorkOrderStatus.Scheduled;
        CreatedByUserId = createdByUserId;
        UpdatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void ChangePlannedSurveyDate(DateOnly plannedSurveyDate, Guid updatedByUserId)
    {
        RequireScheduled();
        RequirePlannedDate(plannedSurveyDate);
        Touch(updatedByUserId);
        PlannedSurveyDate = plannedSurveyDate;
    }

    // Completion is an explicit decision, independent of any visit's type or confirmation.
    public void Complete(Guid updatedByUserId)
    {
        RequireScheduled();
        Touch(updatedByUserId);
        Status = SurveyWorkOrderStatus.Completed;
    }

    public void Cancel(Guid updatedByUserId)
    {
        RequireScheduled();
        Touch(updatedByUserId);
        Status = SurveyWorkOrderStatus.Cancelled;
    }

    private void RequireScheduled()
    {
        if (Status != SurveyWorkOrderStatus.Scheduled)
        {
            throw new InvalidOperationException("Only a scheduled survey work order can be changed.");
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

    private static void RequirePlannedDate(DateOnly plannedSurveyDate)
    {
        if (plannedSurveyDate == default)
        {
            throw new ArgumentException("Planned survey date is required.", nameof(plannedSurveyDate));
        }
    }
}
