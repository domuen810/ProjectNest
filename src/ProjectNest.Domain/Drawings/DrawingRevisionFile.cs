using ProjectNest.Domain.Files;

namespace ProjectNest.Domain.Drawings;

public class DrawingRevisionFile
{
    public Guid Id { get; private set; }
    public Guid DrawingRevisionId { get; private set; }
    public Guid StoredFileId { get; private set; }
    public StoredFile StoredFile { get; private set; } = null!;
    public DrawingRevisionFileRole FileRole { get; private set; }

    private DrawingRevisionFile()
    {
    }

    internal DrawingRevisionFile(Guid drawingRevisionId, StoredFile storedFile,
        DrawingRevisionFileRole fileRole)
    {
        if (drawingRevisionId == Guid.Empty)
        {
            throw new ArgumentException("Drawing revision is required.", nameof(drawingRevisionId));
        }
        ArgumentNullException.ThrowIfNull(storedFile);
        if (storedFile.Id == Guid.Empty)
        {
            throw new ArgumentException("Stored file is required.", nameof(storedFile));
        }
        if (!Enum.IsDefined(fileRole))
        {
            throw new ArgumentException("Invalid drawing revision file role.", nameof(fileRole));
        }

        Id = Guid.NewGuid();
        DrawingRevisionId = drawingRevisionId;
        StoredFileId = storedFile.Id;
        StoredFile = storedFile;
        FileRole = fileRole;
    }
}
