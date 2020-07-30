using System.Collections.Generic;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Models;

namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface ICsprojService
    {
        Task<ICollection<EntityFile>> GetEntitiesFiles();
        void FolderInclude(string foldername);
    }
}
