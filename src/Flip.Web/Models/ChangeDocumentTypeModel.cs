using System.Collections.Generic;

namespace Flip.Web.Models
{
    public class ChangeDocumentTypeModel
    {
        public int NodeId;
        public int ContentTypeId;
        public int TemplateId;
        public IEnumerable<DocumentTypePropertyModel> Properties;
    }
}
