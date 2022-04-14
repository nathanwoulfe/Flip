using Flip.Web.Models;
using System.Collections.Generic;
#if NETCOREAPP
using Umbraco.Cms.Core.Models;
#else
using Umbraco.Core.Models;
#endif

namespace Flip.Web.Services
{
    public interface IFlipService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool TryChangeContentType(ChangeDocumentTypeModel model, out string message);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        IEnumerable<IContentType> GetPermittedTypes(int nodeId);
    }
}
