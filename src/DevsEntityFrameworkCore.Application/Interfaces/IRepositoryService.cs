using System.Threading.Tasks;

namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IRepositoryService
    {
        Task CreateRepositories();
        Task CreateRepositoryBase();
    }
}
