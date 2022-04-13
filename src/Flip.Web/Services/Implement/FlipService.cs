using System.Linq;
using System.Reflection;
#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Security;
#else
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Flip.Web.Security;
#endif

namespace Flip.Web.Services.Implement
{
    public class FlipService : IFlipService
    {
        private readonly IContentTypeService _contentTypeService;
        private readonly IContentService _contentService;
        private readonly IBackOfficeSecurityAccessor _backofficeSecurityAccessor;

        public FlipService(
            IContentTypeService contentTypeService, 
            IContentService contentService, 
            IBackOfficeSecurityAccessor backOfficeSecurityAccessor)
        {
            _contentTypeService = contentTypeService;
            _contentService = contentService;
            _backofficeSecurityAccessor = backOfficeSecurityAccessor;
        }

        public bool TryChangeContentType(int nodeId, int contentTypeId, out string message)
        {
            var originalContent = _contentService.GetById(nodeId);

            if (originalContent == null)
            {
                message = "Could not find source content";
                return false;
            }

            if (originalContent.ContentType.Id == contentTypeId)
            {
                message = "Current type and target type are the same";
                return false;
            }

            var newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Id == contentTypeId);

            if (newType == null)
            {
                message = "Could not find target content type";
                return false;
            }

            MethodInfo changeContentType = originalContent.GetType()
                .GetMethod("ChangeContentType",
                    types: new[] { typeof(IContentType) },
                    modifiers: null,
                    binder: null,
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            changeContentType.Invoke(originalContent, new[] { newType });

            originalContent.TemplateId = newType.DefaultTemplate.Id;

            _contentService.Save(originalContent, _backofficeSecurityAccessor.BackOfficeSecurity.CurrentUser.Id);

            message = "OK";
            return true;
        }
    }
}
