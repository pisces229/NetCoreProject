using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NetCoreProject.Domain.Util
{
    public class FileUtil
    {
        public FileUtil() 
        { 
        }
        public FileInfo GetFileInfo(string basePath, 
            IEnumerable<string> directoryNames, string fileName)
        {
            FileInfo fileInfo = null;
            var directoryInfo = new DirectoryInfo(basePath);
            var directoryInfoExists = directoryInfo.Exists;
            if (directoryInfoExists)
            {
                var directories = directoryInfo.EnumerateDirectories();
                foreach (var directoryName in directoryNames)
                {
                    var directoryInfos = directories
                        .Where(w => w.Name == directoryName);
                    if (directoryInfos.Any())
                    {
                        directoryInfo = directoryInfos.First();
                    }
                    else
                    {
                        directoryInfoExists = false;
                        break;
                    }
                }
                if (directoryInfoExists)
                {
                    fileInfo = directoryInfo.EnumerateFiles()
                        .Where(t => t.Name == fileName)
                        .FirstOrDefault();
                }
            }
            return fileInfo;
        }
        public string GetExtension(string fileName)
        {
            return Path.GetExtension(fileName).Replace(".", "");
        }
    }
}
