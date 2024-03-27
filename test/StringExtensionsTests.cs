// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using GenerateTOC.Extensions;

namespace GenerateTOCTests;

public class StringExtensionsTests
{
    [Fact]
    public void ResourceNameExtractsFromMarkdownWithMatchingTitles()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithTitleInYamlAndHeader;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.NotNull(resourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceName);
    }

    [Fact]
    public void ResourceNameExtractsYamlOverHeader()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithDifferentTitlesInYamlAndHeader;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.NotNull(resourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceName);
    }

    [Fact]
    public void ResourceNameExtractsFromHeaderWhenMissingInYaml()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithTitleOnlyInHeader;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.NotNull(resourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceName);
    }

    [Fact]
    public void ResourceNameExtractReturnsNullOnMissingTitle()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithoutTitle;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.Null(resourceName);
    }

    [Fact]
    public void ResourceNameExtractsWithSingleQuotesInYaml()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithSingleQuotesInYaml;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.NotNull(resourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceName);
    }

    [Fact]
    public void ResourceNameExtractsWithNoQuotesInYaml()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithoutDoubleQuotesInYaml;

        // Act
        var resourceName = markdown.ExtractResourceName();

        // Assert
        Assert.NotNull(resourceName);
        Assert.Equal("membersDeletedEventMessageDetail", resourceName);
    }

    [Fact]
    public void DocTypeExtracts()
    {
        // Arrange
        var resourceMarkdown = SampleData.ResourceMarkdownWithTitleInYamlAndHeader;
        var nonResourceMarkdown = SampleData.ResourceMarkdownWithWrongDocType;

        // Act
        var resourceDocType = resourceMarkdown.ExtractDocType();
        var nonResourceDocType = nonResourceMarkdown.ExtractDocType();

        // Assert
        Assert.NotNull(resourceDocType);
        Assert.Equal("resourcePageType", resourceDocType);
        Assert.NotNull(nonResourceDocType);
        Assert.NotEqual("resourcePageType", nonResourceDocType);
    }

    [Fact]
    public void MissingDocTypeExtractsNull()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithNoDocType;

        // Act
        var docType = markdown.ExtractDocType();

        // Assert
        Assert.Null(docType);
    }

    [Fact]
    public void GraphNameSpaceExtracts()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithTitleInYamlAndHeader;

        // Act
        var graphNameSpace = markdown.ExtractNamespace();

        // Assert
        Assert.NotNull(graphNameSpace);
        Assert.Equal("microsoft.graph", graphNameSpace);
    }

    [Fact]
    public void FilePathsConvertToTocRelative()
    {
        // Arrange
        var fullPath = "C:/Source/Repos/microsoft-graph-docs/api-reference/v1.0/resources/event.md"
            .Replace('/', Path.DirectorySeparatorChar);
        var relativePath = "../api/event-get.md"
            .Replace('/', Path.DirectorySeparatorChar);

        // Act
        var convertedFullPath = fullPath.ToTocRelativePath();
        var convertedRelativePath = relativePath.ToTocRelativePath();

        // Assert
        Assert.Equal("../../resources/event.md", convertedFullPath);
        Assert.Equal("../../api/event-get.md", convertedRelativePath);
    }

    [Fact]
    public void TocTitleWithNoQuotesExtracts()
    {
        // Arrange
        var markdown = SampleData.YamlHeaderWithTocTitleNoQuotes;

        // Act
        var tocTitle = markdown.ExtractTocTitle();

        // Assert
        Assert.Equal("Installation options", tocTitle);
    }

    [Fact]
    public void TocTitleWithDoubleQuotesExtracts()
    {
        // Arrange
        var markdown = SampleData.YamlHeaderWithTocTitleDoubleQuotes;

        // Act
        var tocTitle = markdown.ExtractTocTitle();

        // Assert
        Assert.Equal("Jason's installation options", tocTitle);
    }

    [Fact]
    public void TocTitleWithSingleQuotesExtracts()
    {
        // Arrange
        var markdown = SampleData.YamlHeaderWithTocTitleSingleQuotes;

        // Act
        var tocTitle = markdown.ExtractTocTitle();

        // Assert
        Assert.Equal("\"Special\" installation options", tocTitle);
    }

    [Fact]
    public void NoTocTitleExtractsAsNull()
    {
        // Arrange
        var markdown = SampleData.ResourceMarkdownWithTitleOnlyInHeader;

        // Act
        var tocTitle = markdown.ExtractTocTitle();

        // Assert
        Assert.Null(tocTitle);
    }

    public static TheoryData<string, string> CamelCaseData => new()
    {
        { "application", "Application" },
        { "servicePrincipal", "Service principal" },
        { "openTypeExtension", "Open type extension" },
        { "oAuth2PermissionGrant", "OAuth 2 permission grant" },
        { "windowsMobileMSI", "Windows Mobile MSI" },
        { "managedIOSLobApp", "Managed iOS LOB app" },
        { "managedEBookAssignment", "Managed eBook assignment" },
        { "iosDeviceType", "iOS device type" },
        { "windowsAppX", "Windows AppX" },
        { "managedAndroidLobApp", "Managed Android LOB app" },
        { "iosiPadOSWebClip", "iOS iPad OS web clip" },
        { "iosVppApp", "iOS VPP app" },
        { "microsoftEdgeChannel", "Microsoft Edge channel" },
        { "macOSMicrosoftDefenderApp", "macOS Microsoft Defender app" },
        { "microsoftStoreForBusinessApp", "Microsoft Store for business app" },
        { "macOSOfficeSuiteApp", "macOS Office Suite app" },
        { "corsConfiguration_v2", "CORS configuration v2" },
        { "cloudPcOnPremisesConnectionHealthCheck", "Cloud PC on-premises connection health check" },
        { "onPremisesDirectorySynchronization", "On-premises directory synchronization" },
    };

    [Theory]
    [MemberData(nameof(CamelCaseData))]
    public void CamelCaseStringsConvertToSentenceCase(string camelCase, string expectedSentenceCase)
    {
        // Arrange
        StringExtensions.Initialize(SampleData.TermOverrides);

        // Act
        var sentenceCase = camelCase.SplitCamelCaseToSentenceCase();

        // Assert
        Assert.Equal(expectedSentenceCase, sentenceCase);
    }
}
