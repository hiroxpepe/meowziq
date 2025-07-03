using System.IO;
using System.Text;

namespace Meowziq.Unity {
    /// <summary>
    /// Provides common extension methods.
    /// </summary>
    /// <author>
    /// h.adachi (STUDIO MeowToon)
    /// </author>
    public static class Extensions {
#nullable enable

        /// <summary>
        /// Converts to memory stream.
        /// </summary>
        /// <param name="source">The string to convert.</param>
        public static MemoryStream ToMemoryStream(this string source) {
            return new MemoryStream(buffer: Encoding.UTF8.GetBytes(source));
        }

        /// <summary>
        /// Gets directory name.
        /// </summary>
        /// <param name="source">The path string.</param>
        public static string ToDirectoryName(this string source) {
            return Path.GetDirectoryName(path: source);
        }

        /// <summary>
        /// Gets file name.
        /// </summary>
        /// <param name="source">The path string.</param>
        public static string ToFileName(this string source) {
            return Path.GetFileName(path: source);
        }

        /// <summary>
        /// Converts bytes to megabytes.
        /// </summary>
        /// <param name="source">The value in bytes.</param>
        public static long ToMegabytes(this long source) {
            return source / (1024 * 1024);
        }

        /// <summary>
        /// Returns true if the string is not null, an empty string "", or "undefined".
        /// </summary>
        /// <param name="source">The string to check.</param>
        public static bool HasValue(this string source) {
            return !(source is null || source.Equals(string.Empty) || source.Equals("undefined"));
        }
    }
}