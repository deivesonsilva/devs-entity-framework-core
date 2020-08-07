using System.Threading.Tasks;

namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IContextService
    {
        Task CreateContextFile();
    }
}
