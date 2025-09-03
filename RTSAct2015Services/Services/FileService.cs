namespace RTSAct2015.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string subDirectory)
        {
            if (file == null || file.Length == 0)
                return null;

            // Create unique filename
            string fileName = $"{Guid.NewGuid()}_{file.FileName}";
            string uploadPath = Path.Combine(_environment.WebRootPath, "uploads", subDirectory);

            // Create directory if it doesn't exist
            if (!Directory.Exists(uploadPath))
                Directory.CreateDirectory(uploadPath);

            string filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{subDirectory}/{fileName}";
        }

        public bool DeleteFile(string filePath)
        {
            try
            {
                string fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public string GetFileUrl(string filePath)
        {
            return filePath?.StartsWith("/") == true ? filePath : $"/{filePath}";
        }
    }
}
