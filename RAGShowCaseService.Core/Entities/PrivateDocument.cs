namespace RAGShowCaseService.Core.Entities;

public class PrivateDocument
{
    public int Id { get; private set; }

    public string Name { get; private set; }

    public bool IsActive { get; private set; }

    public PrivateFileStatus Status { get; private set; }

    public PrivateDocument(string name)
    {
        Name = name;
        Status = PrivateFileStatus.Created;
        IsActive = true;
    }

    public void UpdateStatus(PrivateFileStatus newStatus)
    {
        Status = newStatus;
    }

    public void Deactivate()
    {
        IsActive = false;
    }
}

public enum PrivateFileStatus
{
    Created = 1,
    Uploaded = 2,
    Processed = 3,
    Failed = 4,
}

