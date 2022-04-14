using Flip.Web.Services;
using Flip.Web.Models;
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
    public class ApiController : UmbracoAuthorizedApiController
    {
        private readonly IFlipService _flipService;

        public ApiController(IFlipService flipService)
        {
            _flipService = flipService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetPermittedTypes(int nodeId)
        {
            var permittedTypes = _flipService.GetPermittedTypes(nodeId);

            return Ok(new { 
                permittedTypes,
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult ChangeContentType(ChangeDocumentTypeModel model)
        {
            if (!_flipService.TryChangeContentType(model, out string message))
            {
                return BadRequest(message);
            }

            // TODO => return something here
            return Ok(new { });
        }
    }
}
