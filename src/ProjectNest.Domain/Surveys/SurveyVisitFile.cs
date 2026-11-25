using ProjectNest.Domain.Files;

namespace ProjectNest.Domain.Surveys;

public class SurveyVisitFile
{
    public Guid Id { get; private set; }
    public Guid SurveyVisitId { get; private set; }
    public Guid StoredFileId { get; private set; }
    public StoredFile StoredFile { get; private set; } = null!;
    public SurveyFileCategory Category { get; private set; }

    private SurveyVisitFile()
    {
    }

    internal SurveyVisitFile(Guid surveyVisitId, StoredFile storedFile, SurveyFileCategory category)
    {
        if (surveyVisitId == Guid.Empty) throw new ArgumentException("Survey visit is required.", nameof(surveyVisitId));
        ArgumentNullException.ThrowIfNull(storedFile);
        if (storedFile.Id == Guid.Empty) throw new ArgumentException("Stored file is required.", nameof(storedFile));
        if (!Enum.IsDefined(category)) throw new ArgumentException("Invalid survey file category.", nameof(category));

        Id = Guid.NewGuid();
        SurveyVisitId = surveyVisitId;
        StoredFileId = storedFile.Id;
        StoredFile = storedFile;
        Category = category;
    }
}
