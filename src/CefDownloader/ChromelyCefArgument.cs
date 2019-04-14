// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChromelyCefArgument.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace CefDownloader
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.Extensions.CommandLineUtils;

    /// <summary>
    /// The chromely cef argument.
    /// </summary>
    public class ChromelyCefArgument
    {
        /// <summary>
        /// The mget cef dist folder.
        /// </summary>
        private string mgetCefDistFolder;

        /// <summary>
        /// Gets or sets the chromely version.
        /// </summary>
        public string ChromelyVersion { get; set; }

        /// <summary>
        /// Gets or sets the cef binary version.
        /// </summary>
        public string CefBinaryVersion { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is win.
        /// </summary>
        public bool IsWin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is 64 bit.
        /// </summary>
        public bool Is64Bit { get; set; }

        /// <summary>
        /// Gets or sets the destination.
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// Gets the get cef dist folder.
        /// </summary>
        public string GetCefDistFolder
        {
            get
            {
                if (!string.IsNullOrEmpty(this.mgetCefDistFolder))
                {
                    return this.mgetCefDistFolder;
                }

                var os = this.IsWin ? "windows" : "linux";
                var cpu = this.Is64Bit ? "64" : "32";
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;
                var currentFolder = Path.GetDirectoryName(new Uri(codeBase).LocalPath);
                var cefDistFolderName = $"{this.CefBinaryVersion}_{os}{cpu}";
                var directoryInfo = Directory.GetParent(currentFolder);
                if (directoryInfo != null)
                {
                    currentFolder =
                        (!string.IsNullOrEmpty(directoryInfo.FullName) && Directory.Exists(directoryInfo.FullName))
                            ? directoryInfo.FullName
                            : currentFolder;
                }

                this.mgetCefDistFolder = Path.Combine(currentFolder, "CEF", cefDistFolderName);
                return this.mgetCefDistFolder;
            }
        }

        /// <summary>
        /// The set chromely version.
        /// </summary>
        /// <param name="argument">
        /// The argument.
        /// </param>
        public void SetChromelyVersion(CommandArgument argument)
        {
            if (argument != null)
            {
                this.ChromelyVersion = argument?.Value;
            }
        }

        /// <summary>
        /// The set binary version.
        /// </summary>
        /// <param name="template">
        /// The template.
        /// </param>
        /// <param name="option">
        /// The option.
        /// </param>
        public void SetOption(string template, CommandOption option)
        {
            switch (template)
            {
                case Template.BinaryVersion:
                    this.CefBinaryVersion = option.Value();
                    break;
                case Template.OS:
                    this.IsWin = true;
                    string osoption = !string.IsNullOrEmpty(option.Value()) ? option.Value() : string.Empty;
                    if (!string.IsNullOrEmpty(osoption))
                    {
                        this.IsWin = osoption.Equals("win", StringComparison.InvariantCultureIgnoreCase);
                    }
                    break;
                case Template.Cpu:
                    string cpuOption = option.Value();
                    if (!string.IsNullOrEmpty(cpuOption))
                    {
                        if (cpuOption.ToLower().Contains("any"))
                        {
                            this.Is64Bit = Environment.Is64BitOperatingSystem;
                        }
                        else
                        {
                            this.Is64Bit = cpuOption.Equals("x64", StringComparison.InvariantCultureIgnoreCase);
                        }
                    }
                    else
                    {
                        this.Is64Bit = Environment.Is64BitOperatingSystem;
                    }
                    break;
                case Template.Destination:
                    this.Destination = this.GetDestination(option.Value());
                    break;
            }
        }

        /// <summary>
        /// The to string.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public override string ToString()
        {
            string toString = "chromely version: " + (this.ChromelyVersion ?? string.Empty) + "\n\r"
                              + "cef binary version: " + (this.CefBinaryVersion ?? string.Empty) + "\n\r"
                              + "cpu: " + (this.Is64Bit ? "x64" : "x86") + "\n\r"
                              + "destination: " + (this.Destination ?? string.Empty);

            return toString;
        }

        /// <summary>
        /// The get destination.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetDestination(string path)
        {
            string removedIllegalChars = path?.Replace("'", string.Empty).Replace("\"", string.Empty).TrimEnd('\\').TrimEnd('\\');
            if (!string.IsNullOrEmpty(removedIllegalChars))
            {
                bool isFullPath = this.IsFullPath(removedIllegalChars);
                if (isFullPath)
                {
                    return removedIllegalChars;
                }

                var currentFolder = Environment.CurrentDirectory;
                return Path.Combine(currentFolder, removedIllegalChars);
            }

            return Environment.CurrentDirectory;
        }

        /// <summary>
        /// The is full path.
        /// </summary>
        /// <param name="path">
        /// The path.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool IsFullPath(string path)
        {
            try
            {
                return Path.GetFullPath(path) == path;
            }
            catch
            {
                return false;
            }
        }
    }
}
