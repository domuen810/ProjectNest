namespace ProjectNest.Domain.Files;

public class StoredFile
{
    public Guid Id { get; private set; }
    public string OriginalFileName { get; private set; }
    public string ContentType { get; private set; }
    public string FileExtension { get; private set; }
    public long FileSize { get; private set; }
    public string StorageKey { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public Guid UploadedById { get; private set; }

    public StoredFile(string originalFileName, string contentType, string fileExtension,
        long fileSize, string storageKey, Guid uploadedById)
    {
        OriginalFileName = RequiredText(originalFileName, nameof(originalFileName));
        ContentType = RequiredText(contentType, nameof(contentType));
        FileExtension = RequiredText(fileExtension, nameof(fileExtension));
        if (fileSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(fileSize), "File size cannot be negative.");
        }
        StorageKey = RequiredText(storageKey, nameof(storageKey));
        RequireId(uploadedById, nameof(uploadedById));

        Id = Guid.NewGuid();
        FileSize = fileSize;
        UploadedAt = DateTime.UtcNow;
        UploadedById = uploadedById;
    }

    private static string RequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("A non-empty value is required.", parameterName);
        }
        return value.Trim();
    }

    private static void RequireId(Guid value, string parameterName)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("A non-empty identifier is required.", parameterName);
        }
    }
}
