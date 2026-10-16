using Flip.Models;
using Flip.Notifications;
using Flip.Services;
using Moq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Core.Strings;
using Xunit;
using ContentTypeChangedNotification = Flip.Notifications.ContentTypeChangedNotification;

namespace Flip.Tests.UnitTests.Services;

public class FlipServiceTests
{
    private const int UserId = 5;
    private const string English = "en-US";
    private const string Danish = "da-DK";

    private static readonly Guid UserKey = Guid.NewGuid();
    private static readonly Guid TextBoxDataTypeKey = Guid.NewGuid();
    private static readonly IShortStringHelper ShortStringHelper = new DefaultShortStringHelper(new DefaultShortStringHelperConfig());

    private readonly Mock<IContentTypeService> _contentTypeService = new();
    private readonly Mock<IContentService> _contentService = new();
    private readonly Mock<IElementService> _elementService = new();
    private readonly Mock<IIdKeyMap> _idKeyMap = new();
    private readonly Mock<IBackOfficeSecurityAccessor> _backOfficeSecurityAccessor = new();
    private readonly Mock<IEventAggregator> _eventAggregator = new();
    private readonly Mock<IEventMessagesFactory> _eventMessagesFactory = new();
    private readonly Mock<IAuditService> _auditService = new();

    public FlipServiceTests()
    {
        Mock<IUser> user = new();
        _ = user.Setup(x => x.Key).Returns(UserKey);
        _ = user.Setup(x => x.Id).Returns(UserId);

        Mock<IBackOfficeSecurity> backOfficeSecurity = new();
        _ = backOfficeSecurity.Setup(x => x.CurrentUser).Returns(user.Object);

        _ = _backOfficeSecurityAccessor.Setup(x => x.BackOfficeSecurity).Returns(backOfficeSecurity.Object);
        _ = _eventMessagesFactory.Setup(x => x.Get()).Returns(new EventMessages());
        _ = _auditService
            .Setup(x => x.AddAsync(It.IsAny<AuditType>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>()))
            .ReturnsAsync(Attempt.Succeed(AuditLogOperationStatus.Success));
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_No_Current_User()
    {
        _ = _backOfficeSecurityAccessor.Setup(x => x.BackOfficeSecurity).Returns((IBackOfficeSecurity?)null);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(new ChangeEntityTypeModel());

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.UserNotFound, result.Result);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_Content_Not_Found()
    {
        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(new ChangeEntityTypeModel { Unique = Guid.NewGuid() });

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.ContentNotFound, result.Result);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_Cancelled_By_Notification()
    {
        (ContentType contentPage, ContentType contact) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        Register(content, contentPage, contact);
        _ = _eventAggregator.Setup(x => x.PublishCancelable(It.IsAny<CancelableObjectNotification<IContent>>())).Returns(true);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contact));

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.CancelledByNotification, result.Result);
        Assert.Equal(contentPage.Key, content.ContentType.Key);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_Target_Type_Matches_Current_Type()
    {
        (ContentType contentPage, _) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        Register(content, contentPage);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contentPage));

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.CurrentAndNewTypesMatch, result.Result);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_Target_Type_Not_Found()
    {
        (ContentType contentPage, ContentType contact) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        Register(content, contentPage);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contact));

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.ContentTypeNotFound, result.Result);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Maps_Value_To_Property_With_Different_Alias()
    {
        (ContentType contentPage, ContentType contact) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        content.SetValue("title", "Hello");
        Register(content, contentPage, contact);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contact, ("title", "mapHeader")));

        Assert.True(result.Success);
        Assert.Equal(contact.Key, content.ContentType.Key);
        Assert.Equal("Hello", content.GetValue("mapHeader"));
        Assert.False(content.HasProperty("title"));
    }

    [Fact]
    public async Task TryChangeContentType_Maps_Value_To_Property_With_Same_Alias()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing, "title");
        ContentType textPage = CreateContentType(2, "textPage", ContentVariation.Nothing, "title");
        Content content = CreateContent(contentPage);
        content.SetValue("title", "Hello");
        Register(content, contentPage, textPage);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, textPage, ("title", "title")));

        Assert.True(result.Success);
        Assert.Equal("Hello", content.GetValue("title"));
    }

    [Fact]
    public async Task TryChangeContentType_Swaps_Values_Between_Properties()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing, "heading", "summary");
        ContentType textPage = CreateContentType(2, "textPage", ContentVariation.Nothing, "heading", "summary");
        Content content = CreateContent(contentPage);
        content.SetValue("heading", "Heading value");
        content.SetValue("summary", "Summary value");
        Register(content, contentPage, textPage);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, textPage, ("heading", "summary"), ("summary", "heading")));

        Assert.True(result.Success);
        Assert.Equal("Summary value", content.GetValue("heading"));
        Assert.Equal("Heading value", content.GetValue("summary"));
    }

    [Fact]
    public async Task TryChangeContentType_Sets_Null_When_Mapped_Source_Property_Does_Not_Exist()
    {
        (ContentType contentPage, ContentType contact) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        Register(content, contentPage, contact);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contact, ("doesNotExist", "mapHeader")));

        Assert.True(result.Success);
        Assert.Null(content.GetValue("mapHeader"));
    }

    [Fact]
    public async Task TryChangeContentType_Maps_Culture_Variant_Values_And_Keeps_Culture_Names()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Culture, "title");
        ContentType contact = CreateContentType(2, "contact", ContentVariation.Culture, "mapHeader");
        Content content = CreateContent(contentPage, English);
        content.SetCultureName("Hjem", Danish);
        content.SetValue("title", "Hello", English);
        content.SetValue("title", "Hej", Danish);
        Register(content, contentPage, contact);

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(CreateModel(content, contact, ("title", "mapHeader")));

        Assert.True(result.Success);
        Assert.Equal("Hello", content.GetValue("mapHeader", English));
        Assert.Equal("Hej", content.GetValue("mapHeader", Danish));
        Assert.Equal("Home", content.GetCultureName(English));
        Assert.Equal("Hjem", content.GetCultureName(Danish));
    }

    [Fact]
    public async Task TryChangeContentType_Saves_Content_Publishes_Notifications_And_Audits()
    {
        (ContentType contentPage, ContentType contact) = CreateContentPageAndContact();
        Content content = CreateContent(contentPage);
        Register(content, contentPage, contact);

        Guid templateKey = Guid.NewGuid();
        _ = _idKeyMap.Setup(x => x.GetIdForKey(templateKey, UmbracoObjectTypes.Template)).Returns(Attempt.Succeed(99));

        ChangeEntityTypeModel model = CreateModel(content, contact);
        model.TemplateKey = templateKey;

        Attempt<FlipOperationStatus> result = await CreateDocumentService().TryChangeContentType(model);

        Assert.True(result.Success);
        Assert.Equal(FlipOperationStatus.Success, result.Result);
        Assert.Equal(99, content.TemplateId);
        _contentService.As<IPublishableContentService<IContent>>().Verify(x => x.Save(content, UserId, null), Times.Once);
        _eventAggregator.Verify(x => x.PublishCancelable(It.Is<CancelableObjectNotification<IContent>>(n => n is ContentTypeChangingNotification && ((ContentTypeChangingNotification)n).Content == content)), Times.Once);
        _eventAggregator.Verify(x => x.Publish(It.Is<ObjectNotification<IContent>>(n => n is ContentTypeChangedNotification && ((ContentTypeChangedNotification)n).Content == content)), Times.Once);
        _auditService.Verify(x => x.AddAsync(AuditType.Custom, UserKey, content.Id, "Document", "Changed document type from contentPage to contact", "Changed document type from contentPage to contact"), Times.Once);
    }

    [Fact]
    public async Task TryChangeContentType_Element_Saves_Element_Publishes_Element_Notifications_And_Audits()
    {
        ContentType heroBlock = CreateElementType(5, "heroBlock", allowedInLibrary: true, "title");
        ContentType textBlock = CreateElementType(6, "textBlock", allowedInLibrary: true, "mapHeader");
        Element element = CreateElement(heroBlock);
        element.SetValue("title", "Hello");
        RegisterElement(element, heroBlock, textBlock);

        Attempt<FlipOperationStatus> result = await CreateElementService().TryChangeContentType(CreateModel(element, textBlock, ("title", "mapHeader")));

        Assert.True(result.Success);
        Assert.Equal(textBlock.Key, element.ContentType.Key);
        Assert.Equal("Hello", element.GetValue("mapHeader"));
        _elementService.As<IPublishableContentService<IElement>>().Verify(x => x.Save(element, UserId, null), Times.Once);
        _eventAggregator.Verify(x => x.PublishCancelable(It.Is<CancelableObjectNotification<IElement>>(n => n is ElementTypeChangingNotification && ((ElementTypeChangingNotification)n).Element == element)), Times.Once);
        _eventAggregator.Verify(x => x.Publish(It.Is<ObjectNotification<IElement>>(n => n is ElementTypeChangedNotification && ((ElementTypeChangedNotification)n).Element == element)), Times.Once);
        _auditService.Verify(x => x.AddAsync(AuditType.Custom, UserKey, element.Id, "Element", "Changed element type from heroBlock to textBlock", "Changed element type from heroBlock to textBlock"), Times.Once);
    }

    [Fact]
    public async Task TryChangeContentType_Element_Fails_When_Cancelled_By_Notification()
    {
        ContentType heroBlock = CreateElementType(5, "heroBlock", allowedInLibrary: true);
        ContentType textBlock = CreateElementType(6, "textBlock", allowedInLibrary: true);
        Element element = CreateElement(heroBlock);
        RegisterElement(element, heroBlock, textBlock);
        _ = _eventAggregator.Setup(x => x.PublishCancelable(It.IsAny<CancelableObjectNotification<IElement>>())).Returns(true);

        Attempt<FlipOperationStatus> result = await CreateElementService().TryChangeContentType(CreateModel(element, textBlock));

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.CancelledByNotification, result.Result);
        Assert.Equal(heroBlock.Key, element.ContentType.Key);
        _elementService.As<IPublishableContentService<IElement>>().Verify(x => x.Save(It.IsAny<IElement>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection?>()), Times.Never);
    }

    [Fact]
    public void GetContentModel_Returns_Null_When_Content_Not_Found() =>
        Assert.Null(CreateDocumentService().GetContentModel(Guid.NewGuid()));

    [Fact]
    public void GetContentModel_Returns_Content_Properties()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing, "title", "bodyText");
        Content content = CreateContent(contentPage);
        Register(content, contentPage);

        ChangeEntityTypeModel? model = CreateDocumentService().GetContentModel(content.Key);

        Assert.NotNull(model);
        Assert.NotNull(model.Properties);
        Assert.Collection(
            model.Properties.OrderBy(x => x.Alias),
            x => AssertProperty("bodyText", x),
            x => AssertProperty("title", x));

        static void AssertProperty(string alias, EntityTypePropertyModel property)
        {
            Assert.Equal(alias, property.Alias);
            Assert.Equal(alias, property.Label);
            Assert.Equal(Constants.PropertyEditors.Aliases.TextBox, property.Editor);
            Assert.Equal(TextBoxDataTypeKey, property.DataTypeKey);
        }
    }

    [Fact]
    public async Task GetPermittedTypes_Returns_Empty_When_Content_Not_Found() =>
        Assert.Empty(await CreateDocumentService().GetPermittedTypes(Guid.NewGuid()));

    [Fact]
    public async Task GetPermittedTypes_At_Root_Returns_Types_Allowed_At_Root_Except_Current()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing);
        ContentType home = CreateContentType(2, "home", ContentVariation.Nothing);
        ContentType contact = CreateContentType(3, "contact", ContentVariation.Nothing);
        contentPage.AllowedAsRoot = true;
        home.AllowedAsRoot = true;
        Content content = CreateContent(contentPage);
        Register(content, contentPage, home, contact);
        SetupAllowedChildren(contentPage);

        IEnumerable<ContentTypeModel> result = await CreateDocumentService().GetPermittedTypes(content.Key);

        Assert.Equal(home.Key, Assert.Single(result).Unique);
    }

    [Fact]
    public async Task GetPermittedTypes_Below_Root_Returns_Types_Allowed_By_Parent_Except_Current()
    {
        ContentType landing = CreateContentType(10, "landing", ContentVariation.Nothing);
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing);
        ContentType home = CreateContentType(2, "home", ContentVariation.Nothing);
        ContentType contact = CreateContentType(3, "contact", ContentVariation.Nothing);
        landing.AllowedContentTypes =
        [
            new ContentTypeSort(contentPage.Key, 0, contentPage.Alias),
            new ContentTypeSort(contact.Key, 1, contact.Alias),
        ];

        Content parent = new("Landing", -1, landing) { Id = 1000, Key = Guid.NewGuid() };
        Content content = CreateContent(contentPage, parentId: parent.Id);
        Register(content, landing, contentPage, home, contact);
        _ = _idKeyMap.Setup(x => x.GetKeyForId(parent.Id, UmbracoObjectTypes.Document)).Returns(Attempt.Succeed(parent.Key));
        _ = _contentService.Setup(x => x.GetById(parent.Key)).Returns(parent);
        _ = _contentTypeService.Setup(x => x.Get(landing.Id)).Returns(landing);
        SetupAllowedChildren(contentPage);

        IEnumerable<ContentTypeModel> result = await CreateDocumentService().GetPermittedTypes(content.Key);

        Assert.Equal(contact.Key, Assert.Single(result).Unique);
    }

    [Fact]
    public async Task GetPermittedTypes_Excludes_Types_That_Do_Not_Allow_The_Current_Types_Allowed_Children()
    {
        ContentType article = CreateContentType(4, "article", ContentVariation.Nothing);
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing);
        ContentType home = CreateContentType(2, "home", ContentVariation.Nothing);
        ContentType contact = CreateContentType(3, "contact", ContentVariation.Nothing);
        contentPage.AllowedAsRoot = true;
        home.AllowedAsRoot = true;
        contact.AllowedAsRoot = true;
        home.AllowedContentTypes = [new ContentTypeSort(article.Key, 0, article.Alias)];

        Content content = CreateContent(contentPage);
        Register(content, article, contentPage, home, contact);
        SetupAllowedChildren(contentPage, article);

        IEnumerable<ContentTypeModel> result = await CreateDocumentService().GetPermittedTypes(content.Key);

        Assert.Equal(home.Key, Assert.Single(result).Unique);
    }

    [Fact]
    public async Task GetPermittedTypes_Element_Returns_Element_Types_With_Same_Library_Setting_Except_Current()
    {
        ContentType heroBlock = CreateElementType(5, "heroBlock", allowedInLibrary: true);
        ContentType textBlock = CreateElementType(6, "textBlock", allowedInLibrary: true);
        ContentType nestedBlock = CreateElementType(7, "nestedBlock", allowedInLibrary: false);
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing);
        Element element = CreateElement(heroBlock);
        RegisterElement(element, heroBlock, textBlock, nestedBlock, contentPage);

        IEnumerable<ContentTypeModel> result = await CreateElementService().GetPermittedTypes(element.Key);

        Assert.Equal(textBlock.Key, Assert.Single(result).Unique);
    }

    private static (ContentType ContentPage, ContentType Contact) CreateContentPageAndContact() =>
        (CreateContentType(1, "contentPage", ContentVariation.Nothing, "title"),
         CreateContentType(2, "contact", ContentVariation.Nothing, "mapHeader"));

    private static ContentType CreateContentType(int id, string alias, ContentVariation variations, params string[] propertyAliases)
    {
        ContentType contentType = new(ShortStringHelper, -1)
        {
            Id = id,
            Key = Guid.NewGuid(),
            Alias = alias,
            Name = alias,
            Variations = variations,
        };

        foreach (string propertyAlias in propertyAliases)
        {
            _ = contentType.AddPropertyType(new PropertyType(ShortStringHelper, Constants.PropertyEditors.Aliases.TextBox, ValueStorageType.Nvarchar, propertyAlias)
            {
                Name = propertyAlias,
                DataTypeKey = TextBoxDataTypeKey,
                Variations = variations,
            });
        }

        return contentType;
    }

    private static ContentType CreateElementType(int id, string alias, bool allowedInLibrary, params string[] propertyAliases)
    {
        ContentType elementType = CreateContentType(id, alias, ContentVariation.Nothing, propertyAliases);
        elementType.IsElement = true;
        elementType.AllowedInLibrary = allowedInLibrary;
        return elementType;
    }

    private static Content CreateContent(ContentType contentType, string? culture = null, int parentId = -1) =>
        new("Home", parentId, contentType, culture)
        {
            Id = 1234,
            Key = Guid.NewGuid(),
        };

    private static Element CreateElement(ContentType elementType) =>
        new("Hero", elementType)
        {
            Id = 2345,
            Key = Guid.NewGuid(),
        };

    // mirrors the client, which sends the source alias as Alias and the target alias as NewAlias
    private static ChangeEntityTypeModel CreateModel(IPublishableContentBase content, IContentType target, params (string Alias, string NewAlias)[] mappings) =>
        new()
        {
            Unique = content.Key,
            ContentTypeKey = target.Key,
            Properties = mappings
                .Select(x => new EntityTypePropertyModel { Alias = x.Alias, NewAlias = x.NewAlias })
                .ToList(),
        };

    private void Register(IContent content, params IContentType[] contentTypes)
    {
        _ = _contentService.Setup(x => x.GetById(content.Key)).Returns(content);
        _ = _contentTypeService.Setup(x => x.GetAll()).Returns(contentTypes);
    }

    private void RegisterElement(IElement element, params IContentType[] contentTypes)
    {
        _ = _elementService.Setup(x => x.GetById(element.Key)).Returns(element);
        _ = _contentTypeService.Setup(x => x.GetAll()).Returns(contentTypes);
    }

    private void SetupAllowedChildren(IContentType contentType, params IContentType[] allowedChildren) =>
        _contentTypeService
            .Setup(x => x.GetAllowedChildrenAsync(contentType.Key, 0, int.MaxValue))
            .ReturnsAsync(Attempt.SucceedWithStatus<PagedModel<IContentType>?, ContentTypeOperationStatus>(
                ContentTypeOperationStatus.Success,
                new PagedModel<IContentType>(allowedChildren.Length, allowedChildren)));

    private void VerifyNotSaved() =>
        _contentService.As<IPublishableContentService<IContent>>()
            .Verify(x => x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection?>()), Times.Never);

    private FlipDocumentService CreateDocumentService() =>
        new(
            _contentService.Object,
            _idKeyMap.Object,
            _contentTypeService.Object,
            _backOfficeSecurityAccessor.Object,
            _eventAggregator.Object,
            _eventMessagesFactory.Object,
            _auditService.Object);

    private FlipElementService CreateElementService() =>
        new(
            _elementService.Object,
            _idKeyMap.Object,
            _contentTypeService.Object,
            _backOfficeSecurityAccessor.Object,
            _eventAggregator.Object,
            _eventMessagesFactory.Object,
            _auditService.Object);
}
