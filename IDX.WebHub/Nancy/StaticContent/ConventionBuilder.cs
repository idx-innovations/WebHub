//import .net namespace(s) required
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;

//import namespace(s) required from 'NancyFX' framework
using Nancy.Helpers;
using Nancy;
using Nancy.Diagnostics;

namespace IDX.WebHub.Nancy.StaticContent
{

    /// <summary>
    /// convention builder class for static content from external location(s)
    /// </summary>
    /// <remarks>based on the articles found at 'https://stackoverflow.com/questions/14439424/nancyfx-serving-static-files-from-a-parent-folder' and 'https://gist.github.com/elliotwoods/4822f9c512b4f464534d9e01eb6d21b1'</remarks>
    public class ConventionBuilder
    {

        #region private variable(s)

        private static readonly ConcurrentDictionary<ResponseFactoryCacheKey, Func<NancyContext, Response>> ResponseFactoryCache;
        private static readonly Regex PathReplaceRegex = new Regex(@"[/\\]", RegexOptions.Compiled);

        #endregion

        static ConventionBuilder()
        {
            ResponseFactoryCache = new ConcurrentDictionary<ResponseFactoryCacheKey, Func<NancyContext, Response>>();
        }

        #region shared method(s)

        /// <summary>
        /// Adds a directory-based convention for static convention.
        /// </summary>
        /// <param name="requestedPath">The path that should be matched with the request.</param>
        /// <param name="contentPath">The path to where the content is stored in your application, relative to the root. If this is <see langword="null" /> then it will be the same as <paramref name="requestedPath"/>.</param>
        /// <param name="allowedExtensions">A list of extensions that is valid for the conventions. If not supplied, all extensions are valid.</param>
        /// <returns>A <see cref="UnsafeGenericFileResponse"/> instance for the requested static contents if it was found, otherwise <see langword="null"/>.</returns>
        public static Func<NancyContext, string, Response> AddDirectory(string requestedPath, string contentPath = null, params string[] allowedExtensions)
        {
            if (!requestedPath.StartsWith("/"))
            {
                requestedPath = string.Concat("/", requestedPath);
            }

            return (ctx, root) =>
            {
                var path =
                    HttpUtility.UrlDecode(ctx.Request.Path);

                var fileName = GetSafeFileName(path);

                if (string.IsNullOrEmpty(fileName))
                {
                    return null;
                }

                var pathWithoutFilename =
                    GetPathWithoutFilename(fileName, path);

                if (!pathWithoutFilename.StartsWith(requestedPath, StringComparison.OrdinalIgnoreCase))
                {
                    (ctx.Trace.TraceLog ?? new NullLog()).WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested resource '", path, "' does not match convention mapped to '", requestedPath, "'")));
                    return null;
                }

                contentPath =
                    GetContentPath(requestedPath, contentPath);

                if (contentPath.Equals("/"))
                {
                    throw new ArgumentException("This is not the security vulnerability you are looking for. Mapping static content to the root of your application is not a good idea.");
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(new ResponseFactoryCacheKey(path, root), BuildContentDelegate(ctx, root, requestedPath, contentPath, allowedExtensions));

                return responseFactory.Invoke(ctx);
            };

        }

        /// <summary>
        /// Adds a file-based convention for static convention.
        /// </summary>
        /// <param name="requestedFile">The file that should be matched with the request.</param>
        /// <param name="contentFile">The file that should be served when the requested path is matched.</param>
        public static Func<NancyContext, string, Response> AddFile(string requestedFile, string contentFile)
        {
            return (ctx, root) =>
            {

                var path =
                    ctx.Request.Path;

                if (!path.Equals(requestedFile, StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested resource '", path, "' does not match convention mapped to '", requestedFile, "'")));
                    return null;
                }

                var responseFactory =
                    ResponseFactoryCache.GetOrAdd(new ResponseFactoryCacheKey(path, root), BuildContentDelegate(ctx, root, requestedFile, contentFile, ArrayCache.Empty<string>()));

                return responseFactory.Invoke(ctx);
            };
        }

        /// <summary>
        /// method validating a path given for being a file path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsFile(string path)
        {

            //check for a file pattern
            //Regex myRegex = new Regex(@"[\/|\\]\w*.\w*$", RegexOptions.IgnoreCase);
            Regex myRegex = new Regex(@"\\\w*\.\w*", RegexOptions.IgnoreCase);

            foreach (Match myMatch in myRegex.Matches(path))
            {
                if (myMatch.Success)
                {
                    return true;
                }
            }

            //return the default output value
            return false;

        }

        #endregion

        #region private method(s)

        private static string GetSafeFullPath(string path)
        {
            try
            {
                return Path.GetFullPath(path);
            }
            catch (Exception)
            {
            }

            return null;
        }

        private static string GetContentPath(string requestedPath, string contentPath)
        {
            contentPath =
                contentPath ?? requestedPath;

            if (!contentPath.StartsWith("/"))
            {
                contentPath = string.Concat("/", contentPath);
            }

            return contentPath;
        }

        private static Func<ResponseFactoryCacheKey, Func<NancyContext, Response>> BuildContentDelegate(NancyContext context, string applicationRootPath, string requestedPath, string contentPath, string[] allowedExtensions)
        {
            return pathAndRootPair =>
            {
                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Attempting to resolve static content '", pathAndRootPair, "'")));

                var extension =
                    Path.GetExtension(pathAndRootPair.Path);

                if (!string.IsNullOrEmpty(extension))
                {
                    extension = extension.Substring(1);
                }

                if (allowedExtensions.Length != 0 && !allowedExtensions.Any(e => string.Equals(e.TrimStart(new[] { '.' }), extension, StringComparison.OrdinalIgnoreCase)))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested extension '", extension, "' does not match any of the valid extensions for the convention '", string.Join(",", allowedExtensions), "'")));
                    return ctx => null;
                }

                var transformedRequestPath =
                    GetSafeRequestPath(pathAndRootPair.Path, requestedPath, contentPath);

                transformedRequestPath =
                    GetEncodedPath(transformedRequestPath);

                var relativeFileName =
                    Path.Combine(applicationRootPath, transformedRequestPath);

                var fileName =
                    GetSafeFullPath(relativeFileName);

                if (fileName == null)
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The request '", relativeFileName, "' contains an invalid path character")));
                    return ctx => null;
                }

                var relatveContentRootPath =
                    Path.Combine(applicationRootPath, GetEncodedPath(contentPath));

                var contentRootPath =
                    GetSafeFullPath(relatveContentRootPath);

                if (contentRootPath == null)
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The request '", fileName, "' is trying to access a path inside the content folder, which contains an invalid path character '", relatveContentRootPath, "'")));
                    return ctx => null;
                }

                /*
				* REMOVE FOR UNSAFE
				if (!IsWithinContentFolder(contentRootPath, fileName))
				{
					context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The request '", fileName, "' is trying to access a path outside the content folder '", contentPath, "'")));
					return ctx => null;
				}
				*/

                if (!File.Exists(fileName))
                {
                    context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] The requested file '", fileName, "' does not exist")));
                    return ctx => null;
                }

                context.Trace.TraceLog.WriteLog(x => x.AppendLine(string.Concat("[StaticContentConventionBuilder] Returning file '", fileName, "'")));
                return ctx => new GenericFilesystemResponse(fileName, ctx);
            };
        }

        private static string GetEncodedPath(string path)
        {
            return PathReplaceRegex.Replace(path.TrimStart(new[] { '/' }), Path.DirectorySeparatorChar.ToString());
        }

        private static string GetPathWithoutFilename(string fileName, string path)
        {
            var pathWithoutFileName =
                path.Replace(fileName, string.Empty);

            return (pathWithoutFileName.Equals("/")) ?
                pathWithoutFileName :
                pathWithoutFileName.TrimEnd(new[] { '/' });
        }

        private static string GetSafeRequestPath(string requestPath, string requestedPath, string contentPath)
        {
            var actualContentPath =
                (contentPath.Equals("/") ? string.Empty : contentPath);

            if (requestedPath.Equals("/"))
            {
                return string.Concat(actualContentPath, requestPath);
            }

            var expression =
                new Regex(Regex.Escape(requestedPath), RegexOptions.IgnoreCase);

            return expression.Replace(requestPath, actualContentPath, 1);
        }

        /*
		 * REMOVE FOR UNSAFE
		/// <summary>
		/// Returns whether the given filename is contained within the content folder
		/// </summary>
		/// <param name="contentRootPath">Content root path</param>
		/// <param name="fileName">Filename requested</param>
		/// <returns>True if contained within the content root, false otherwise</returns>
		private static bool IsWithinContentFolder(string contentRootPath, string fileName)
		{
			return fileName.StartsWith(contentRootPath, StringComparison.Ordinal);
		}
		*/

        private static string GetSafeFileName(string path)
        {
            try
            {
                return Path.GetFileName(path);
            }
            catch (Exception)
            {
            }

            return null;
        }

        #endregion

        /// <summary>
        /// Used to uniquely identify a request. Needed for when two Nancy applications want to serve up static content of the same
        /// name from within the same AppDomain.
        /// </summary>
        private class ResponseFactoryCacheKey : IEquatable<ResponseFactoryCacheKey>
        {
            private readonly string path;
            private readonly string rootPath;

            public ResponseFactoryCacheKey(string path, string rootPath)
            {
                this.path = path;
                this.rootPath = rootPath;
            }

            /// <summary>
            /// The path of the static content for which this response is being issued
            /// </summary>
            public string Path
            {
                get { return this.path; }
            }

            /// <summary>
            /// The root folder path of the Nancy application for which this response will be issued
            /// </summary>
            public string RootPath
            {
                get { return this.rootPath; }
            }

            public bool Equals(ResponseFactoryCacheKey other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }

                if (ReferenceEquals(this, other))
                {
                    return true;
                }

                return string.Equals(this.path, other.path) && string.Equals(this.rootPath, other.rootPath);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() != this.GetType())
                {
                    return false;
                }

                return Equals((ResponseFactoryCacheKey)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    return ((this.path != null ? this.path.GetHashCode() : 0) * 397) ^ (this.rootPath != null ? this.rootPath.GetHashCode() : 0);
                }
            }
        }
    }

}
