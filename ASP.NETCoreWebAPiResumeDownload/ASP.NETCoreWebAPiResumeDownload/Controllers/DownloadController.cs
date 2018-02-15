using ASP.NETCoreWebAPiResumeDownload.Files;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;

namespace ASP.NETCoreWebAPiResumeDownload.Controllers
{
    public class FileDownloadController
    {
        private const int BufferSize = 80 * 1024;

        private const string MimeType = "application/octet-stream";
        public IFileProvider _fileProvider { get; set; }

        private IHttpContextAccessor _contextAccessor;
        private HttpContext _context { get { return _contextAccessor.HttpContext; } }
        public FileDownloadController(
            IFileProvider fileProvider,
            IHttpContextAccessor contextAccessor)
        {
            _fileProvider = fileProvider;
            _contextAccessor = contextAccessor;
        }


        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        [HttpGet("api/download")]
        public IActionResult GetFile(string fileName)
        {
            fileName = "cn_windows_8_1_x64_dvd_2707237.iso";

            if (!_fileProvider.Exists(fileName))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }

            //获取下载文件长度
            var fileLength = _fileProvider.GetLength(fileName);

            //初始化下载文件信息
            var fileInfo = GetFileInfoFromRequest(_context.Request, fileLength);

            //获取剩余部分文件流
            var stream = new PartialContentFileStream(_fileProvider.Open(fileName),
                                                 fileInfo.From, fileInfo.To);
            //设置响应 请求头
            SetResponseHeaders(_context.Response, fileInfo, fileLength, fileName);

            return new FileStreamResult(stream, new MediaTypeHeaderValue(MimeType));
        }


        /// <summary>
        /// 根据请求信息赋予封装的文件信息类
        /// </summary>
        /// <param name="request"></param>
        /// <param name="entityLength"></param>
        /// <returns></returns>
        private FileInfo GetFileInfoFromRequest(HttpRequest request, long entityLength)
        {
            var fileInfo = new FileInfo
            {
                From = 0,
                To = entityLength - 1,
                IsPartial = false,
                Length = entityLength
            };

            var requestHeaders = request.GetTypedHeaders();

            if (requestHeaders.Range != null && requestHeaders.Range.Ranges.Count > 0)
            {
                var range = requestHeaders.Range.Ranges.FirstOrDefault();
                if (range.From.HasValue && range.From < 0 || range.To.HasValue && range.To > entityLength - 1)
                {
                    return null;
                }

                var start = range.From;
                var end = range.To;

                if (start.HasValue)
                {
                    if (start.Value >= entityLength)
                    {
                        return null;
                    }
                    if (!end.HasValue || end.Value >= entityLength)
                    {
                        end = entityLength - 1;
                    }
                }
                else
                {
                    if (end.Value == 0)
                    {
                        return null;
                    }

                    var bytes = Math.Min(end.Value, entityLength);
                    start = entityLength - bytes;
                    end = start + bytes - 1;
                }

                fileInfo.IsPartial = true;
                fileInfo.Length = end.Value - start.Value + 1;
            }      
            return fileInfo;
        }

        /// <summary>
        /// 设置响应头信息
        /// </summary>
        /// <param name="response"></param>
        /// <param name="fileInfo"></param>
        /// <param name="fileLength"></param>
        /// <param name="fileName"></param>
        private void SetResponseHeaders(HttpResponse response, FileInfo fileInfo,
                                      long fileLength, string fileName)
        {
            response.Headers[HeaderNames.AcceptRanges] = "bytes";
            response.StatusCode = fileInfo.IsPartial ? StatusCodes.Status206PartialContent
                                      : StatusCodes.Status200OK;

            var contentDisposition = new ContentDispositionHeaderValue("attachment");
            contentDisposition.SetHttpFileName(fileName);
            response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            response.Headers[HeaderNames.ContentType] = MimeType;
            response.Headers[HeaderNames.ContentLength] = fileInfo.Length.ToString();
            if (fileInfo.IsPartial)
            {
                response.Headers[HeaderNames.ContentRange] = new ContentRangeHeaderValue(fileInfo.From, fileInfo.To, fileLength).ToString();
            }
        }
    }
}