using System.Threading.Tasks;

namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IFileService
    {
        Task<string> GetContentFile(string filename);
        string[] GetFileFromFolder(string foldername, string extensao = "*.cs");
        Task<string> GetContentFileFromUrl(string url);
        Task SaveFile(string content, string fullpathname);
    }
}
