// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//     MIT
// </license>
// --------------------------------------------------------------------------------------------------------------------

namespace CefDownloader
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using Microsoft.Extensions.CommandLineUtils;

    /// <summary>
    /// The program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int Main(string[] args)
        {
            try
            {
                ResolveCommandLineUtilsDll();
                return Run(args);
            }
            catch (Exception e)
            {
                var message = e.GetBaseException().Message;
                Console.Error.WriteLine(message);
                return 0xbad;
            }
        }

        /// <summary>
        /// The run.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        private static int Run(string[] args)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var app = new CommandLineApplication(throwOnUnexpectedArg: false);
            app.Name = "cef";
            app.HelpOption("-?|-h|--help");

            string description =
                "chromelycef downloads cef binaries from http://opensource.spotify.com/cefbuilds/index.html, unpacks the compressed binaries and move files to a specfified location or current directory.\n\r"
                + "This tool only download files that are required for a Chromely app.\n\r"
                + "For more info please go to https://github.com/mattkol/Chromely/wiki/Cef-Binaries-Download.\n\r\n\r"
                + "Note that depending on your network, this might take up to 90 seconds to complete.";

            app.OnExecute(() =>
            {
                var noCommandInfo =
                    "Missing command.\n\r"
                    + "To run you need the \"download\" command.\n\r"
                    + "For more help type:\n\r\n\r"
                    + "dotnet chromelycef.dll download -h"
                    + "\n\r\n\r"
                    + description;

                Console.WriteLine(noCommandInfo);
                return 0;
            });

            app.Command(
                "download",
                (command) =>
                {
                    command.Description = description;
                    command.HelpOption("-?|-h|--help");

                    var chromelyVersionArgument = command.Argument(Template.ChromelyVersion, Template.GetDescription(Template.ChromelyVersion));

                    var cefBinaryVersion = command.Option(
                        Template.BinaryVersion,
                        Template.GetDescription(Template.BinaryVersion),
                        CommandOptionType.SingleValue);

                    var platform = command.Option(
                        Template.OS,
                        Template.GetDescription(Template.OS),
                        CommandOptionType.SingleValue);

                    var cpu = command.Option(
                        Template.Cpu,
                        Template.GetDescription(Template.Cpu),
                        CommandOptionType.SingleValue);

                    var destination = command.Option(
                        Template.Destination,
                        Template.GetDescription(Template.Destination),
                        CommandOptionType.SingleValue);

                    command.OnExecute(() =>
                    {
                        var cefArgument = new ChromelyCefArgument();
                        cefArgument.SetChromelyVersion(chromelyVersionArgument);

                        cefArgument.SetOption(Template.BinaryVersion, cefBinaryVersion);
                        cefArgument.SetOption(Template.OS, platform);
                        cefArgument.SetOption(Template.Cpu, cpu);
                        cefArgument.SetOption(Template.Destination, destination);

                        DownloadService downloadService = new DownloadService();
                        if (downloadService.CefBinariesExist(cefArgument))
                        {
                            Console.WriteLine("Cef binaries exist!");
                            return 0;
                        }

                        if (downloadService.CopyCefBinariesIfExist(cefArgument))
                        {
                            Console.WriteLine("Cef binaries copied!");
                            return 0;
                        }

                        Console.WriteLine("Note that depending on your network, this might take up to 90 seconds to complete.\n\rCef binaries download started.");

                        bool result = downloadService.Process(cefArgument);

                        Console.WriteLine(result ? "Cef binaries download completed successfully." : "Cef binaries download completed with error.");

                        stopwatch.Stop();
                        Console.WriteLine(@"Time elapsed: {0}", stopwatch.Elapsed);

                        return 0;
                    });
                });

            return app.Execute(args);
        }

        /// <summary>
        /// The resolve command line utils dll.
        /// </summary>
        private static void ResolveCommandLineUtilsDll()
        {
            AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
            {
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CefDownloader.Microsoft.Extensions.CommandLineUtils.dll"))
                {
                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);
                    return Assembly.Load(assemblyData);
                }
            };
        }
    }
}


