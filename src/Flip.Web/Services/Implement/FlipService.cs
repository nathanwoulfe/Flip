using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Flip.Web.Models;
#if NETCOREAPP
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Security;
using Umbraco.Extensions;
#else
using Umbraco.Core;
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

        /// <inheritdoc/>
        public bool TryChangeContentType(ChangeDocumentTypeModel model, out string message)
        {
            var node = _contentService.GetById(model.NodeId);

            if (node == null)
            {
                message = "Could not find source content";
                return false;
            }

            if (node.ContentType.Id == model.ContentTypeId)
            {
                message = "Current type and target type are the same";
                return false;
            }

            var newType = _contentTypeService.GetAll().FirstOrDefault(x => x.Id == model.ContentTypeId);

            if (newType == null)
            {
                message = "Could not find target content type";
                return false;
            }

            MethodInfo changeContentType = node.GetType()
                .GetMethod("ChangeContentType",
                    types: new[] { typeof(IContentType), typeof(bool) },
                    modifiers: null,
                    binder: null,
                    bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);

            changeContentType.Invoke(node, new object[] { newType, true });

            node.TemplateId = model.TemplateId;

            // ensure properties are cleared and re-mapped
            foreach (var prop in node.Properties)
            {
                var newProp = model.Properties.FirstOrDefault(p => p.NewAlias == prop.Alias);
                var newValue = newProp?.Value ?? null;

                node.SetValue(prop.Alias, newValue);
            }

            _contentService.Save(node, _backofficeSecurityAccessor.BackOfficeSecurity.CurrentUser.Id);

            message = "OK";
            return true;
        }

        /// <inheritdoc />
        public IEnumerable<IContentType> GetPermittedTypes(int nodeId)
        {
            var content = _contentService.GetById(nodeId);
            var permittedTypes = _contentTypeService.GetAll();

            permittedTypes = RemoveCurrentDocumentTypeFromAlternatives(permittedTypes, content.ContentTypeId);
            permittedTypes = RemoveInvalidByParentDocumentTypesFromAlternatives(permittedTypes, content.ParentId);
            permittedTypes = RemoveInvalidByChildrenDocumentTypesFromAlternatives(permittedTypes, nodeId);

            return permittedTypes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentTypes"></param>
        /// <param name="currentTypeId"></param>
        /// <returns></returns>
        private static IEnumerable<IContentType> RemoveCurrentDocumentTypeFromAlternatives(IEnumerable<IContentType> documentTypes, int currentTypeId) =>
            documentTypes.Where(x => x.Id != currentTypeId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentTypes"></param>
        /// <param name="parentId"></param>
        /// <returns></returns>
        private IEnumerable<IContentType> RemoveInvalidByParentDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, int parentId)
        {
            if (parentId == -1)
            {
                // Root content, only include those that have been selected as allowed at root
                return documentTypes.Where(x => x.AllowedAsRoot);
            }

            // Below root, so only include those allowed as sub-nodes for the parent
            var parentNode = _contentService.GetById(parentId);
            var parentType = _contentTypeService.Get(parentNode.ContentTypeId);

            return documentTypes
                .Where(x => parentType.AllowedContentTypes
                    .Select(y => y.Id.Value)
                    .Contains(x.Id));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentTypes"></param>
        /// <param name="children"></param>
        /// <returns></returns>
        private IEnumerable<IContentType> RemoveInvalidByChildrenDocumentTypesFromAlternatives(IEnumerable<IContentType> documentTypes, int nodeId)
        {
            var children = _contentService.GetPagedChildren(nodeId, 0, 10000, out _);

            var docTypeIdsOfChildren = children
                .Select(x => x.ContentType.Id)
                .Distinct()
                .ToList();

            return documentTypes
                .Where(x => x.AllowedContentTypes
                    .Select(y => y.Id.Value)
                    .ContainsAll(docTypeIdsOfChildren));
        }
    }
}
