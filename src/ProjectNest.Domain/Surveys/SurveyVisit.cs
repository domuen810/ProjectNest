namespace ProjectNest.Domain.Surveys;

public class SurveyVisit
{
    private readonly List<SurveyPersonnel> personnel = new();

    public Guid Id { get; private set; }
    public Guid SurveyWorkOrderId { get; private set; }
    public SurveyVisitType SurveyVisitType { get; private set; }
    public DateOnly PlannedDate { get; private set; }
    public DateOnly? ActualVisitDate { get; private set; }
    public IReadOnlyList<SurveyPersonnel> Personnel => personnel.AsReadOnly();
    public string? NoResultReason { get; private set; }
    public Guid? ConfirmedByDesignerUserId { get; private set; }
    public DateTime? ConfirmedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public SurveyVisit(Guid surveyWorkOrderId, SurveyVisitType surveyVisitType, DateOnly plannedDate)
    {
        if (surveyWorkOrderId == Guid.Empty)
        {
            throw new ArgumentException("Survey work order is required.", nameof(surveyWorkOrderId));
        }

        if (!Enum.IsDefined(surveyVisitType))
        {
            throw new ArgumentException("A valid survey visit type is required.", nameof(surveyVisitType));
        }

        RequireDate(plannedDate, nameof(plannedDate));
        Id = Guid.NewGuid();
        SurveyWorkOrderId = surveyWorkOrderId;
        SurveyVisitType = surveyVisitType;
        PlannedDate = plannedDate;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void ChangePlannedDate(DateOnly plannedDate)
    {
        RequireUnconfirmed();
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

    public void ConfirmWithMeasurementResult(Guid designerUserId)
    {
        RequireUnconfirmed();
        if (!ActualVisitDate.HasValue)
        {
            throw new InvalidOperationException("Record the actual visit date before confirming measurement results.");
        }

        // Result-file association and upload verification belong to the future StoredFile workflow.
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
        if (designerUserId == Guid.Empty)
        {
            throw new ArgumentException("Confirming designer is required.", nameof(designerUserId));
        }

        ConfirmedByDesignerUserId = designerUserId;
        ConfirmedAt = DateTime.UtcNow;
        UpdatedAt = ConfirmedAt.Value;
    }

    private void RequireUnconfirmed()
    {
        if (ConfirmedAt.HasValue)
        {
            throw new InvalidOperationException("A confirmed survey visit cannot be changed.");
        }
    }

    private static void RequireDate(DateOnly value, string parameterName)
    {
        if (value == default)
        {
            throw new ArgumentException("A date is required.", parameterName);
        }
    }
}
