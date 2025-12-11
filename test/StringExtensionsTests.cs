// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

/* cSpell:ignore iosi Fapi Fbeta */

using GenerateTOC.Extensions;

namespace GenerateTOCTests;

/// <summary>
/// Tests for the StringExtensions class.
/// </summary>
public class StringExtensionsTests
{
    /// <summary>
    /// Gets a dataset for testing camel case to sentence case conversion.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name is extracted correctly from Markdown with matching titles in YAML and header.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name is extracted correctly from Markdown with different titles in YAML and header.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name is extracted correctly from Markdown with title only in header.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name extraction returns null when the title is missing.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name is extracted correctly from Markdown with single quotes in YAML.
    /// </summary>
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

    /// <summary>
    /// Tests that the resource name is extracted correctly from Markdown with no quotes in YAML.
    /// </summary>
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

    /// <summary>
    /// Tests that the doc type extracts correctly from various markdown samples.
    /// </summary>
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

    /// <summary>
    /// Tests that the doc type extraction returns null when the doc type is missing.
    /// </summary>
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

    /// <summary>
    /// Tests that the graph namespace is extracted correctly from Markdown.
    /// </summary>
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

    /// <summary>
    /// Tests that file paths are converted to TOC-relative paths correctly.
    /// </summary>
    [Fact]
    public void FilePathsConvertToTocRelative()
    {
        // Arrange
        var fullPath = "C:/Source/Repos/microsoft-graph-docs/api-reference/v1.0/resources/event.md"
            .Replace('/', Path.DirectorySeparatorChar);
        var relativePath = "../api/event-get.md"
            .Replace('/', Path.DirectorySeparatorChar);
        var relativePathWithAnchor = "../api/user-post-messages.md#request-2"
            .Replace('/', Path.DirectorySeparatorChar);

        // Act
        var convertedFullPath = fullPath.ToTocRelativePath();
        var convertedRelativePath = relativePath.ToTocRelativePath();
        var convertedRelativePathWithAnchor = relativePathWithAnchor.ToTocRelativePath();

        // Assert
        Assert.Equal("../../resources/event.md", convertedFullPath);
        Assert.Equal("../../api/event-get.md", convertedRelativePath);
        Assert.Equal("../../api/user-post-messages.md#request-2", convertedRelativePathWithAnchor);
    }

    /// <summary>
    /// Tests that relative URL paths do not get converted.
    /// </summary>
    [Fact]
    public void RelativeUrlPathsDoNotConvert()
    {
        // Arrange
        var betaRelativeUrl = "/graph/extensibility-overview?context=graph%2Fapi%2Fbeta&preserve-view=true";
        var v1RelativeUrl = "/graph/extensibility-overview?context=graph%2Fapi%2F1.0&preserve-view=true";

        // Act
        var convertedBetaPath = betaRelativeUrl.ToTocRelativePath();
        var convertedV1Path = v1RelativeUrl.ToTocRelativePath();

        // Assert
        Assert.Equal(betaRelativeUrl, convertedBetaPath);
        Assert.Equal(v1RelativeUrl, convertedV1Path);
    }

    /// <summary>
    /// Tests that camel case strings convert to sentence case correctly.
    /// </summary>
    /// <param name="camelCase">The camel case string to convert.</param>
    /// <param name="expectedSentenceCase">The expected sentence case result.</param>
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

    /// <summary>
    /// Tests that a relative path resolves correctly against a file path.
    /// </summary>
    [Fact]
    public void RelativePathResolvesCorrectly()
    {
        // Arrange
        var workingDirectory = Directory.GetCurrentDirectory();
        var resourceDocFile = Path.Combine(workingDirectory, "docs", "resources", "resource.md");
        var methodLinkPath = "../api/resource-get.md";
        var expectedFullPath = Path.Combine(workingDirectory, "docs", "api", "resource-get.md");

        // Act
        var fullPath = methodLinkPath.FullPathRelativeToFile(resourceDocFile);

        // Assert
        Assert.Equal(expectedFullPath, fullPath);
    }

    /// <summary>
    /// Tests that a path with an anchor trims correctly.
    /// </summary>
    [Fact]
    public void PathWithAnchorTrimsCorrectly()
    {
        // Arrange
        var filePathWithAnchor = "../api/resource-get.md#some-anchor";
        var urlWithAnchor = "/graph/overview#some-anchor";

        // Act
        var trimmedFilePath = filePathWithAnchor.TrimAnchor();
        var trimmedUrl = urlWithAnchor.TrimAnchor();

        // Assert
        Assert.Equal("../api/resource-get.md", trimmedFilePath);
        Assert.Equal("/graph/overview", trimmedUrl);
    }

    /// <summary>
    /// Tests that a path normalizes correctly for different OS styles.
    /// </summary>
    [Fact]
    public void PathNormalizesCorrectly()
    {
        // Arrange
        var windowsStylePath = "..\\api\\resource-get.md";
        var unixStylePath = "../api/resource-get.md";
        var expectedNormalizedPath = $"..{Path.DirectorySeparatorChar}api{Path.DirectorySeparatorChar}resource-get.md";

        // Act
        var normalizedWindowsPath = windowsStylePath.NormalizeFilePath();
        var normalizedUnixPath = unixStylePath.NormalizeFilePath();

        // Assert
        Assert.Equal(expectedNormalizedPath, normalizedWindowsPath);
        Assert.Equal(expectedNormalizedPath, normalizedUnixPath);
    }
}
