using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace LibroLib.Text
{
    public static class FormattingUtils
    {
        /// <summary>
        /// Formats a size in bytes into string using the optimal human readable format.
        /// </summary>
        /// <param name="byteSize">
        /// Size in bytes.
        /// </param>
        /// <param name="culture">
        /// The culture to use for formatting.
        /// </param>
        /// <returns>
        /// Formatted size in bytes.
        /// </returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte")]
        public static string FormatByteSizeToString(long byteSize, CultureInfo culture = null)
        {
            Contract.Requires(byteSize != -9223372036854775808);
            Contract.Ensures(Contract.Result<string>() != null);

            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            bool isNegative = byteSize < 0;
            double size = Math.Abs(byteSize);

            if (size < 1024)
                return string.Format(culture, "{1}{0:n1} B", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n1} KB", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n1} MB", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n1} GB", size, isNegative ? "-" : null);
            size /= 1024;
            return string.Format(culture, "{1}{0:n1} TB", size, isNegative ? "-" : null);
        }

        /// <summary>
        /// Formats a size in bytes into string using the optimal human readable format.
        /// </summary>
        /// <param name="byteSize">Size in bytes.</param>
        /// <param name="culture">
        /// The culture to use for formatting.
        /// </param>
        /// <returns>Formatted size in bytes.</returns>
        [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "byte")]
        public static string FormatByteSizeRoundedToString(long byteSize, CultureInfo culture = null)
        {
            Contract.Requires(byteSize != -9223372036854775808);
            Contract.Ensures(Contract.Result<string>() != null);

            if (culture == null)
                culture = CultureInfo.InvariantCulture;

            bool isNegative = byteSize < 0;
            double size = Math.Abs(byteSize);

            if (size < 1024)
                return string.Format(culture, "{1}{0:n0} B", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n0} KB", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n0} MB", size, isNegative ? "-" : null);
            size /= 1024;
            if (size < 1024)
                return string.Format(culture, "{1}{0:n0} GB", size, isNegative ? "-" : null);
            size /= 1024;
            return string.Format(culture, "{1}{0:n0} TB", size, isNegative ? "-" : null);
        }

        public static string FileSortableFormat(this DateTime dateTime)
        {
            Contract.Ensures(Contract.Result<string>() != null);

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0}-{1}-{2}_{3}-{4}-{5}",
                dateTime.ToString("yyyy", CultureInfo.InvariantCulture),
                dateTime.ToString("MM", CultureInfo.InvariantCulture),
                dateTime.ToString("dd", CultureInfo.InvariantCulture),
                dateTime.ToString("HH", CultureInfo.InvariantCulture),
                dateTime.ToString("mm", CultureInfo.InvariantCulture),
                dateTime.ToString("ss", CultureInfo.InvariantCulture));
        }
    }
}
