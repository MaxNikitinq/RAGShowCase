using RAGShowCaseService.Core.Entities;

namespace RAGShowCaseService.Tests.Core.Entities;

public class PrivateDocumentTests
{
    [Fact]
    public void Constructor_ValidName_SetsInitialState()
    {
        // Arrange
        const string name = "report.pdf";

        // Act
        var document = new PrivateDocument(name);

        // Assert
        Assert.Equal(name, document.Name);
        Assert.Equal(PrivateFileStatus.Created, document.Status);
        Assert.True(document.IsActive);
    }

    [Fact]
    public void UpdateStatus_NewStatus_UpdatesStatus()
    {
        // Arrange
        var document = new PrivateDocument("report.pdf");

        // Act
        document.UpdateStatus(PrivateFileStatus.Uploaded);

        // Assert
        Assert.Equal(PrivateFileStatus.Uploaded, document.Status);
    }

    [Fact]
    public void Deactivate_Always_SetsIsActiveToFalse()
    {
        // Arrange
        var document = new PrivateDocument("report.pdf");

        // Act
        document.Deactivate();

        // Assert
        Assert.False(document.IsActive);
    }
}
