using Flip.Web.Services;
#if NETCOREAPP
using Microsoft.AspNetCore.Mvc;
using Umbraco.Cms.Web.Common.Attributes;
using Umbraco.Cms.Web.BackOffice.Controllers;
#else
using Umbraco.Web.WebApi;
using Umbraco.Web.Mvc;
using System.Web.Http;
using IActionResult = System.Web.Http.IHttpActionResult;
#endif

namespace Flip.Web.Controllers
{
    [PluginController(Constants.Name)]
    public class FlipController : UmbracoAuthorizedApiController
    {
        private readonly IFlipService _flipService;

        public FlipController(IFlipService flipService)
        {
            _flipService = flipService;
        }

        [HttpPost]
        public IActionResult ChangeContentType(ChangeContentTypeModel model)
        {
            if (!_flipService.TryChangeContentType(model.NodeId, model.ContentTypeId, out string message))
            {
                return BadRequest(message);
            }

            // TODO => return something here
            return Ok(new { });
        }
    }

    public class ChangeContentTypeModel
    {
        public int NodeId;
        public int ContentTypeId;
    }
}
