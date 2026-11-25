namespace ProjectNest.Domain.Projects;

public class Project
{
    private readonly List<OpeningDateChange> openingDateHistory = new();
    private readonly List<MeasuredAreaChange> measuredAreaHistory = new();
    private readonly List<ProjectAssignment> responsibleDesignerAssignmentHistory = new();
    private readonly List<ProjectStatusChange> statusHistory = new();

    public Guid Id { get; private set; }
    public string ProjectNumber { get; private set; } = string.Empty;
    public Guid StoreId { get; private set; }
    public ProjectNature ProjectNature { get; private set; }
    public ProjectCategory ProjectCategory { get; private set; }
    public DateOnly OpeningDate { get; private set; }
    public decimal? InitialArea { get; private set; }
    public decimal? MeasuredArea { get; private set; }
    public string? Floor { get; private set; }
    public string? ShopNumber { get; private set; }
    public string? InitialRequirements { get; private set; }
    public Guid? ResponsibleDesignerId { get; private set; }
    public decimal BrandPaymentPercentage { get; private set; }
    public decimal MallPaymentPercentage { get; private set; }
    public string BrandNameSnapshot { get; private set; } = string.Empty;
    public string MallNameSnapshot { get; private set; } = string.Empty;
    public string? BrandContactName { get; private set; }
    public string? BrandContactPosition { get; private set; }
    public string? BrandContactPhone { get; private set; }
    public string? BrandContactEmail { get; private set; }
    public string? MallContactName { get; private set; }
    public string? MallContactPosition { get; private set; }
    public string? MallContactPhone { get; private set; }
    public string? MallContactEmail { get; private set; }
    public CounterType? CounterType { get; private set; }
    public bool? HasIntegratedBeautyRoom { get; private set; }
    public bool RequiresTemporaryCounter { get; private set; }
    public string? TemporaryCounterNotes { get; private set; }
    public bool? HasExistingCounter { get; private set; }
    public ExistingCounterLocationRelation? ExistingCounterLocationRelation { get; private set; }
    public bool RequiresExistingCounterRemoval { get; private set; }
    public string? ExistingCounterRemovalReason { get; private set; }
    public ProjectStatus Status { get; private set; }
    public Guid CreatedByUserId { get; private set; }
    public Guid UpdatedByUserId { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public IReadOnlyList<OpeningDateChange> OpeningDateHistory => openingDateHistory.AsReadOnly();
    public IReadOnlyList<MeasuredAreaChange> MeasuredAreaHistory => measuredAreaHistory.AsReadOnly();
    public IReadOnlyList<ProjectAssignment> ResponsibleDesignerAssignmentHistory => responsibleDesignerAssignmentHistory.AsReadOnly();
    public IReadOnlyList<ProjectStatusChange> StatusHistory => statusHistory.AsReadOnly();

    private Project()
    {
    }

    public Project(
        string projectNumber, Guid storeId, ProjectNature projectNature, ProjectCategory projectCategory,
        DateOnly openingDate, string brandNameSnapshot, string mallNameSnapshot, Guid createdByUserId,
        decimal brandPaymentPercentage, decimal mallPaymentPercentage,
        string? mallContactName = null, string? mallContactPhone = null,
        CounterType? counterType = null, bool? hasIntegratedBeautyRoom = null,
        bool requiresTemporaryCounter = false, string? temporaryCounterNotes = null,
        bool? hasExistingCounter = null,
        ExistingCounterLocationRelation? existingCounterLocationRelation = null,
        bool requiresExistingCounterRemoval = false, string? existingCounterRemovalReason = null,
        decimal? initialArea = null, decimal? measuredArea = null,
        string? floor = null, string? shopNumber = null, string? initialRequirements = null,
        Guid? responsibleDesignerId = null,
        string? brandContactName = null, string? brandContactPosition = null,
        string? brandContactPhone = null, string? brandContactEmail = null,
        string? mallContactPosition = null, string? mallContactEmail = null)
    {
        ProjectNumber = RequiredText(projectNumber, nameof(projectNumber));
        RequireId(storeId, nameof(storeId));
        RequireId(createdByUserId, nameof(createdByUserId));
        if (!Enum.IsDefined(projectNature))
        {
            throw new ArgumentException("Invalid project nature.", nameof(projectNature));
        }
        if (!Enum.IsDefined(projectCategory))
        {
            throw new ArgumentException("Invalid project category.", nameof(projectCategory));
        }
        RequireOpeningDate(openingDate);
        BrandNameSnapshot = RequiredText(brandNameSnapshot, nameof(brandNameSnapshot));
        MallNameSnapshot = RequiredText(mallNameSnapshot, nameof(mallNameSnapshot));
        ValidatePaymentShares(brandPaymentPercentage, mallPaymentPercentage);
        ValidateOptionalId(responsibleDesignerId, nameof(responsibleDesignerId));

        ProjectNature = projectNature;
        ProjectCategory = projectCategory;
        ValidateCounter(counterType, hasIntegratedBeautyRoom);
        ValidateTemporaryCounter(requiresTemporaryCounter, temporaryCounterNotes);
        ValidateExistingCounter(hasExistingCounter, existingCounterLocationRelation,
            requiresExistingCounterRemoval, existingCounterRemovalReason);

        Id = Guid.NewGuid();
        StoreId = storeId;
        OpeningDate = openingDate;
        InitialArea = initialArea;
        MeasuredArea = measuredArea;
        Floor = OptionalText(floor);
        ShopNumber = OptionalText(shopNumber);
        InitialRequirements = OptionalText(initialRequirements);
        ResponsibleDesignerId = responsibleDesignerId;
        BrandPaymentPercentage = brandPaymentPercentage;
        MallPaymentPercentage = mallPaymentPercentage;
        MallContactName = OptionalText(mallContactName);
        MallContactPhone = OptionalText(mallContactPhone);
        BrandContactName = OptionalText(brandContactName);
        BrandContactPosition = OptionalText(brandContactPosition);
        BrandContactPhone = OptionalText(brandContactPhone);
        BrandContactEmail = OptionalText(brandContactEmail);
        MallContactPosition = OptionalText(mallContactPosition);
        MallContactEmail = OptionalText(mallContactEmail);
        CounterType = counterType;
        HasIntegratedBeautyRoom = hasIntegratedBeautyRoom;
        RequiresTemporaryCounter = requiresTemporaryCounter;
        TemporaryCounterNotes = OptionalText(temporaryCounterNotes);
        HasExistingCounter = hasExistingCounter;
        ExistingCounterLocationRelation = existingCounterLocationRelation;
        RequiresExistingCounterRemoval = requiresExistingCounterRemoval;
        ExistingCounterRemovalReason = OptionalText(existingCounterRemovalReason);
        Status = ProjectStatus.Active;
        CreatedByUserId = createdByUserId;
        UpdatedByUserId = createdByUserId;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = CreatedAt;

        if (ResponsibleDesignerId.HasValue)
        {
            responsibleDesignerAssignmentHistory.Add(
                new ProjectAssignment(Id, ResponsibleDesignerId.Value, CreatedAt, createdByUserId));
        }
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

    public void ChangeMeasuredArea(decimal? measuredArea, Guid updatedByUserId, string reason)
    {
        RequireId(updatedByUserId, nameof(updatedByUserId));
        if (MeasuredArea == measuredArea) return;

        reason = RequiredText(reason, nameof(reason));
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

    public void AssignResponsibleDesigner(Guid responsibleDesignerId, Guid updatedByUserId)
    {
        RequireId(responsibleDesignerId, nameof(responsibleDesignerId));
        RequireId(updatedByUserId, nameof(updatedByUserId));
        if (ResponsibleDesignerId == responsibleDesignerId) return;

        var assignedAt = DateTime.UtcNow;
        var newAssignment = new ProjectAssignment(Id, responsibleDesignerId, assignedAt, updatedByUserId);
        var currentAssignment = responsibleDesignerAssignmentHistory
            .SingleOrDefault(assignment => assignment.EndedAt is null);

        if (ResponsibleDesignerId.HasValue)
        {
            if (currentAssignment is null || currentAssignment.DesignerUserId != ResponsibleDesignerId.Value)
            {
                throw new InvalidOperationException("The current responsible Designer must have an active assignment record.");
            }

            currentAssignment.End(assignedAt);
        }
        else if (currentAssignment is not null)
        {
            throw new InvalidOperationException("A project without a current responsible Designer cannot have an active assignment record.");
        }

        responsibleDesignerAssignmentHistory.Add(newAssignment);
        ResponsibleDesignerId = responsibleDesignerId;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = assignedAt;
    }

    public void UpdatePaymentShares(decimal brandPaymentPercentage, decimal mallPaymentPercentage, Guid updatedByUserId)
    {
        ValidatePaymentShares(brandPaymentPercentage, mallPaymentPercentage);
        Touch(updatedByUserId);
        BrandPaymentPercentage = brandPaymentPercentage;
        MallPaymentPercentage = mallPaymentPercentage;
    }

    public void UpdateBrandContact(string? name, string? position, string? phone, string? email, Guid updatedByUserId)
    {
        Touch(updatedByUserId);
        BrandContactName = OptionalText(name);
        BrandContactPosition = OptionalText(position);
        BrandContactPhone = OptionalText(phone);
        BrandContactEmail = OptionalText(email);
    }

    public void UpdateMallContact(string name, string? position, string phone, string? email, Guid updatedByUserId)
    {
        name = RequiredText(name, nameof(name));
        phone = RequiredText(phone, nameof(phone));
        Touch(updatedByUserId);
        MallContactName = name;
        MallContactPhone = phone;
        MallContactPosition = OptionalText(position);
        MallContactEmail = OptionalText(email);
    }

    public void UpdateCounter(CounterType? counterType, bool? hasIntegratedBeautyRoom, Guid updatedByUserId)
    {
        ValidateCounter(counterType, hasIntegratedBeautyRoom);
        Touch(updatedByUserId);
        CounterType = counterType;
        HasIntegratedBeautyRoom = hasIntegratedBeautyRoom;
    }

    public void UpdateTemporaryCounter(bool requiresTemporaryCounter, string? notes, Guid updatedByUserId)
    {
        ValidateTemporaryCounter(requiresTemporaryCounter, notes);
        Touch(updatedByUserId);
        RequiresTemporaryCounter = requiresTemporaryCounter;
        TemporaryCounterNotes = OptionalText(notes);
    }

    public void UpdateExistingCounter(bool? hasExistingCounter,
        ExistingCounterLocationRelation? locationRelation, bool requiresRemoval,
        string? removalReason, Guid updatedByUserId)
    {
        ValidateExistingCounter(hasExistingCounter, locationRelation, requiresRemoval, removalReason);
        Touch(updatedByUserId);
        HasExistingCounter = hasExistingCounter;
        ExistingCounterLocationRelation = locationRelation;
        RequiresExistingCounterRemoval = requiresRemoval;
        ExistingCounterRemovalReason = OptionalText(removalReason);
    }

    public void Pause(Guid updatedByUserId, string reason)
    {
        RequireStatus(ProjectStatus.Active);
        ChangeStatus(ProjectStatus.Paused, updatedByUserId, RequiredText(reason, nameof(reason)));
    }

    public void Resume(Guid updatedByUserId, string? reason = null)
    {
        RequireStatus(ProjectStatus.Paused);
        ChangeStatus(ProjectStatus.Active, updatedByUserId, OptionalText(reason));
    }

    public void Cancel(Guid updatedByUserId, string reason)
    {
        if (Status != ProjectStatus.Active && Status != ProjectStatus.Paused)
        {
            throw new InvalidOperationException("Only active or paused projects can be cancelled.");
        }
        ChangeStatus(ProjectStatus.Cancelled, updatedByUserId, RequiredText(reason, nameof(reason)));
    }

    public void Complete(Guid updatedByUserId, string? reason = null)
    {
        RequireStatus(ProjectStatus.Active);
        ChangeStatus(ProjectStatus.Completed, updatedByUserId, OptionalText(reason));
    }

    private void ChangeStatus(ProjectStatus newStatus, Guid updatedByUserId, string? reason)
    {
        RequireId(updatedByUserId, nameof(updatedByUserId));
        var changedAt = DateTime.UtcNow;
        statusHistory.Add(new ProjectStatusChange(Id, Status, newStatus, reason, changedAt, updatedByUserId));
        Status = newStatus;
        UpdatedByUserId = updatedByUserId;
        UpdatedAt = changedAt;
    }

    private void ValidateCounter(CounterType? counterType, bool? hasIntegratedBeautyRoom)
    {
        if (ProjectCategory == global::ProjectNest.Domain.Projects.ProjectCategory.PermanentCounter)
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

        if (hasIntegratedBeautyRoom == true &&
            (ProjectCategory != global::ProjectNest.Domain.Projects.ProjectCategory.PermanentCounter || counterType != Projects.CounterType.WallCounter))
        {
            throw new ArgumentException("Only wall counters can have an integrated beauty room.", nameof(hasIntegratedBeautyRoom));
        }
    }

    private void ValidateTemporaryCounter(bool requiresTemporaryCounter, string? notes)
    {
        if (ProjectCategory != global::ProjectNest.Domain.Projects.ProjectCategory.PermanentCounter &&
            (requiresTemporaryCounter || !string.IsNullOrWhiteSpace(notes)))
        {
            throw new ArgumentException("Temporary counter configuration only applies to permanent counters.");
        }
    }

    private static void ValidateExistingCounter(bool? hasExistingCounter,
        ExistingCounterLocationRelation? locationRelation, bool requiresRemoval, string? removalReason)
    {
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
            if (locationRelation.HasValue || requiresRemoval || !string.IsNullOrWhiteSpace(removalReason))
            {
                throw new ArgumentException("Existing counter details only apply when a counter exists.");
            }
            return;
        }
        if (!locationRelation.HasValue)
        {
            throw new ArgumentException("An existing counter requires its location relation.");
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

    private static void ValidatePaymentShares(decimal brandPercentage, decimal mallPercentage)
    {
        if (brandPercentage is < 0 or > 100 || mallPercentage is < 0 or > 100 ||
            brandPercentage + mallPercentage != 100)
        {
            throw new ArgumentException("Brand and Mall payment percentages must each be between 0 and 100 and total 100%.");
        }
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
