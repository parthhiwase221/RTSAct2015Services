namespace RTSAct2015.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string subDirectory);
        bool DeleteFile(string filePath);
        string GetFileUrl(string filePath);
    }
}
