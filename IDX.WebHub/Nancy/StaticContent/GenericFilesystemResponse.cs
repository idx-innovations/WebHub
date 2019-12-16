//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

//import namespace(s) required from 'NancyFX' framework
using Nancy;
using Nancy.Configuration;
using Nancy.Helpers;

namespace IDX.WebHub.Nancy.StaticContent
{

    /// <summary>
    /// response class for static content from external location(s)
    /// </summary>
    /// <remarks>based on the articles found at 'https://stackoverflow.com/questions/14439424/nancyfx-serving-static-files-from-a-parent-folder' and 'https://gist.github.com/elliotwoods/4822f9c512b4f464534d9e01eb6d21b1'</remarks>
    public class GenericFilesystemResponse : Response
    {
        private readonly StaticContentConfiguration configuration;

        /// <summary>
        ///  Size of buffer for transmitting file. Default size 4 Mb
        /// </summary>
        public static int BufferSize = 4 * 1024 * 1024;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <paramref name="filePath"/> parameter and <paramref name="context"/>.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <remarks>The <see cref="MimeTypes.GetMimeType"/> method will be used to determine the mimetype of the file and will be used as the content-type of the response. If no match if found the content-type will be set to application/octet-stream.</remarks>
        /// <param name="context">Current context</param>
        public GenericFilesystemResponse(string filePath, NancyContext context)
            : this(filePath, MimeTypes.GetMimeType(filePath), context)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericFileResponse"/> for the file specified
        /// by the <paramref name="filePath"/> parameter, the content-type specified by the <paramref name="contentType"/> parameter 
        /// and <paramref name="context"/>.
        /// </summary>
        /// <param name="filePath">The name of the file, including path relative to the root of the application, that should be returned.</param>
        /// <param name="contentType">The content-type of the response.</param>
        /// <param name="context">Current context</param>
        public GenericFilesystemResponse(string filePath, string contentType, NancyContext context)
        {
            var environment = context.Environment;
            this.configuration = environment.GetValue<StaticContentConfiguration>();
            this.InitializeGenericFileResponse(filePath, contentType, context);
        }

        /// <summary>
        /// Gets the filename of the file response
        /// </summary>
        /// <value>A string containing the name of the file.</value>
        public string Filename { get; protected set; }

        private static Action<Stream> GetFileContent(string filePath, long length)
        {
            return stream =>
            {
                using (var file = File.OpenRead(filePath))
                {
                    file.CopyTo(stream, (int)(length < BufferSize ? length : BufferSize));
                }
            };
        }

        /*
		 * REMOVE FOR UNSAFE
		static bool IsSafeFilePath(string rootPath, string filePath)
		{
			if (!File.Exists(filePath))
			{
				return false;
			}

			var fullPath = Path.GetFullPath(filePath);

			return fullPath.StartsWith(Path.GetFullPath(rootPath), StringComparison.OrdinalIgnoreCase);
		}
		*/

        private void InitializeGenericFileResponse(string filePath, string contentType, NancyContext context)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                StatusCode = HttpStatusCode.NotFound;
                return;
            }

            if (this.configuration.SafePaths == null || !this.configuration.SafePaths.Any())
            {
                throw new InvalidOperationException("No SafePaths defined.");
            }
            foreach (var rootPath in this.configuration.SafePaths)
            {
                string fullPath;
                if (Path.IsPathRooted(filePath))
                {
                    fullPath = filePath;
                }
                else
                {
                    fullPath = Path.Combine(rootPath, filePath);
                }

                /* 
				 * REMOVE FOR UNSAFE
				if (IsSafeFilePath(rootPath, fullPath))
				{
				*/
                if (true)
                {
                    this.Filename = Path.GetFileName(fullPath);

                    this.SetResponseValues(contentType, fullPath, context);

                    return;
                }
            }

            StatusCode = HttpStatusCode.NotFound;
        }

        private void SetResponseValues(string contentType, string fullPath, NancyContext context)
        {
            // TODO - set a standard caching time and/or public?
            var fi = new FileInfo(fullPath);

            var lastWriteTimeUtc = fi.LastWriteTimeUtc;
            var etag = string.Concat("\"", lastWriteTimeUtc.Ticks.ToString("x"), "\"");
            var lastModified = lastWriteTimeUtc.ToString("R");
            var length = fi.Length;

            if (CacheHelpers.ReturnNotModified(etag, lastWriteTimeUtc, context))
            {
                this.StatusCode = HttpStatusCode.NotModified;
                this.ContentType = null;
                this.Contents = NoBody;

                return;
            }

            this.Headers["ETag"] = etag;
            this.Headers["Last-Modified"] = lastModified;
            this.Headers["Content-Length"] = length.ToString();

            if (length > 0)
            {
                this.Contents = GetFileContent(fullPath, length);
            }

            this.ContentType = contentType;
            this.StatusCode = HttpStatusCode.OK;
        }
    }

}
