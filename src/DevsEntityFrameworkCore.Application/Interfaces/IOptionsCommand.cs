namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IOptionsCommand
    {
        string DirectoryWorking { get; set; }
        bool CreateContext { get; set; }
        bool CreateUnitOfWork { get; set; }
        bool CreateRepositoryBase { get; set; }
        bool CreateInitialize { get; set; }
        bool RunAll { get; set; }
        bool ReplaceFile { get; set; }
    }
}
