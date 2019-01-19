// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Template.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace CefDownloaderNetCore
{
    /// <summary>
    /// The template.
    /// </summary>
    public static class Template
    {
        /// <summary>
        /// The none.
        /// </summary>
        public const string None = "";

        /// <summary>
        /// The chromely version.
        /// </summary>
        public const string ChromelyVersion = "[chromely version]";

        /// <summary>
        /// The binary version.
        /// </summary>
        public const string BinaryVersion = "-v|--cef-binary-version";

        /// <summary>
        /// The os.
        /// </summary>
        public const string OS = "-o|--os";

        /// <summary>
        /// The cpu.
        /// </summary>
        public const string Cpu = "-c|--cpu";

        /// <summary>
        /// The destination.
        /// </summary>
        public const string Destination = "-d|--dest";

        /// <summary>
        /// The description.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetDescription(string template)
        {
            string description = string.Empty;
            switch (template)
            {
                case Template.ChromelyVersion:
                    description =
                        "The chromely app version. e.g: Chromely.CefGlue.Winapi -Version 65.0.0; argument = \"v65\".";
                    break;
                case Template.BinaryVersion:
                    description =
                        "The cef binary version. e.g: \"3.3325.1751.ge5b78a5\". If provideed, it will override the chromely version argument.";
                    break;
                case Template.OS:
                    description =
                        "The operating system. Windows or Linux. Options are \"win\" and \"linux\". If not provided chromelycef will use current running system.";
                    break;
                case Template.Cpu:
                    description =
                        "The system cpu. Options are \"x86\" and \"x64\". If not provided chromelycef will determine that.";
                    break;
                case Template.Destination:
                    description =
                        "The destination of the unpacked cef binaries. A relative path can be provided or full path. If not provided, current location will be assumed.";
                    break;
            }

            return description;
        }
    }
}
