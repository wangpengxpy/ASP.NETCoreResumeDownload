using System;

namespace ASP.NETCoreWebAPiResumeDownload.Files
{
    public class FileInfo : Exception
    {
        public long From;
        public long To;
        public bool IsPartial;
        public long Length;
    }
}
