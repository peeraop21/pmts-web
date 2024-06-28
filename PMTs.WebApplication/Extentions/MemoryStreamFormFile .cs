using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace PMTs.WebApplication.Extentions
{
    public class MemoryStreamFormFile : IFormFile
    {
        private readonly MemoryStream _stream;
        private readonly string _fileName;
        private readonly string _contentType;

        public MemoryStreamFormFile(MemoryStream stream, string fileName, string contentType = null)
        {
            _stream = stream;
            _fileName = fileName;
            _contentType = contentType ?? "application/octet-stream"; // Default to binary data if content type is not provided
        }

        public string ContentType => _contentType;

        public string ContentDisposition => $"form-data; name=\"file\"; filename=\"{_fileName}\"";

        public IHeaderDictionary Headers => new HeaderDictionary();

        public long Length => _stream.Length;

        public string Name => "file";

        public void CopyTo(Stream target)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            _stream.CopyTo(target);
        }

        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            _stream.Seek(0, SeekOrigin.Begin);
            return _stream.CopyToAsync(target, (int)_stream.Length, cancellationToken);
        }

        public Stream OpenReadStream() => _stream;

        public string FileName => _fileName;
    }
}
