// --------------------------------------------------------------------------------------------------------------------
// <copyright file="VersionMapper.cs" company="Chromely Projects">
//   Copyright (c) 2017-2018 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace CefDownloader
{
    /// <summary>
    /// The version mapper.
    /// </summary>
    public static class VersionMapper
    {
        /// <summary>
        /// The url template
        /// </summary>
        private const string UrlTemplate = @"http://opensource.spotify.com/cefbuilds/cef_binary_{0}_{1}{2}_client.tar.bz2";

        /// <summary>
        /// The get url.
        /// </summary>
        /// <param name="arguments">
        /// The arguments.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetUrl(ChromelyCefArgument arguments)
        {
            string platform = arguments.IsWin ? "windows" : "linux";
            string cpu = arguments.Is64Bit ? "64" : "32";
            string cefVersion = arguments.CefBinaryVersion;
            if (string.IsNullOrEmpty(cefVersion))
            {
                cefVersion = GetCefVersion(arguments.ChromelyVersion);
            }

            return string.Format(UrlTemplate, cefVersion, platform, cpu);
        }

        /// <summary>
        /// The get cef version.
        /// </summary>
        /// <param name="version">
        /// The version.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string GetCefVersion(string version)
        {
            switch (version)
            {
                case "v70":
                    return "3.3538.1849.g458cc98";
                case "v68":
                    return "3.3440.1805.gbe070f9";
                case "v66":
                    return "3.3359.1772.gd1df190";
                case "v65":
                    return "3.3325.1751.ge5b78a5";
                case "v64":
                    return "3.3282.1741.gcd94615";
                case "v63":
                    return "3.3239.1723.g071d1c1";
                case "v59":
                    return "3.3071.1646.gbb29707";
            }

            return "3.3359.1772.gd1df190";
        }
    }
}
