// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DownloadService.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace CefDownloaderNetCore
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading;
    using ICSharpCode.SharpZipLib.BZip2;
    using ICSharpCode.SharpZipLib.Tar;

    /// <summary>
    /// The cef binaries downloader.
    /// </summary>
    public class DownloadService
    {
        /// <summary>
        /// The process.
        /// </summary>
        /// <param name="commandLineArguments">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool Process(ChromelyCefArgument commandLineArguments)
        {
            bool result = false;

            try
            {
                string downloadClientUrl = VersionMapper.GetUrl(commandLineArguments);
                Console.WriteLine($"From url: {downloadClientUrl}");
                string destFolder = commandLineArguments.Destination;

                if (!Directory.Exists(destFolder))
                {
                    Directory.CreateDirectory(destFolder);
                }

                Console.WriteLine($"Destination: {destFolder}");

                Console.WriteLine("Processing ... please wait ...");

                var finalUnzippedName = this.UnzippedFolderName(downloadClientUrl);

                var bz2FileName = string.Format("{0}.tar.bz2", finalUnzippedName);
                var tarFileName = string.Format("{0}.tar", finalUnzippedName);

                var guid = Guid.NewGuid().ToString();
                var tempFolder = Path.Combine(Path.GetTempPath(), guid);

                if (!Directory.Exists(tempFolder))
                {
                    Directory.CreateDirectory(tempFolder);
                }

                var downloadedBz2File = Path.Combine(tempFolder, bz2FileName);
                var finalUnzippedFolder = Path.Combine(tempFolder, finalUnzippedName);

                var webClient = new WebClient();
                webClient.DownloadFile(downloadClientUrl, downloadedBz2File);

                var tarFile = Path.Combine(tempFolder, tarFileName);

                var bz2FileInfo = new FileInfo(downloadedBz2File);
                using (var fileToDecompressAsStream = bz2FileInfo.OpenRead())
                {
                    using (var decompressedStream = File.Create(tarFile))
                    {
                        BZip2.Decompress(fileToDecompressAsStream, decompressedStream, true);
                    }
                }

                using (var inStream = File.OpenRead(tarFile))
                {
                    TarArchive tarArchive = TarArchive.CreateInputTarArchive(inStream);
                    tarArchive.ExtractContents(tempFolder);
                    tarArchive.Close();
                    inStream.Close();
                }

                var filesToCopyInfos = this.GetFileInfosToCopy(finalUnzippedFolder, destFolder, commandLineArguments.IsWin);

                this.CopyFiles(filesToCopyInfos);

                // Now copy all cef binaries to dist folder
                var copyCefBinariestoDistThread = new Thread(() => this.CopyCefBinariesToDistFolder(commandLineArguments));
                copyCefBinariestoDistThread.Start();

                // Delete temp folder
                var deleteTempFolderThread = new Thread(() => this.DeleteTempFolder(tempFolder));
                deleteTempFolderThread.Start();
                result = true;
            }
            catch (Exception exception)
            {
                var message = exception.GetBaseException().Message;
                Console.Error.WriteLine(message);

                // ignored
                result = false;
            }

            return result;
        }

        /// <summary>
        /// The cef binaries exist.
        /// </summary>
        /// <param name="commandLineArguments">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CefBinariesExist(ChromelyCefArgument commandLineArguments)
        {
            try
            {
                return this.CefBinariesExist(commandLineArguments.Destination, commandLineArguments.IsWin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// The copy cef binaries if exist.
        /// </summary>
        /// <param name="commandLineArguments">
        /// The command line arguments.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CopyCefBinariesIfExist(ChromelyCefArgument commandLineArguments)
        {
            try
            {
                if (!this.CefBinariesExist(commandLineArguments.GetCefDistFolder, commandLineArguments.IsWin))
                {
                    return false;
                }

                Console.WriteLine("Copying Cef binaries from {0} to {1}", commandLineArguments.GetCefDistFolder, commandLineArguments.Destination);

                // Copy all files from dist folder to bin folder.
                this.DirectoryCopy(commandLineArguments.GetCefDistFolder, commandLineArguments.Destination);

                return this.CefBinariesExist(commandLineArguments.Destination, commandLineArguments.IsWin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// The cef binaries exist.
        /// </summary>
        /// <param name="folder">
        /// The destination folder.
        /// </param>
        /// <param name="isWin">
        /// The is win.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private bool CefBinariesExist(string folder, bool isWin)
        {
            try
            {
                if (!Directory.Exists(folder))
                {
                    return false;
                }

                var localesFolder = Path.Combine(folder, "locales");
                if (!Directory.Exists(localesFolder))
                {
                    return false;
                }

                var cefBinaryFiles = new List<string>
                                         {
                                             "cef_100_percent.pak",
                                             "cef_200_percent.pak",
                                             "cef_extensions.pak",
                                             "devtools_resources.pak",
                                             "icudtl.dat",
                                             "natives_blob.bin",
                                             "snapshot_blob.bin",
                                             "v8_context_snapshot.bin"
                                         };

                if (isWin)
                {
                    // These first two may no longer be required.
                    // cefBinaryFiles.Add("chrome_elf.dll");
                    // cefBinaryFiles.Add("widevinecdmadapter.dll");
                    cefBinaryFiles.Add("libcef.dll");
                    cefBinaryFiles.Add("libEGL.dll");
                    cefBinaryFiles.Add("libGLESv2.dll");
                }
                else
                {
                    cefBinaryFiles.Add("libcef.so");
                    cefBinaryFiles.Add("libEGL.so");
                    cefBinaryFiles.Add("libGLESv2.so");
                }

                foreach (var file in cefBinaryFiles)
                {
                    string fullFilePath = Path.Combine(folder, file);
                    if (!File.Exists(fullFilePath))
                    {
                        return false;
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// The directory copy.
        /// </summary>
        /// <param name="sourceDirName">
        /// The source dir name.
        /// </param>
        /// <param name="destDirName">
        /// The dest dir name.
        /// </param>
        private void DirectoryCopy(string sourceDirName, string destDirName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath);
            }
        }

        /// <summary>
        /// The copy files.
        /// </summary>
        /// <param name="filesToCopyInfos">
        /// The files to copy infos.
        /// </param>
        private void CopyFiles(List<FileCopyInfo> filesToCopyInfos)
        {
            try
            {
                foreach (FileCopyInfo copyInfo in filesToCopyInfos)
                {
                    try
                    {
                        FileInfo fileInfo = new FileInfo(copyInfo.CopyFrom);
                        fileInfo.CopyTo(copyInfo.CopyTo, true);
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// The get file infos to copy.
        /// </summary>
        /// <param name="finalUnzippedFolder">
        /// The final unzipped folder.
        /// </param>
        /// <param name="destFolder">
        /// The dest folder.
        /// </param>
        /// <param name="isWindows">
        /// The is windows.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        private List<FileCopyInfo> GetFileInfosToCopy(string finalUnzippedFolder, string destFolder, bool isWindows)
        {
            string libExtension = isWindows ? ".dll" : ".so";

            string releaseFolder = Path.Combine(finalUnzippedFolder, "Release");
            string releaseLocalesFolder = Path.Combine(releaseFolder, "locales");
            string destLocalesFolder = Path.Combine(destFolder, "locales");

            if (!Directory.Exists(destLocalesFolder))
            {
                Directory.CreateDirectory(destLocalesFolder);
            }

            // List of files to copy
            var filesToCopy = new List<FileCopyInfo>();

            // CEF core libraries
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "libcef" + libExtension, destFolder, "libcef" + libExtension));

            // Angle and Direct3D support
            // Note: Without these components HTML5 accelerated content like 2D canvas, 3D CSS and WebGL will not function.
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "libEGL" + libExtension, destFolder, "libEGL" + libExtension));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "libGLESv2" + libExtension, destFolder, "libGLESv2" + libExtension));

            // Unicode support
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "icudtl.dat", destFolder, "icudtl.dat"));

            // V8 native mapping files, see
            // https://groups.google.com/a/chromium.org/forum/#!topic/chromium-packagers/75J9Y1vIc_E
            // http://www.magpcss.org/ceforum/viewtopic.php?f=6&t=12580
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "natives_blob.bin", destFolder, "natives_blob.bin"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "snapshot_blob.bin", destFolder, "snapshot_blob.bin"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "v8_context_snapshot.bin", destFolder, "v8_context_snapshot.bin"));

            if (isWindows)
            {
                // Crashpad support
                filesToCopy.Add(new FileCopyInfo(releaseFolder, "chrome_elf.dll", destFolder, "chrome_elf.dll"));

                filesToCopy.Add(new FileCopyInfo(releaseFolder, "widevinecdmadapter.dll", destFolder, "widevinecdmadapter.dll"));
            }

            // Pack Files
            // Note: Contains WebKit image and inspector resources.
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "devtools_resources.pak", destFolder, "devtools_resources.pak"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "cef.pak", destFolder, "cef.pak"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "cef_extensions.pak", destFolder, "cef_extensions.pak"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "cef_100_percent.pak", destFolder, "cef_100_percent.pak"));
            filesToCopy.Add(new FileCopyInfo(releaseFolder, "cef_200_percent.pak", destFolder, "cef_200_percent.pak"));

            filesToCopy.Add(new FileCopyInfo(finalUnzippedFolder, "LICENSE.txt", destFolder, "LICENSE.txt"));
            filesToCopy.Add(new FileCopyInfo(finalUnzippedFolder, "README.txt", destFolder, "README.txt"));

            filesToCopy.Add(new FileCopyInfo(releaseLocalesFolder, "en-US.pak", destLocalesFolder, "en-US.pak"));
            filesToCopy.Add(new FileCopyInfo(releaseLocalesFolder, "en-US.pak.info", destLocalesFolder, "en-US.pak.info"));

            return filesToCopy;
        }

        /// <summary>
        /// The unzipped folder name.
        /// </summary>
        /// <param name="url">
        /// The url.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string UnzippedFolderName(string url)
        {
            string[] urlSplit = url.Split('/');
            string lastPart = urlSplit[urlSplit.Length - 1];
            return lastPart.Substring(0, lastPart.Length - ".tar.bz2".Length);
        }

        /// <summary>
        /// The copy cef binaries to dist folder.
        /// </summary>
        /// <param name="commandLineArguments">
        /// The command line arguments.
        /// </param>
        private void CopyCefBinariesToDistFolder(ChromelyCefArgument commandLineArguments)
        {
            try
            {
                string sourceDir = commandLineArguments.Destination;
                string destDir = commandLineArguments.GetCefDistFolder;
                DirectoryCopy(sourceDir, destDir);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// The delete temp folder.
        /// </summary>
        /// <param name="folderToDelete">
        /// The folder to delete.
        /// </param>
        private void DeleteTempFolder(string folderToDelete)
        {
            try
            {
                Directory.Delete(folderToDelete, true);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        /// <summary>
        /// The file copy info.
        /// </summary>
        private class FileCopyInfo
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FileCopyInfo"/> class.
            /// </summary>
            /// <param name="fromFolder">
            /// The from folder.
            /// </param>
            /// <param name="fromFileName">
            /// The from file name.
            /// </param>
            /// <param name="toFolder">
            /// The to folder.
            /// </param>
            /// <param name="toFileName">
            /// The to file name.
            /// </param>
            public FileCopyInfo(string fromFolder, string fromFileName, string toFolder, string toFileName)
            {
                this.CopyFrom = Path.Combine(fromFolder, fromFileName);
                this.CopyTo = Path.Combine(toFolder, toFileName);
            }

            /// <summary>
            /// Gets the copy from.
            /// </summary>
            public string CopyFrom { get; }

            /// <summary>
            /// Gets the copy to.
            /// </summary>
            public string CopyTo { get; }
        }
    }
}
