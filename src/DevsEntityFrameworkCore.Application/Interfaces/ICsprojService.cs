
namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface ICsprojService
    {
        void FolderInclude(string foldername);
        void ExistPackageReference();
        void IsValidProject(string fullpath);
        string ProjectFileName { get; }
        string ProjectNamespace { get; }
        string ProjectPath { get; }
    }
}
