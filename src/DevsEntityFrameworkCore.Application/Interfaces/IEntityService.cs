using System.Collections.Generic;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Models;

namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IEntityService
    {
        Task<ICollection<EntityMap>> GetEntities(bool includeProperty = false);
        Task CreateAbstractEntity();
    }
}
