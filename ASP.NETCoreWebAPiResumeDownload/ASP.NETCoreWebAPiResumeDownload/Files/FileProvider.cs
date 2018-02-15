using System.IO;
using System.Linq;

namespace ASP.NETCoreWebAPiResumeDownload.Files
{
    public class FileProvider : IFileProvider
    {
        private const string FileDirectory = @"D:\软件\win8.1系统安装";

        public FileProvider()
        {
        }

        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            string file = Directory.GetFiles(FileDirectory, name, SearchOption.TopDirectoryOnly)
                    .FirstOrDefault();
            return true;
        }


        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public FileStream Open(string name)
        {
            var fullFilePath = Path.Combine(FileDirectory, name);
            return File.Open(fullFilePath,
                FileMode.Open, FileAccess.Read, FileShare.Read);
        }

        /// <summary>
        /// 获取文件长度
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public long GetLength(string name)
        {
            var fullFilePath = Path.Combine(FileDirectory, name);
            return new System.IO.FileInfo(fullFilePath).Length;
        }
    }
}
