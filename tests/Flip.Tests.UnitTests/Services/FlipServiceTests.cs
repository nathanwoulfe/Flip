using Flip.Models;
using Flip.Notifications;
using Flip.Services;
using Moq;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Persistence.Querying;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Core.Strings;
using Xunit;

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(new ChangeDocumentTypeModel());

        Assert.False(result.Success);
        Assert.Equal(FlipOperationStatus.UserNotFound, result.Result);
        VerifyNotSaved();
    }

    [Fact]
    public async Task TryChangeContentType_Fails_When_Content_Not_Found()
    {
        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(new ChangeDocumentTypeModel { Unique = Guid.NewGuid() });

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
        _ = _eventAggregator.Setup(x => x.PublishCancelable(It.IsAny<ContentTypeChangingNotification>())).Returns(true);

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contact));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contentPage));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contact));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contact, ("title", "mapHeader")));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, textPage, ("title", "title")));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, textPage, ("heading", "summary"), ("summary", "heading")));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contact, ("doesNotExist", "mapHeader")));

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

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(CreateModel(content, contact, ("title", "mapHeader")));

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

        ChangeDocumentTypeModel model = CreateModel(content, contact);
        model.TemplateId = 99;

        Attempt<FlipOperationStatus> result = await CreateService().TryChangeContentType(model);

        Assert.True(result.Success);
        Assert.Equal(FlipOperationStatus.Success, result.Result);
        Assert.Equal(99, content.TemplateId);
        _contentService.Verify(x => x.Save(content, UserId, null), Times.Once);
        _eventAggregator.Verify(x => x.PublishCancelable(It.Is<ContentTypeChangingNotification>(n => n.Content == content)), Times.Once);
        _eventAggregator.Verify(x => x.Publish(It.Is<ContentTypeChangedNotification>(n => n.Content == content)), Times.Once);
        _auditService.Verify(x => x.AddAsync(AuditType.Custom, UserKey, content.Id, "Document", "Changed document type from contentPage to contact", "Changed document type from contentPage to contact"), Times.Once);
    }

    [Fact]
    public void GetContentModel_Returns_Null_When_Content_Not_Found() =>
        Assert.Null(CreateService().GetContentModel(Guid.NewGuid()));

    [Fact]
    public void GetContentModel_Returns_Content_Properties()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing, "title", "bodyText");
        Content content = CreateContent(contentPage);
        Register(content, contentPage);

        ChangeDocumentTypeModel? model = CreateService().GetContentModel(content.Key);

        Assert.NotNull(model);
        Assert.NotNull(model.Properties);
        Assert.Collection(
            model.Properties.OrderBy(x => x.Alias),
            x => AssertProperty("bodyText", x),
            x => AssertProperty("title", x));

        static void AssertProperty(string alias, DocumentTypePropertyModel property)
        {
            Assert.Equal(alias, property.Alias);
            Assert.Equal(alias, property.Label);
            Assert.Equal(Constants.PropertyEditors.Aliases.TextBox, property.Editor);
            Assert.Equal(TextBoxDataTypeKey.ToString(), property.DataTypeKey);
        }
    }

    [Fact]
    public void GetPermittedTypes_Returns_Empty_When_Content_Not_Found() =>
        Assert.Empty(CreateService().GetPermittedTypes(Guid.NewGuid()));

    [Fact]
    public void GetPermittedTypes_At_Root_Returns_Types_Allowed_At_Root_Except_Current()
    {
        ContentType contentPage = CreateContentType(1, "contentPage", ContentVariation.Nothing);
        ContentType home = CreateContentType(2, "home", ContentVariation.Nothing);
        ContentType contact = CreateContentType(3, "contact", ContentVariation.Nothing);
        contentPage.AllowedAsRoot = true;
        home.AllowedAsRoot = true;
        Content content = CreateContent(contentPage);
        Register(content, contentPage, home, contact);
        SetupChildren(content);

        IEnumerable<ContentTypeModel> result = CreateService().GetPermittedTypes(content.Key);

        Assert.Equal(home.Key, Assert.Single(result).Unique);
    }

    [Fact]
    public void GetPermittedTypes_Below_Root_Returns_Types_Allowed_By_Parent_Except_Current()
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

        Content parent = new("Landing", -1, landing) { Id = 1000 };
        Content content = CreateContent(contentPage, parentId: parent.Id);
        Register(content, landing, contentPage, home, contact);
        _ = _contentService.Setup(x => x.GetById(parent.Id)).Returns(parent);
        _ = _contentTypeService.Setup(x => x.Get(landing.Id)).Returns(landing);
        SetupChildren(content);

        IEnumerable<ContentTypeModel> result = CreateService().GetPermittedTypes(content.Key);

        Assert.Equal(contact.Key, Assert.Single(result).Unique);
    }

    [Fact]
    public void GetPermittedTypes_Excludes_Types_That_Do_Not_Allow_Existing_Children()
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
        SetupChildren(content, new Content("Article", content.Id, article));

        IEnumerable<ContentTypeModel> result = CreateService().GetPermittedTypes(content.Key);

        Assert.Equal(home.Key, Assert.Single(result).Unique);
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

    private static Content CreateContent(ContentType contentType, string? culture = null, int parentId = -1) =>
        new("Home", parentId, contentType, culture)
        {
            Id = 1234,
            Key = Guid.NewGuid(),
        };

    // mirrors the client, which sends the source alias as Alias and the target alias as NewAlias
    private static ChangeDocumentTypeModel CreateModel(IContent content, IContentType target, params (string Alias, string NewAlias)[] mappings) =>
        new()
        {
            Unique = content.Key,
            ContentTypeKey = target.Key,
            Properties = mappings
                .Select(x => new DocumentTypePropertyModel { Alias = x.Alias, NewAlias = x.NewAlias })
                .ToList(),
        };

    private void Register(IContent content, params IContentType[] contentTypes)
    {
        _ = _contentService.Setup(x => x.GetById(content.Key)).Returns(content);
        _ = _contentTypeService.Setup(x => x.GetAll()).Returns(contentTypes);
    }

    private void SetupChildren(IContent content, params IContent[] children)
    {
        long totalRecords = children.Length;
        _ = _contentService
            .Setup(x => x.GetPagedChildren(content.Id, It.IsAny<long>(), It.IsAny<int>(), out totalRecords, It.IsAny<IQuery<IContent>?>(), It.IsAny<Ordering?>()))
            .Returns(children);
    }

    private void VerifyNotSaved() =>
        _contentService.Verify(x => x.Save(It.IsAny<IContent>(), It.IsAny<int?>(), It.IsAny<ContentScheduleCollection?>()), Times.Never);

    private FlipService CreateService() =>
        new(
            _contentTypeService.Object,
            _contentService.Object,
            _backOfficeSecurityAccessor.Object,
            _eventAggregator.Object,
            _eventMessagesFactory.Object,
            _auditService.Object);
}
