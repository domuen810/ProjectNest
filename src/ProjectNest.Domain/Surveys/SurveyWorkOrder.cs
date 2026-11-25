using ProjectNest.Domain.Files;

namespace ProjectNest.Domain.Surveys;

public class SurveyWorkOrder
{
    private readonly List<SurveyVisit> visits = new();

    public Guid Id { get; private set; }
    public Guid ProjectId { get; private set; }
    public SurveyType SurveyType { get; private set; }
    public DateOnly? PlannedSurveyDate { get; private set; }
    public SurveyWorkOrderStatus Status { get; private set; }
    public string? Notes { get; private set; }
    public Guid? FinalSurveyReportFileId { get; private set; }
    public IReadOnlyList<SurveyVisit> Visits => visits.AsReadOnly();
    public Guid CreatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SurveyWorkOrder()
    {
    }

    public SurveyWorkOrder(Guid projectId, SurveyType surveyType, DateOnly? plannedSurveyDate, Guid createdByUserId, string? notes = null)
    {
        RequireId(projectId, nameof(projectId));
        RequireId(createdByUserId, nameof(createdByUserId));
        if (!Enum.IsDefined(surveyType))
        {
            throw new ArgumentException("A valid survey type is required.", nameof(surveyType));
        }
        if (plannedSurveyDate.HasValue)
        {
            RequirePlannedDate(plannedSurveyDate.Value);
        }

        Id = Guid.NewGuid();
        ProjectId = projectId;
        SurveyType = surveyType;
        PlannedSurveyDate = plannedSurveyDate;
        Notes = OptionalText(notes);
        Status = SurveyWorkOrderStatus.Planned;
        CreatedByUserId = createdByUserId;
        UpdatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void AddVisit(SurveyVisit visit)
    {
        ArgumentNullException.ThrowIfNull(visit);
        if (visit.SurveyWorkOrderId != Id)
        {
            throw new ArgumentException("The visit belongs to a different survey work order.", nameof(visit));
        }
        if (visits.Any(existing => existing.Id == visit.Id)) return;
        visits.Add(visit);
    }

    public void AttachFinalSurveyReport(StoredFile file, Guid updatedByUserId)
    {
        ArgumentNullException.ThrowIfNull(file);
        Touch(updatedByUserId);
        FinalSurveyReportFileId = file.Id;
    }

    public void ChangePlannedSurveyDate(DateOnly? plannedSurveyDate, Guid updatedByUserId)
    {
        RequirePlanned();
        if (plannedSurveyDate.HasValue)
        {
            RequirePlannedDate(plannedSurveyDate.Value);
        }
        Touch(updatedByUserId);
        PlannedSurveyDate = plannedSurveyDate;
    }

    public void UpdateNotes(string? notes, Guid updatedByUserId)
    {
        RequirePlanned();
        Touch(updatedByUserId);
        Notes = OptionalText(notes);
    }

    public void Start(Guid updatedByUserId)
    {
        RequirePlanned();
        Touch(updatedByUserId);
        Status = SurveyWorkOrderStatus.InProgress;
    }

    public void Complete(Guid updatedByUserId)
    {
        RequireNotTerminal();
        Touch(updatedByUserId);
        Status = SurveyWorkOrderStatus.Completed;
    }

    public void Cancel(Guid updatedByUserId)
    {
        RequireNotTerminal();
        Touch(updatedByUserId);
        Status = SurveyWorkOrderStatus.Cancelled;
    }

    private void RequirePlanned()
    {
        if (Status != SurveyWorkOrderStatus.Planned)
        {
            throw new InvalidOperationException("Only a planned survey work order can be changed or started.");
        }
    }

    private void RequireNotTerminal()
    {
        if (Status is SurveyWorkOrderStatus.Completed or SurveyWorkOrderStatus.Cancelled)
        {
            throw new InvalidOperationException("A completed or cancelled survey work order cannot be changed.");
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

    private static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
