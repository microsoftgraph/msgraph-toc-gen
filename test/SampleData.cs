// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System.Text;
using System.Xml.Linq;
using GenerateTOC.Generation;

namespace GenerateTOCTests;

public static class SampleData
{
    private static readonly string csdl = @"<?xml version=""1.0"" encoding=""utf-8""?>
<edmx:Edmx Version=""4.0"" xmlns:ags=""http://aggregator.microsoft.com/internal"" xmlns:edmx=""http://docs.oasis-open.org/odata/ns/edmx"">
  <edmx:DataServices>
    <Schema Alias=""self"" Namespace=""microsoft.test"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
      <ComplexType Name=""identity"" OpenType=""true"">
        <Property Name=""displayName"" Type=""Edm.String"" />
        <Property Name=""id"" Type=""Edm.String"" />
      </ComplexType>

      <ComplexType Name=""identitySet"" OpenType=""true"">
        <Property Name=""application"" Type=""self.identity"" />
        <Property Name=""device"" Type=""self.identity"" />
        <Property Name=""user"" Type=""self.identity"" />
      </ComplexType>

      <EntityType Name=""testItemRequest"" ags:IsOwner=""true"" ags:IsHidden=""true"">
        <Property Name=""createdDateTime"" Type=""Edm.DateTimeOffset""/>
        <Property Name=""approver"" Type=""self.testIdentitySet"" />
        <Property Name=""reassignedFrom"" Type=""self.testIdentitySet"" />
        <Property Name=""isReassigned"" Type=""Edm.Boolean"" />
      </EntityType>

      <EntityType Name=""testItemResponse"" ags:IsOwner=""true"" ags:IsHidden=""true"">
        <Property Name=""createdDateTime"" Type=""Edm.DateTimeOffset""/>
        <Property Name=""createdBy"" Type=""self.testIdentitySet""/>
        <Property Name=""comments"" Type=""Edm.String""/>
        <Property Name=""response"" Type=""Edm.String""/>
        <Property Name=""owners"" Type=""Collection(self.testIdentitySet)""/>
      </EntityType>

      <EntityType Name=""testItem"" ags:IsOwner=""true"" ags:IsHidden=""true"">
        <Key>
          <PropertyRef Name=""id"" />
        </Key>
        <Property Name=""id"" Type=""Edm.String"" Nullable=""false"" />
        <Property Name=""displayName"" Type=""Edm.String""/>
        <Property Name=""description"" Type=""Edm.String""/>
      </EntityType>

      <EntityType Name=""testSolution"" ags:IsOwner=""true"" ags:IsHidden=""true"">
          <Property Name=""provisioningStatus"" Type=""self.provisionState""/>
          <NavigationProperty Name=""testItems"" Type=""Collection(self.testItem)"" ContainsTarget=""true"" />
      </EntityType>

      <EntityType Name=""solutionsRoot"" ags:IsOwner=""false"" ags:IsOwnerlessSingleton=""true"">
          <NavigationProperty Name=""test"" Type=""self.testSolution"" ContainsTarget=""true"" ags:IsHidden=""true"" ags:AddressContainsEntitySetSegment=""true"" ags:AddressUrl=""https://contoso.com/beta/solutions/test""/>
      </EntityType>

      <EntityContainer Name=""testContainer"" ags:OmitOperationNamespace=""true"">
        <Singleton Name=""solutions"" Type=""self.solutionsRoot""/>
      </EntityContainer>

      <Action Name=""cancel"" IsBound=""true"" ags:IsHidden=""true"" ags:OmitOperationNamespace=""true"">
        <Parameter Name=""bindingParameter"" Type=""self.testItem""/>
      </Action>

      <Action Name=""provision"" IsBound=""true"" ags:IsHidden=""true"" ags:OmitOperationNamespace=""true"">
        <Parameter Name=""bindingParameter"" Type=""self.testSolution"" />
      </Action>
    </Schema>
    <Schema Namespace=""microsoft.test.foo"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
      <EntityType Name=""foo"" ags:IsOwner=""true"">
          <NavigationProperty Name=""test"" Type=""self.testSolution"" ContainsTarget=""true"" ags:IsHidden=""true"" ags:AddressContainsEntitySetSegment=""true"" ags:AddressUrl=""https://contoso.com/beta/solutions/test""/>
      </EntityType>
    </Schema>
  </edmx:DataServices>
</edmx:Edmx>";

    private static readonly string schemaElement = @"<Schema Namespace=""microsoft.graph"" xmlns=""http://docs.oasis-open.org/odata/ns/edm"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
      <EntityType Name=""canteen"" ags:IsOwner=""true"">
          <Property Name=""name"" Type=""Edm.String""/>
      </EntityType>
      <EntityType Name=""user"" ags:IsOwner=""true"">
          <Property Name=""name"" Type=""Edm.String""/>
      </EntityType>
    </Schema>";

    private static readonly string entityElement = @"<EntityType Name=""canteen"" ags:IsOwner=""true"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
          <Property Name=""name"" Type=""Edm.String""/>
      </EntityType>";

    private static readonly string hiddenEntityElement = @"<EntityType Name=""canteen"" ags:IsOwner=""true"" ags:IsHidden=""true"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
          <Property Name=""name"" Type=""Edm.String""/>
      </EntityType>";

    private static readonly string entityWithBaseTypeElement = @"<EntityType Name=""canteen"" BaseType=""microsoft.graph.entity"" ags:IsOwner=""true"" xmlns:ags=""http://aggregator.microsoft.com/internal"">
          <Property Name=""name"" Type=""Edm.String""/>
      </EntityType>";

    public static readonly string ResourceMarkdownWithTitleInYamlAndHeader = @"---
title: ""membersDeletedEventMessageDetail resource type""
description: ""Represents the details of an event message about members deleted.""
author: ""jasonjoh""
ms.localizationpriority: medium
ms.prod: ""microsoft-teams""
doc_type: ""resourcePageType""
---

# membersDeletedEventMessageDetail resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.

## Methods

| Method | Return Type | Description |
|:-------|:------------|:------------|
|[List mailFolders](../api/user-list-mailfolders.md) | [mailFolder](mailfolder.md) collection|Get all the mail folders in the specified user's mailbox, including any mail search folders.|
|[Get mailFolder](../api/mailfolder-get.md) | [mailFolder](mailfolder.md) |Read properties and relationships of mailFolder object.|
|[Create mailFolder](../api/user-post-mailfolders.md) |[mailFolder](mailfolder.md)| Create a new mail folder in the root folder of the user's mailbox.|
|[List childFolders](../api/mailfolder-list-childfolders.md) |[mailFolder](mailfolder.md) collection| Get the folder collection under the specified folder. You can use the `.../me/MailFolders` shortcut to get the top-level folder collection and navigate to another folder.|
|[Create childFolder](../api/mailfolder-post-childfolders.md) |[mailFolder](mailfolder.md)| Create a new mailFolder under the current one by posting to the childFolders collection.|
|[Create Message](../api/mailfolder-post-messages.md) |[message](message.md)| Create a new message in the current mailFolder by posting to the messages collection.|
|[List messages](../api/mailfolder-list-messages.md) |[message](message.md) collection| Get all the messages in the signed-in user's mailbox, or those messages in a specified folder in the mailbox.|
|[Update](../api/mailfolder-update.md) | [mailFolder](mailfolder.md)|Update the specified mailFolder object. |
|[Delete](../api/mailfolder-delete.md) | None |Delete the specified mailFolder object. |
|[copy](../api/mailfolder-copy.md)|[mailFolder](mailfolder.md)|Copy a mailFolder and its contents to another mailFolder.|
|[delta](../api/mailfolder-delta.md)|[mailFolder](mailfolder.md) collection|Get a set of mail folders that have been added, deleted, or removed from the user's mailbox.|
|[move](../api/mailfolder-move.md)|[mailFolder](mailfolder.md)|Move a mailFolder and its contents to another mailFolder.|
|**Extended properties**| | |
|[Create single-value extended property](../api/singlevaluelegacyextendedproperty-post-singlevalueextendedproperties.md) |[mailFolder](mailfolder.md)  |Create one or more single-value extended properties in a new or existing mailFolder.   |
|[Get mailFolder with single-value extended property](../api/singlevaluelegacyextendedproperty-get.md)  | [mailFolder](mailfolder.md) | Get mailFolders that contain a single-value extended property by using `$expand` or `$filter`. |
|[Create multi-value extended property](../api/multivaluelegacyextendedproperty-post-multivalueextendedproperties.md) | [mailFolder](mailfolder.md) | Create one or more multi-value extended properties in a new or existing mailFolder.  |
|[Get mailFolder with multi-value extended property](../api/multivaluelegacyextendedproperty-get.md)  | [mailFolder](mailfolder.md) | Get a mailFolder that contains a multi-value extended property by using `$expand`. |";

    public static readonly string ResourceMarkdownWithDifferentTitlesInYamlAndHeader = @"---
title: ""membersDeletedEventMessageDetail resource type""
description: ""Represents the details of an event message about members deleted.""
author: ""jasonjoh""
ms.localizationpriority: medium
ms.prod: ""microsoft-teams""
doc_type: ""resourcePageType""
---

# membersDeletedEvent resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithTitleOnlyInHeader = @"---
description: ""Represents the details of an event message about members deleted.""
author: ""jasonjoh""
ms.localizationpriority: medium
ms.prod: ""microsoft-teams""
doc_type: ""resourcePageType""
---

# membersDeletedEventMessageDetail resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithoutTitle = @"---
description: ""Represents the details of an event message about members deleted.""
author: ""jasonjoh""
ms.localizationpriority: medium
ms.prod: ""microsoft-teams""
doc_type: ""resourcePageType""
---

# membersDeletedEventMessageDetail

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithSingleQuotesInYaml = @"---
title: 'membersDeletedEventMessageDetail resource type'
description: 'Represents the details of an event message about members deleted.'
author: 'jasonjoh'
ms.localizationpriority: 'medium'
ms.prod: 'microsoft-teams'
doc_type: 'resourcePageType'
---

# membersDeletedEvent resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithoutDoubleQuotesInYaml = @"---
title: membersDeletedEventMessageDetail resource type
description: Represents the details of an event message about members deleted.
author: jasonjoh
ms.localizationpriority: medium
ms.prod: microsoft-teams
doc_type: resourcePageType
---

# membersDeletedEvent resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithWrongDocType = @"---
title: membersDeletedEventMessageDetail resource type
description: Represents the details of an event message about members deleted.
author: jasonjoh
ms.localizationpriority: medium
ms.prod: microsoft-teams
doc_type: enumPageType
---

# membersDeletedEvent resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string ResourceMarkdownWithNoDocType = @"---
title: membersDeletedEventMessageDetail resource type
description: Represents the details of an event message about members deleted.
author: jasonjoh
ms.localizationpriority: medium
ms.prod: microsoft-teams
---

# membersDeletedEvent resource type

Namespace: microsoft.graph

[!INCLUDE [beta-disclaimer](../../includes/beta-disclaimer.md)]

Represents the details of an event message about members deleted.
This message is generated when members are removed from a chat, a channel, or a team.";

    public static readonly string YamlHeaderWithTocTitleNoQuotes = @"---
title: m365AppsInstallationOptions resource type
description: Represents the tenant-level installation options for Microsoft 365 apps.
ms.localizationpriority: medium
doc_type: resourcePageType
ms.subservice: reports
author: yan-git
toc.title: Installation options
---";

    public static readonly string YamlHeaderWithTocTitleDoubleQuotes = @"---
title: m365AppsInstallationOptions resource type
description: Represents the tenant-level installation options for Microsoft 365 apps.
ms.localizationpriority: medium
doc_type: resourcePageType
ms.subservice: reports
author: yan-git
toc.title: ""Jason's installation options""
---";

    public static readonly string YamlHeaderWithTocTitleSingleQuotes = @"---
title: m365AppsInstallationOptions resource type
description: Represents the tenant-level installation options for Microsoft 365 apps.
ms.localizationpriority: medium
doc_type: resourcePageType
ms.subservice: reports
author: yan-git
toc.title: '""Special"" installation options'
---";

    public static XDocument GetSampleCsdl()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(csdl));
        return XDocument.Load(stream);
    }

    public static XElement GetSampleSchemaElement()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(schemaElement));
        return XElement.Load(stream);
    }

    public static XElement GetSampleEntityElement()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(entityElement));
        return XElement.Load(stream);
    }

    public static XElement GetSampleHiddenEntityElement()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(hiddenEntityElement));
        return XElement.Load(stream);
    }

    public static XElement GetSampleEntityWithBaseTypeElement()
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(entityWithBaseTypeElement));
        return XElement.Load(stream);
    }

    public static readonly List<TocTermOverride> TermOverrides = [
        new() { Term = "ios", Override = "iOS" },
        new() { Term = "ipad", Override = "iPad" },
        new() { Term = "macos", Override = "macOS" },
        new() { Term = "mac os", Override = "macOS" },
        new() { Term = "macos office suite", Override = "macOS Office Suite" },
        new() { Term = "mac os office suite", Override = "macOS Office Suite" },
        new() { Term = "lob", Override = "LOB" },
        new() { Term = "vpp", Override = "VPP" },
        new() { Term = "e book", Override = "eBook" },
        new() { Term = "android", Override = "Android" },
        new() { Term = "microsoft", Override = "Microsoft" },
        new() { Term = "microsoft defender", Override = "Microsoft Defender" },
        new() { Term = "microsoft edge", Override = "Microsoft Edge" },
        new() { Term = "microsoft store", Override = "Microsoft Store" },
        new() { Term = "windows app x", Override = "Windows AppX" },
        new() { Term = "windows mobile", Override = "Windows Mobile" },
        new() { Term = "msi", Override = "MSI" },
        new() { Term = "o auth", Override = "OAuth" },
        new() { Term = "iosi pad", Override = "iOS iPad" },
        new() { Term = "os", Override = "OS" },
        new() { Term = "cors", Override = "CORS" },
        new() { Term = "v 2", Override = "v2" },
        new() { Term = "cloud pc", Override = "Cloud PC" },
        new() { Term = "on premises", Override = "on-premises", CaseSensitive = true },
        new() { Term = "On premises", Override = "On-premises", CaseSensitive = true },
    ];
}
