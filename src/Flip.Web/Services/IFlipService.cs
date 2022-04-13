namespace Flip.Web.Services
{
    public interface IFlipService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="contentTypeId"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool TryChangeContentType(int nodeId, int contentTypeId, out string message);
    }
}
