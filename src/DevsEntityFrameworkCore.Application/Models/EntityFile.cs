using System.Collections.Generic;

namespace DevsEntityFrameworkCore.Application.Models
{
    public class EntityFile
    {
        public string Namespace { get; set; }
        public string ClassName { get; set; }
        public ICollection<EntityProperty> Properties { get; set; }
    }
}
