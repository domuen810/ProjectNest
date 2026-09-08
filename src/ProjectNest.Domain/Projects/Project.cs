namespace ProjectNest.Domain.Projects;

public class Project
{
    private readonly List<OpeningDateChange> openingDateHistory = new();
    private readonly List<MeasuredAreaChange> measuredAreaHistory = new();

    public Guid Id { get; private set; }
    public string ProjectNumber { get; private set; }
    public Guid StoreId { get; private set; }
    public ProjectType ProjectType { get; private set; }
    public DateOnly OpeningDate { get; private set; }
    public decimal? InitialArea { get; private set; }
    public decimal? MeasuredArea { get; private set; }
    public string? Floor { get; private set; }
    public string? ShopNumber { get; private set; }
    public string? InitialRequirements { get; private set; }
    public Guid? ResponsibleDesignerId { get; private set; }
    public string? PaymentArrangement { get; private set; }
    public string BrandNameSnapshot { get; private set; }
    public string MallNameSnapshot { get; private set; }
    public string? BrandContactName { get; private set; }
    public string? BrandContactEmail { get; private set; }
    public string MallContactName { get; private set; }
    public string? MallContactRole { get; private set; }
    public string MallContactPhone { get; private set; }
    public string? MallContactEmail { get; private set; }
    public CounterType? CounterType { get; private set; }
    public bool? HasIntegratedBeautyRoom { get; private set; }
    public bool? HasExistingCounter { get; private set; }
    public ExistingCounterLocationRelation? ExistingCounterLocationRelation { get; private set; }
    public bool? RequiresExistingCounterRemoval { get; private set; }
    public string? ExistingCounterRemovalReason { get; private set; }
    public ProjectStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public Guid? RelatedPermanentCounterProjectId { get; private set; }
    public IReadOnlyList<OpeningDateChange> OpeningDateHistory => openingDateHistory.AsReadOnly();
    public IReadOnlyList<MeasuredAreaChange> MeasuredAreaHistory => measuredAreaHistory.AsReadOnly();

    public Project(
        string projectNumber, Guid storeId, ProjectType projectType, DateOnly openingDate,
        string brandNameSnapshot, string mallNameSnapshot, string mallContactName,
        string mallContactPhone, Guid createdByUserId,
        CounterType? counterType = null, bool? hasIntegratedBeautyRoom = null,
        bool? hasExistingCounter = null,
        ExistingCounterLocationRelation? existingCounterLocationRelation = null,
        bool? requiresExistingCounterRemoval = null, string? existingCounterRemovalReason = null,
        Guid? relatedPermanentCounterProjectId = null,
        decimal? initialArea = null, decimal? measuredArea = null,
        string? floor = null, string? shopNumber = null, string? initialRequirements = null,
        Guid? responsibleDesignerId = null, string? paymentArrangement = null,
        string? brandContactName = null, string? brandContactEmail = null,
        string? mallContactRole = null, string? mallContactEmail = null)
    {
        ProjectNumber = RequiredText(projectNumber, nameof(projectNumber));
        RequireId(storeId, nameof(storeId));
        RequireId(createdByUserId, nameof(createdByUserId));
        if (!Enum.IsDefined(projectType))
        {
            throw new ArgumentException("Invalid project type.", nameof(projectType));
        }
        RequireOpeningDate(openingDate);
        BrandNameSnapshot = RequiredText(brandNameSnapshot, nameof(brandNameSnapshot));
        MallNameSnapshot = RequiredText(mallNameSnapshot, nameof(mallNameSnapshot));
        MallContactName = RequiredText(mallContactName, nameof(mallContactName));
        MallContactPhone = RequiredText(mallContactPhone, nameof(mallContactPhone));
        ValidateOptionalId(responsibleDesignerId, nameof(responsibleDesignerId));

        ProjectType = projectType;
        ValidateCounter(counterType, hasIntegratedBeautyRoom);
        ValidateExistingCounter(hasExistingCounter, existingCounterLocationRelation,
            requiresExistingCounterRemoval, existingCounterRemovalReason);
        // Existence, target project type and one-to-one uniqueness require cross-record validation.
        if (projectType == ProjectType.TemporaryCounter)
        {
            RequireId(relatedPermanentCounterProjectId ?? Guid.Empty, nameof(relatedPermanentCounterProjectId));
        }
        else if (relatedPermanentCounterProjectId.HasValue)
        {
            throw new ArgumentException("Only temporary counters reference a permanent counter project.", nameof(relatedPermanentCounterProjectId));
        }

        Id = Guid.NewGuid();
        StoreId = storeId;
        OpeningDate = openingDate;
        InitialArea = initialArea;
        MeasuredArea = measuredArea;
        Floor = OptionalText(floor);
        ShopNumber = OptionalText(shopNumber);
        InitialRequirements = OptionalText(initialRequirements);
        ResponsibleDesignerId = responsibleDesignerId;
        PaymentArrangement = OptionalText(paymentArrangement);
        BrandContactName = OptionalText(brandContactName);
        BrandContactEmail = OptionalText(brandContactEmail);
        MallContactRole = OptionalText(mallContactRole);
        MallContactEmail = OptionalText(mallContactEmail);
        CounterType = counterType;
        HasIntegratedBeautyRoom = hasIntegratedBeautyRoom;
        HasExistingCounter = hasExistingCounter;
        ExistingCounterLocationRelation = existingCounterLocationRelation;
        RequiresExistingCounterRemoval = requiresExistingCounterRemoval;
        ExistingCounterRemovalReason = OptionalText(existingCounterRemovalReason);
        RelatedPermanentCounterProjectId = relatedPermanentCounterProjectId;
        Status = ProjectStatus.Active;
        CreatedByUserId = createdByUserId;
        UpdatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;
    }

    public void ChangeOpeningDate(DateOnly openingDate, Guid updatedByUserId)
    {
        RequireOpeningDate(openingDate);
        RequireId(updatedByUserId, nameof(updatedByUserId));
        if (OpeningDate == openingDate) return;

        var changedAt = DateTime.UtcNow;
        openingDateHistory.Add(new OpeningDateChange(OpeningDate, openingDate, updatedByUserId, changedAt));
        OpeningDate = openingDate;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = changedAt;
    }

    public void ChangeMeasuredArea(decimal? measuredArea, Guid updatedByUserId, string? reason = null)
    {
        RequireId(updatedByUserId, nameof(updatedByUserId));
        if (MeasuredArea == measuredArea) return;

        // Replacing or clearing a confirmed measurement must preserve its value and justification.
        reason = MeasuredArea.HasValue ? RequiredText(reason, nameof(reason)) : OptionalText(reason);
        var changedAt = DateTime.UtcNow;
        measuredAreaHistory.Add(new MeasuredAreaChange(MeasuredArea, measuredArea, updatedByUserId, changedAt, reason));
        MeasuredArea = measuredArea;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = changedAt;
    }

    public void UpdateInitialArea(decimal? initialArea, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        InitialArea = initialArea;
    }

    public void UpdateLocation(string? floor, string? shopNumber, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        Floor = OptionalText(floor);
        ShopNumber = OptionalText(shopNumber);
    }

    public void UpdateInitialRequirements(string? initialRequirements, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        InitialRequirements = OptionalText(initialRequirements);
    }

    public void AssignResponsibleDesigner(Guid? responsibleDesignerId, Guid updatedByUserId)
    {
        ValidateOptionalId(responsibleDesignerId, nameof(responsibleDesignerId));
        Touch(updatedByUserId);
        ResponsibleDesignerId = responsibleDesignerId;
    }

    public void UpdatePaymentArrangement(string? paymentArrangement, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        PaymentArrangement = OptionalText(paymentArrangement);
    }

    public void UpdateBrandContact(string? name, string? email, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        BrandContactName = OptionalText(name);
        BrandContactEmail = OptionalText(email);
    }

    public void UpdateMallContact(string name, string phone, string? role, string? email, Guid updatedByUserId)
    {
        name = RequiredText(name, nameof(name));
        phone = RequiredText(phone, nameof(phone));
        Touch(updatedByUserId);
        MallContactName = name;
        MallContactPhone = phone;
        MallContactRole = OptionalText(role);
        MallContactEmail = OptionalText(email);
    }

    public void UpdateCounter(CounterType? counterType, bool? hasIntegratedBeautyRoom, Guid updatedByUserId)
    {
        ValidateCounter(counterType, hasIntegratedBeautyRoom);
        Touch(updatedByUserId);
        CounterType = counterType;
        HasIntegratedBeautyRoom = hasIntegratedBeautyRoom;
    }

    public void UpdateExistingCounter(bool? hasExistingCounter,
        ExistingCounterLocationRelation? locationRelation, bool? requiresRemoval,
        string? removalReason, Guid updatedByUserId)
    {
        ValidateExistingCounter(hasExistingCounter, locationRelation, requiresRemoval, removalReason);
        Touch(updatedByUserId);
        HasExistingCounter = hasExistingCounter;
        ExistingCounterLocationRelation = locationRelation;
        RequiresExistingCounterRemoval = requiresRemoval;
        ExistingCounterRemovalReason = OptionalText(removalReason);
    }

    public void Pause(Guid updatedByUserId)
    {
        RequireStatus(ProjectStatus.Active);
        Touch(updatedByUserId);
        Status = ProjectStatus.Paused;
    }

    public void Resume(Guid updatedByUserId)
    {
        RequireStatus(ProjectStatus.Paused);
        Touch(updatedByUserId);
        Status = ProjectStatus.Active;
    }

    public void Cancel(Guid updatedByUserId)
    {
        if (Status != ProjectStatus.Active && Status != ProjectStatus.Paused)
        {
            throw new InvalidOperationException("Only active or paused projects can be cancelled.");
        }
        Touch(updatedByUserId);
        Status = ProjectStatus.Cancelled;
    }

    public void Complete(Guid updatedByUserId)
    {
        RequireStatus(ProjectStatus.Active);
        Touch(updatedByUserId);
        Status = ProjectStatus.Completed;
    }

    private void ValidateCounter(CounterType? counterType, bool? hasIntegratedBeautyRoom)
    {
        if (ProjectType == ProjectType.PermanentCounter)
        {
            if (!counterType.HasValue || !Enum.IsDefined(counterType.Value))
            {
                throw new ArgumentException("A permanent counter requires a valid counter type.", nameof(counterType));
            }
        }
        else if (counterType.HasValue)
        {
            throw new ArgumentException("Counter type only applies to permanent counters.", nameof(counterType));
        }

        if (hasIntegratedBeautyRoom == true && counterType != Projects.CounterType.WallCounter)
        {
            throw new ArgumentException("Only wall counters can have an integrated beauty room.", nameof(hasIntegratedBeautyRoom));
        }
    }

    private void ValidateExistingCounter(bool? hasExistingCounter,
        ExistingCounterLocationRelation? locationRelation, bool? requiresRemoval, string? removalReason)
    {
        if (ProjectType == ProjectType.TemporaryCounter)
        {
            if (hasExistingCounter.HasValue || locationRelation.HasValue || requiresRemoval.HasValue || !string.IsNullOrWhiteSpace(removalReason))
            {
                throw new ArgumentException("Existing counter conditions do not apply to temporary counters.");
            }
            return;
        }

        if (!hasExistingCounter.HasValue)
        {
            throw new ArgumentException("Specify whether an existing counter is present.", nameof(hasExistingCounter));
        }
        if (locationRelation.HasValue && !Enum.IsDefined(locationRelation.Value))
        {
            throw new ArgumentException("Invalid existing counter location relation.", nameof(locationRelation));
        }
        if (hasExistingCounter == false)
        {
            if (locationRelation.HasValue || requiresRemoval.HasValue || !string.IsNullOrWhiteSpace(removalReason))
            {
                throw new ArgumentException("Existing counter details only apply when a counter exists.");
            }
            return;
        }
        if (!locationRelation.HasValue || !requiresRemoval.HasValue)
        {
            throw new ArgumentException("An existing counter requires its location relation and removal decision.");
        }
        if (requiresRemoval == false)
        {
            RequiredText(removalReason, nameof(removalReason));
        }
    }

    private void RequireStatus(ProjectStatus status)
    {
        if (Status != status)
        {
            throw new InvalidOperationException($"This transition requires a {status} project.");
        }
    }

    private void Touch(Guid updatedByUserId)
    {
        RequireId(updatedByUserId, nameof(updatedByUserId));
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty value is required.", parameterName);
        }
        return value.Trim();
    }

    private static string? OptionalText(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }

    private static void ValidateOptionalId(Guid? value, string parameterName)
    {
        if (value.HasValue) RequireId(value.Value, parameterName);
    }

    private static void RequireOpeningDate(DateOnly openingDate)
    {
        if (openingDate == default)
        {
            throw new ArgumentException("Opening date is required.", nameof(openingDate));
        }
    }
}
