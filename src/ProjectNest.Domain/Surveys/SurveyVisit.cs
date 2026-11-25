using ProjectNest.Domain.Files;

namespace ProjectNest.Domain.Surveys;

public class SurveyVisit
{
    private readonly List<SurveyPersonnel> personnel = new();
    private readonly List<SurveyVisitFile> files = new();

    public Guid Id { get; private set; }
    public Guid SurveyWorkOrderId { get; private set; }
    public DateOnly PlannedDate { get; private set; }
    public DateOnly? ActualVisitDate { get; private set; }
    public SurveyVisitStatus Status { get; private set; }
    public IReadOnlyList<SurveyPersonnel> Personnel => personnel.AsReadOnly();
    public IReadOnlyList<SurveyVisitFile> Files => files.AsReadOnly();
    public Guid? VisitReportFileId { get; private set; }
    public string? Findings { get; private set; }
    public string? Notes { get; private set; }
    public string? NoResultReason { get; private set; }
    public Guid? ConfirmedByDesignerUserId { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    private SurveyVisit()
    {
    }

    public SurveyVisit(Guid surveyWorkOrderId, DateOnly plannedDate)
    {
        if (surveyWorkOrderId == Guid.Empty)
        {
            throw new ArgumentException("Survey work order is required.", nameof(surveyWorkOrderId));
        }

        RequireDate(plannedDate, nameof(plannedDate));
        Id = Guid.NewGuid();
        SurveyWorkOrderId = surveyWorkOrderId;
        PlannedDate = plannedDate;
        Status = SurveyVisitStatus.Planned;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void Start()
    {
        RequirePlanned();
        Status = SurveyVisitStatus.InProgress;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status is SurveyVisitStatus.Completed or SurveyVisitStatus.Cancelled)
        {
            throw new InvalidOperationException("A completed or cancelled survey visit cannot be cancelled.");
        }
        Status = SurveyVisitStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangePlannedDate(DateOnly plannedDate)
    {
        RequireUnconfirmed();
        RequirePlanned();
        RequireDate(plannedDate, nameof(plannedDate));
        PlannedDate = plannedDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RecordActualVisitDate(DateOnly actualVisitDate)
    {
        RequireUnconfirmed();
        RequireDate(actualVisitDate, nameof(actualVisitDate));
        ActualVisitDate = actualVisitDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPersonnel(IEnumerable<SurveyPersonnel> surveyPersonnel)
    {
        RequireUnconfirmed();
        ArgumentNullException.ThrowIfNull(surveyPersonnel);
        var replacement = surveyPersonnel.ToList();
        if (replacement.Any(person => person is null))
        {
            throw new ArgumentException("Survey personnel cannot contain null entries.", nameof(surveyPersonnel));
        }

        personnel.Clear();
        personnel.AddRange(replacement);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddFile(StoredFile file, SurveyFileCategory category)
    {
        ArgumentNullException.ThrowIfNull(file);
        if (!Enum.IsDefined(category))
        {
            throw new ArgumentException("Invalid survey file category.", nameof(category));
        }
        files.Add(new SurveyVisitFile(Id, file, category));
        UpdatedAt = DateTime.UtcNow;
    }

    public void AttachVisitReport(StoredFile file)
    {
        ArgumentNullException.ThrowIfNull(file);
        VisitReportFileId = file.Id;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFindingsAndNotes(string? findings, string? notes)
    {
        RequireUnconfirmed();
        Findings = OptionalText(findings);
        Notes = OptionalText(notes);
        UpdatedAt = DateTime.UtcNow;
    }

    public void ConfirmWithMeasurementResult(Guid designerUserId)
    {
        RequireUnconfirmed();
        if (!ActualVisitDate.HasValue)
        {
            throw new InvalidOperationException("Record the actual visit date before confirming measurement results.");
        }

        Confirm(designerUserId);
    }

    public void ConfirmWithoutResult(string noResultReason, Guid designerUserId)
    {
        RequireUnconfirmed();
        if (string.IsNullOrWhiteSpace(noResultReason))
        {
            throw new ArgumentException("A reason is required when no usable measurement result exists.", nameof(noResultReason));
        }

        Confirm(designerUserId);
        NoResultReason = noResultReason.Trim();
    }

    private void Confirm(Guid designerUserId)
    {
        if (Status == SurveyVisitStatus.Cancelled)
        {
            throw new InvalidOperationException("A cancelled survey visit cannot be confirmed.");
        }

        if (designerUserId == Guid.Empty)
        {
            throw new ArgumentException("Confirming designer is required.", nameof(designerUserId));
        }

        ConfirmedByDesignerUserId = designerUserId;
        ConfirmedAt = DateTime.UtcNow;
        Status = SurveyVisitStatus.Completed;
        UpdatedAt = ConfirmedAt.Value;
    }

    private void RequireUnconfirmed()
    {
        if (ConfirmedAt.HasValue)
        {
            throw new InvalidOperationException("A confirmed survey visit cannot be changed.");
        }
    }

    private void RequirePlanned()
    {
        if (Status != SurveyVisitStatus.Planned)
        {
            throw new InvalidOperationException("Only a planned survey visit can be changed or started.");
        }
    }

    private static void RequireDate(DateOnly value, string parameterName)
    {
        if (value == default)
        {
            throw new ArgumentException("A date is required.", parameterName);
        }
    }

    private static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
