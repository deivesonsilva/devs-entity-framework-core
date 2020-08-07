using System.Collections.Generic;

namespace DevsEntityFrameworkCore.Application.Models
{
    public class EntityMap
    {
        public string ClassName { get; set; }
        public ICollection<EntityPropertyMap> Properties { get; set; }
    }
}
