// Copyright 2022 - 2023 GZSkins, Inc. All rights reserved.
// Licensed under the Mozilla Public License, Version 2.0 (the "License.txt").
//
// This Source Code Form is subject to the terms of the Mozilla Public
// License, v. 2.0. If a copy of the MPL was not distributed with this
// file, You can obtain one at https://mozilla.org/MPL/2.0/.

namespace GZSkinsX.Uwp.IO.Extensions;

/// <summary>
/// 
/// </summary>
public enum WindowCoreProperties
{
    /// <summary>
    /// 指示获取会话的哈希值。
    /// </summary>
    System_AcquisitionID,
    /// <summary>
    /// 
    /// </summary>
    System_ApplicationDefinedProperties,
    /// <summary>
    /// 创建此文件或项的应用程序的名称。 请勿使用版本号来标识应用程序的特定版本。
    /// </summary>
    System_ApplicationName,
    /// <summary>
    /// 应用容器的标记。 文件最后一个编写器确定的区域标识符。
    /// </summary>
    System_AppZoneIdentifier,
    /// <summary>
    /// 表示文档的作者或作者。
    /// </summary>
    System_Author,
    /// <summary>
    /// 
    /// </summary>
    System_CachedFileUpdaterContentIdForConflictResolution,
    /// <summary>
    /// 
    /// </summary>
    System_CachedFileUpdaterContentIdForStream,
    /// <summary>
    /// 总存储空间量（以字节表示）。
    /// </summary>
    System_Capacity,
    /// <summary>
    /// 已弃用。 可分配给文档或文件等项的类别。
    /// </summary>
    System_Category,
    /// <summary>
    /// 附加到文件的注释，通常由用户添加。
    /// </summary>
    System_Comment,
    /// <summary>
    /// 公司或发布者。
    /// </summary>
    System_Company,
    /// <summary>
    /// 项或文件所在的计算机的名称。
    /// </summary>
    System_ComputerName,
    /// <summary>
    /// 项中内容类型的列表。
    /// </summary>
    System_ContainedItems,
    /// <summary>
    /// 
    /// </summary>
    System_ContentStatus,
    /// <summary>
    /// 
    /// </summary>
    System_ContentType,
    /// <summary>
    /// 作为字符串存储的版权信息。
    /// </summary>
    System_Copyright,
    /// <summary>
    /// 创建此文件的应用程序的 AppId。
    /// </summary>
    System_CreatorAppId,
    /// <summary>
    /// 提供影响 UI 控件的行为的选项，这些控件使用 <seealso cref="System_CreatorAppId">System.CreatorAppId</seealso> 中指定的应用启动文件。
    /// </summary>
    System_CreatorOpenWithUIOptions,
    /// <summary>
    /// 数据对象格式。 一个字符串值，该值是剪贴板格式名称。
    /// </summary>
    System_DataObjectFormat,
    /// <summary>
    /// 指示上次访问项目的时间。 索引服务友好名称为“access”。
    /// </summary>
    System_DateAccessed,
    /// <summary>
    /// 文件或媒体的获取日期。
    /// </summary>
    System_DateAcquired,
    /// <summary>
    /// 文件项上次存档的日期。
    /// </summary>
    System_DateArchived,
    /// <summary>
    /// 
    /// </summary>
    System_DateCompleted,
    /// <summary>
    /// 项在当前所在的文件系统上创建的日期和时间。
    /// </summary>
    System_DateCreated,
    /// <summary>
    /// 文件导入到专用应用程序数据库的日期和时间。
    /// </summary>
    System_DateImported,
    /// <summary>
    /// 上次修改项的日期和时间。 索引服务友好名称为“write”。
    /// </summary>
    System_DateModified,
    /// <summary>
    /// 帮助显示为图标，无论位置是否是库所有者和非所有者的默认保存位置
    /// </summary>
    System_DefaultSaveLocationDisplay,
    /// <summary>
    /// 
    /// </summary>
    System_DueDate,
    /// <summary>
    /// 
    /// </summary>
    System_EndDate,
    /// <summary>
    /// 未存储在项本身中的属性，其中属性采用包含 SERIALIZEDPROPSTORAGE 的流的形式。
    /// </summary>
    System_ExpandoProperties,
    /// <summary>
    /// 
    /// </summary>
    System_FileAllocationSize,
    /// <summary>
    /// 项的属性。 这些值等效于 <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/minwinbase/ns-minwinbase-win32_find_dataa">WIN32_FIND_DATA</see> 结构的 dwFileAttributes 成员中识别的值。
    /// </summary>
    System_FileAttributes,
    /// <summary>
    /// 
    /// </summary>
    System_FileCount,
    /// <summary>
    /// 文件的用户友好说明。
    /// </summary>
    System_FileDescription,
    /// <summary>
    /// 标识基于文件的项的文件扩展名，包括前导期。
    /// </summary>
    System_FileExtension,
    /// <summary>
    /// 唯一的文件 ID，也称为文件引用号。
    /// </summary>
    System_FileFRN,
    /// <summary>
    /// 文件名，包括其扩展名。
    /// </summary>
    System_FileName,
    /// <summary>
    /// Null 表示正常情况下， (文件脱机) 可用。 部分情况仅适用于某些内容可能脱机使用的文件夹，有些文件夹可能不可用。
    /// </summary>
    System_FileOfflineAvailabilityStatus,
    /// <summary>
    /// 文件的所有者，如文件系统所称。
    /// </summary>
    System_FileOwner,
    /// <summary>
    /// 包含占位符文件的状态标志。
    /// </summary>
    System_FilePlaceholderStatus,
    /// <summary>
    /// 
    /// </summary>
    System_FileVersion,
    /// <summary>
    /// 包含作为字节缓冲区 的 <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/minwinbase/ns-minwinbase-win32_find_dataa">WIN32_FIND_DATA</see> 结构。 请勿将此属性用于任何其他目的。
    /// </summary>
    System_FindData,
    /// <summary>
    /// 
    /// </summary>
    System_FlagColor,
    /// <summary>
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/properties/props-system-flagcolor">System.FlagColor</see> 的用户友好形式。 此值不打算以编程方式进行分析。
    /// </summary>
    System_FlagColorText,
    /// <summary>
    /// 标志的状态。 值： (0=none 1=white 2=Red) 。
    /// </summary>
    System_FlagStatus,
    /// <summary>
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/properties/props-system-flagstatus">System.FlagStatus</see> 的用户友好形式。 此值不打算以编程方式进行分析。
    /// </summary>
    System_FlagStatusText,
    /// <summary>
    /// 此属性表示存储提供程序指定的此文件夹中存储的内容类型。每个文件夹类型必须是 <seealso cref="System_Kind">System.Kind</seealso>, <seealso cref="System_FolderKind">System.FolderKind</seealso> 指定的已知值之一是只读属性，它只能由存储提供程序更新。
    /// </summary>
    System_FolderKind,
    /// <summary>
    /// 此属性类似于 <seealso cref="System_ItemNameDisplay">System.ItemNameDisplay</seealso>，但仅针对文件夹设置，对于文件，此属性将为空。 这可用于将文件和文件夹用作第一个排序键来隔离文件和文件夹。 当 <seealso cref="System_ItemDate">System.ItemDate</seealso> 用作第二个排序键时，它将生成结果，其中文件夹首先按名称排序，然后按日期排序的文件。
    /// </summary>
    System_FolderNameDisplay,
    /// <summary>
    /// 卷中的可用空间量（以字节为单位）。
    /// </summary>
    System_FreeSpace,
    /// <summary>
    /// 此属性用于指定应尽可能广泛地应用于所搜索数据源的所有有效属性的搜索词。不应从数据源发出它。
    /// </summary>
    System_FullText,
    /// <summary>
    /// 项的高置信度关键字。
    /// </summary>
    System_HighKeywords,
    /// <summary>
    /// 图像分析名称。
    /// </summary>
    System_ImageParsingName,
    /// <summary>
    /// 
    /// </summary>
    System_Importance,
    /// <summary>
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/properties/props-system-importance">System.Importance</see> 的用户友好形式。 此值不是以编程方式分析的。
    /// </summary>
    System_ImportanceText,
    /// <summary>
    /// 标识该项是否为附件。
    /// </summary>
    System_IsAttachment,
    /// <summary>
    /// 标识库非所有者库的默认保存位置。
    /// </summary>
    System_IsDefaultNonOwnerSaveLocation,
    /// <summary>
    /// 标识库所有者的默认保存位置。
    /// </summary>
    System_IsDefaultSaveLocation,
    /// <summary>
    /// 
    /// </summary>
    System_IsDeleted,
    /// <summary>
    /// 标识项是否已加密。
    /// </summary>
    System_IsEncrypted,
    /// <summary>
    /// 
    /// </summary>
    System_IsFlagged,
    /// <summary>
    /// 
    /// </summary>
    System_IsFlaggedComplete,
    /// <summary>
    /// 标识消息是否已完全接收。 此值与某些错误条件一起使用。
    /// </summary>
    System_IsIncomplete,
    /// <summary>
    /// 标识将某个位置添加到库时是否已编制索引（本地或远程）。
    /// </summary>
    System_IsLocationSupported,
    /// <summary>
    /// 标识 shell 文件夹是否固定到导航窗格。
    /// </summary>
    System_IsPinnedToNameSpaceTree,
    /// <summary>
    /// 标识项是否已读取。
    /// </summary>
    System_IsRead,
    /// <summary>
    /// 标识位置或库是否仅搜索。
    /// </summary>
    System_IsSearchOnlyItem,
    /// <summary>
    /// 指示项是否为有效的 SendTo 目标。 此信息由某些 Shell 文件夹提供。
    /// </summary>
    System_IsSendToTarget,
    /// <summary>
    /// 指示项是否共享。 这会仅检查非继承 ACL。
    /// </summary>
    System_IsShared,
    /// <summary>
    /// 与项关联的作者的泛型列表。 例如，音乐曲目的艺术家名称是项目作者。
    /// </summary>
    System_ItemAuthors,
    /// <summary>
    /// 项的类类型。
    /// </summary>
    System_ItemClassType,
    /// <summary>
    /// 项目的主要兴趣日期。 例如，对于照片，此属性映射到 <see href="https://learn.microsoft.com/zh-cn/windows/win32/properties/props-system-photo-datetaken">System.Photo.DateTaken</see>。
    /// </summary>
    System_ItemDate,
    /// <summary>
    /// 项目的父文件夹的用户友好显示名称。
    /// </summary>
    System_ItemFolderNameDisplay,
    /// <summary>
    /// 项目的父文件夹的用户友好显示路径。
    /// </summary>
    System_ItemFolderPathDisplay,
    /// <summary>
    /// 项目的父文件夹的用户友好显示路径。
    /// </summary>
    System_ItemFolderPathDisplayNarrow,
    /// <summary>
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/properties/props-system-itemnamedisplay">System.ItemNameDisplay</see> 属性的基名称。
    /// </summary>
    System_ItemName,
    /// <summary>
    /// “最完整”形式的显示名称。
    /// </summary>
    System_ItemNameDisplay,
    /// <summary>
    /// 这类似于 <seealso cref="System_ItemNameDisplay"/>，只是它从不包含文件扩展名。
    /// </summary>
    System_ItemNameDisplayWithoutExtension,
    /// <summary>
    /// 项目的前缀，用于主题以前缀“Re：”开头的电子邮件。
    /// </summary>
    System_ItemNamePrefix,
    /// <summary>
    /// 此字符串应设置为在 CJK 区域设置（CHS 拼音、JPN 平假名、KOR 韩文等）中定义的显示名称的拼音版本。此字段的第一个字符还用于按首字母对名称进行分组。对于大多数非 CJK 语言，不需要设置此字段（在这种情况下，将使用 <seealso cref="System_ItemNameDisplay"/>）。但是，如果需要覆盖分组字母（例如，删除“a”和“the”等前导文章），则可以在此处提供备用字符串。
    /// </summary>
    System_ItemNameSortOverride,
    /// <summary>
    /// 与项目关联的人员的泛型列表和参与项。
    /// </summary>
    System_ItemParticipants,
    /// <summary>
    /// 项的用户友好显示路径。
    /// </summary>
    System_ItemPathDisplay,
    /// <summary>
    /// 项的用户友好显示路径。
    /// </summary>
    System_ItemPathDisplayNarrow,
    /// <summary>
    /// 描述项的子类型。 该值用于向用户显示。与 <seealso cref="System_ItemType">System.ItemType</seealso> 相比，<seealso cref="System_ItemType">System.ItemType</seealso> 通常用于描述所有具有相同通用内容格式的项类，<seealso cref="System_ItemType">System.ItemType</seealso> 可能因项的各个内容或用途而异。例如，此属性可用于将 System.ItemType = “jpg”标识为 <seealso cref="System_ItemType">System.ItemType</seealso> = “Panorama”或 <seealso cref="System_ItemType">System.ItemType</seealso> = “Smart Shot”的项。
    /// </summary>
    System_ItemSubType,
    /// <summary>
    /// 项目的规范类型。
    /// </summary>
    System_ItemType,
    /// <summary>
    /// 项的用户友好类型名称。
    /// </summary>
    System_ItemTypeText,
    /// <summary>
    /// 表示指向项的格式正确的 URL。
    /// </summary>
    System_ItemUrl,
    /// <summary>
    /// 关键字集 (也称为“tags”) 分配给该项。
    /// </summary>
    System_Keywords,
    /// <summary>
    /// 地图各种扩展。搜索文件夹。
    /// </summary>
    System_Kind,
    /// <summary>
    /// <seealso cref="System_Kind">System.Kind</seealso> 的用户友好形式。 此值不是以编程方式分析的。
    /// </summary>
    System_KindText,
    /// <summary>
    /// 文件的主要语言，尤其是该文件是文档时。
    /// </summary>
    System_Language,
    /// <summary>
    /// 
    /// </summary>
    System_LastSyncError,
    /// <summary>
    /// 应用容器的标记。 要编辑文件内容的上一个应用的包系列名称。
    /// </summary>
    System_LastWriterPackageFamilyName,
    /// <summary>
    /// 项的低置信度关键字。
    /// </summary>
    System_LowKeywords,
    /// <summary>
    /// 项的中等置信度关键字。
    /// </summary>
    System_MediumKeywords,
    /// <summary>
    /// 
    /// </summary>
    System_MileageInformation,
    /// <summary>
    /// MIME 类型。
    /// </summary>
    System_MIMEType,
    /// <summary>
    /// 
    /// </summary>
    System_Null,
    /// <summary>
    /// 
    /// </summary>
    System_OfflineAvailability,
    /// <summary>
    /// 
    /// </summary>
    System_OfflineStatus,
    /// <summary>
    /// 
    /// </summary>
    System_OriginalFileName,
    /// <summary>
    /// 拥有库的用户的 SID。
    /// </summary>
    System_OwnerSID,
    /// <summary>
    /// 以格式存储的家长分级通常由 <seealso cref="System_ParentalRatingsOrganization">System.ParentalRatingsOrganization</seealso> 中命名的组织确定。
    /// </summary>
    System_ParentalRating,
    /// <summary>
    /// 说明文件分级。
    /// </summary>
    System_ParentalRatingReason,
    /// <summary>
    /// 其分级系统用于 <seealso cref="System_ParentalRating">System.ParentalRating</seealso> 的组织的名称。
    /// </summary>
    System_ParentalRatingsOrganization,
    /// <summary>
    /// 用于获取要分析的项的 <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/objidl/nn-objidl-ibindctx">IBindCtx</see> 。
    /// </summary>
    System_ParsingBindContext,
    /// <summary>
    /// 项相对于父文件夹的 Shell 命名空间名称。
    /// </summary>
    System_ParsingName,
    /// <summary>
    /// 项的 Shell 命名空间路径。
    /// </summary>
    System_ParsingPath,
    /// <summary>
    /// 基于其规范类型的感知文件类型。
    /// </summary>
    System_PerceivedType,
    /// <summary>
    /// 填充的空间量（以百分比表示）。
    /// </summary>
    System_PercentFull,
    /// <summary>
    /// 
    /// </summary>
    System_Priority,
    /// <summary>
    /// <seealso cref="System_Priority">System.Priority</seealso> 的用户友好形式。 此值不是以编程方式分析的。
    /// </summary>
    System_PriorityText,
    /// <summary>
    /// 
    /// </summary>
    System_Project,
    /// <summary>
    /// 
    /// </summary>
    System_ProviderItemID,
    /// <summary>
    /// 使用介于 1 和 99 之间的整数值的分级系统。 这是 Windows Vista Shell 使用的分级系统。
    /// </summary>
    System_Rating,
    /// <summary>
    /// <seealso cref="System_Rating">System.Rating</seealso> 的用户友好形式。 此值不是以编程方式分析的。
    /// </summary>
    System_RatingText,
    /// <summary>
    /// 
    /// </summary>
    System_RemoteConflictingFile,
    /// <summary>
    /// 
    /// </summary>
    System_Sensitivity,
    /// <summary>
    /// <seealso cref="System_Sensitivity">System.Sensitivity</seealso> 的用户友好形式。 此值不是以编程方式分析的。
    /// </summary>
    System_SensitivityText,
    /// <summary>
    /// <see href="https://learn.microsoft.com/zh-cn/windows/win32/api/shobjidl_core/nf-shobjidl_core-ishellfolder-getattributesof">IShellFolder::GetAttributesOf</see> 中使用的 <see href="https://learn.microsoft.com/zh-cn/windows/win32/shell/sfgao">SFGAO</see> 值。
    /// </summary>
    System_SFGAOFlags,
    /// <summary>
    /// 指示项与谁共享。
    /// </summary>
    System_SharedWith,
    /// <summary>
    /// 
    /// </summary>
    System_ShareUserRating,
    /// <summary>
    /// 指示项目的共享状态：“未共享”、“共享”、“每个人” (家庭组或) 或“专用”。
    /// </summary>
    System_SharingStatus,
    /// <summary>
    /// 省略 Shell 视图中的项。
    /// </summary>
    System_Shell_OmitFromView,
    /// <summary>
    /// 使用介于 0 和 5 之间的整数值的分级系统。
    /// </summary>
    System_SimpleRating,
    /// <summary>
    /// 项的系统提供的文件系统大小（以字节为单位）。
    /// </summary>
    System_Size,
    /// <summary>
    /// 
    /// </summary>
    System_SoftwareUsed,
    /// <summary>
    /// 
    /// </summary>
    System_SourceItem,
    /// <summary>
    /// 存储项实例发起的应用的包系列名称。
    /// </summary>
    System_SourcePackageFamilyName,
    /// <summary>
    /// 
    /// </summary>
    System_StartDate,
    /// <summary>
    /// 适用于该项的常规状态信息。
    /// </summary>
    System_Status,
    /// <summary>
    /// 存储提供程序调用方协议版本信息。此属性的格式特定于提供程序，有关详细信息，请参阅存储提供程序文档。
    /// </summary>
    System_StorageProviderCallerVersionInformation,
    /// <summary>
    /// 
    /// </summary>
    System_StorageProviderError,
    /// <summary>
    /// 存储提供程序为文件计算的校验和。 具有相同校验和值的文件将具有相同的内容。
    /// </summary>
    System_StorageProviderFileChecksum,
    /// <summary>
    /// 此文件的存储提供程序标识符。
    /// </summary>
    System_StorageProviderFileIdentifier,
    /// <summary>
    /// 此文件的存储提供程序的远程 URI。
    /// </summary>
    System_StorageProviderFileRemoteUri,
    /// <summary>
    /// 此文件的存储提供程序文件版本。
    /// </summary>
    System_StorageProviderFileVersion,
    /// <summary>
    /// 此文件的存储提供程序计算文件版本水线。 此值用于检测文件是否已更改。
    /// </summary>
    System_StorageProviderFileVersionWaterline,
    /// <summary>
    /// 此属性表示完全限定的提供程序标识符的 [存储 提供程序 ID] 的一部分“[存储提供程序 ID]！[Windows SID]！[帐户 ID]”。
    /// </summary>
    System_StorageProviderId,
    /// <summary>
    /// 此属性表示存储提供程序指定的文件/文件夹的共享状态列表。每个共享状态必须是下面StorageProviderShareStatuses 枚举指定的已知值之一是只读属性，它只能由存储提供程序更新。
    /// </summary>
    System_StorageProviderShareStatuses,
    /// <summary>
    /// 此属性表示存储提供程序指定的文件/文件夹的最宽松共享状态。共享状态从最宽松到最不宽松依次为“拥有”>“共同拥有”>“公共”>“共享”>“专用”。存储提供程序共享状态是只读属性。
    /// </summary>
    System_StorageProviderSharingStatus,
    /// <summary>
    /// 
    /// </summary>
    System_StorageProviderStatus,
    /// <summary>
    /// 文档的主题。 此属性映射到 OLE 文档属性 Subject。
    /// </summary>
    System_Subject,
    /// <summary>
    /// 
    /// </summary>
    System_SyncTransferStatus,
    /// <summary>
    /// 表示VT_CF格式的缩略图。
    /// </summary>
    System_Thumbnail,
    /// <summary>
    /// 用作缓存缩略图的键的唯一值。
    /// </summary>
    System_ThumbnailCacheId,
    /// <summary>
    /// 以VT_STREAM格式表示缩略图的数据，Windows GDI+和Windows编解码器（如.jpg和.png）支持。
    /// </summary>
    System_ThumbnailStream,
    /// <summary>
    /// 项的标题。
    /// </summary>
    System_Title,
    /// <summary>
    /// 此可选字符串值允许重写 <seealso cref="System_Title">System.Title</seealso> 的标准排序顺序。这对于正确排序日语中的音乐文件非常重要，无法正确排序， (用户预期的排序) 没有此字段。它还可用于自定义非东亚方案中的排序，例如允许用户删除用于排序的文章。
    /// </summary>
    System_TitleSortOverride,
    /// <summary>
    /// 
    /// </summary>
    System_TotalFileSize,
    /// <summary>
    /// 与项目关联的商标，采用字符串格式。
    /// </summary>
    System_Trademarks,
    /// <summary>
    /// 
    /// </summary>
    System_TransferOrder,
    /// <summary>
    /// 
    /// </summary>
    System_TransferPosition,
    /// <summary>
    /// 
    /// </summary>
    System_TransferSize,
    /// <summary>
    /// NTFS 卷的 GUID。
    /// </summary>
    System_VolumeId,
    /// <summary>
    /// Web 区域的标记，作为 URLZONE 枚举值。
    /// </summary>
    System_ZoneIdentifier
}
