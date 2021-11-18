using ICSharpCode.SharpZipLib.Zip;
using System;
using System.IO;
using System.Linq;

namespace NetCoreProject.Domain.Util
{
    public class CompressUtil
    {
        public CompressUtil() 
        { 
        }
        public void Compress(FileInfo fileInfo, string password, DirectoryInfo source)
        {
            using var fileStream = fileInfo.Create();
            using var zipOutputStream = new ZipOutputStream(fileStream);
            zipOutputStream.Password = password;
            zipOutputStream.SetLevel(9);
            AddZipEntry(zipOutputStream, string.Empty, source);
        }
        public void Compress(FileInfo fileInfo, string password, FileInfo source)
        {
            using var fileStream = fileInfo.Create();
            using var zipOutputStream = new ZipOutputStream(fileStream);
            zipOutputStream.Password = password;
            zipOutputStream.SetLevel(9);
            AddZipEntry(zipOutputStream, string.Empty, source);
        }
        private void AddZipEntry(ZipOutputStream zipOutputStream, string path, DirectoryInfo directoryInfo)
        {
            directoryInfo.EnumerateDirectories().ToList().ForEach(f => AddZipEntry(zipOutputStream, Path.Combine(path, f.Name), f));
            directoryInfo.EnumerateFiles().ToList().ForEach(f => AddZipEntry(zipOutputStream, path, f));
        }
        private void AddZipEntry(ZipOutputStream zipOutputStream, string path, FileInfo fileInfo)
        {
            var buffer = new byte[4096];
            using var fileStream = fileInfo.OpenRead();
            var zipEntry = new ZipEntry(PathConvert(Path.Combine(path, fileInfo.Name)))
            {
                DateTime = DateTime.Now,
                Size = fileInfo.Length
            };
            zipOutputStream.PutNextEntry(zipEntry);
            var length = fileStream.Length;
            fileStream.Seek(0, SeekOrigin.Begin);
            while (length > 0)
            {
                var readSoFar = fileStream.Read(buffer, 0, buffer.Length);
                zipOutputStream.Write(buffer, 0, readSoFar);
                length -= readSoFar;
            }
        }
        public void UnCompress(string sourcePath, string password, DirectoryInfo directoryInfo)
        {
            var buffer = new byte[4096];
            using var zipInputStream = new ZipInputStream(File.OpenRead(sourcePath));
            if (!directoryInfo.Exists)
            {
                directoryInfo.Create();
            }
            var zipEntry = default(ZipEntry);
            while ((zipEntry = zipInputStream.GetNextEntry()) != null)
            {
                var targetFileInfo = new FileInfo(PathConvert(Path.Combine(directoryInfo.FullName, zipEntry.Name)));
                if (!targetFileInfo.Directory.Exists)
                {
                    targetFileInfo.Directory.Create();
                }
                using var fileStream = new FileStream(targetFileInfo.FullName, FileMode.Create);
                var length = fileStream.Length;
                while (length > 0)
                {
                    var readSoFar = zipInputStream.Read(buffer, 0, buffer.Length);
                    fileStream.Write(buffer, 0, readSoFar);
                    length -= readSoFar;
                }
            }
        }
        private string PathConvert(string path) => path.Replace('\\', '/');
    }
}
