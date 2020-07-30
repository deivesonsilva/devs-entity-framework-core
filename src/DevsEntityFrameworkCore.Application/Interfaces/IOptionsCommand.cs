namespace DevsEntityFrameworkCore.Application.Interfaces
{
    public interface IOptionsCommand
    {
        string DirectoryWorking { get; set; }
        bool ReplaceFile { get; set; }
    }
}
