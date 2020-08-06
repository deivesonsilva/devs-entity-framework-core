using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DevsEntityFrameworkCore.Application.Interfaces;

namespace DevsEntityFrameworkCore.Application.Services
{
    public class FileService : IFileService
    {
        public async Task<string> GetContentFile(string filename)
        {
            const string reduceMultiSpace = @"[ ]{2,}";

            string content = string.Join(" ", await File.ReadAllLinesAsync(filename));

            return Regex.Replace(content.Replace("\t", " "), reduceMultiSpace, " ");
        }

        public string[] GetFileFromFolder(string folderpath, string extensao = "*.cs")
        {
            if (Directory.Exists(folderpath))
                return Directory.GetFiles(folderpath, extensao, SearchOption.TopDirectoryOnly);

            return new string[] { };
        }

        public async Task<string> GetContentFileFromUrl(string url)
        {
            Uri uri = new Uri(url, UriKind.Absolute);

            using (WebClient webClient = new WebClient())
            {
                using (Stream stream = await webClient.OpenReadTaskAsync(uri))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        return await reader.ReadToEndAsync();
                    }
                }
            }
        }

        public async Task SaveFile(string content, string fullpathname)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fullpathname)))
                Directory.CreateDirectory(Path.GetDirectoryName(fullpathname));

            using (FileStream stream = new FileStream(fullpathname, FileMode.Create, FileAccess.Write, FileShare.Write, 4096, useAsync: true))
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                await stream.WriteAsync(bytes, 0, bytes.Length);
                stream.Close();
            }
        }
    }
}
