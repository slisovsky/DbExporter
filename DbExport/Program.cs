//#define USECOMMANDLINEARGS

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Practices.Unity;
using NLog;
using NLog.Internal;

namespace DbExport
{
    public class ChunkDescription
    {
        public int MinId { get; set; }
        public int MaxId { get; set; }

    }
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = LogManager.GetLogger("mainlogger");
            var container = new UnityContainer();
            try
            {

#if USECOMMANDLINEARGS
                container.RegisterType<ISettingsProvider, CommandLineSettingsProvider>(
                    new InjectionConstructor(args.ToList()));
#else
                container.RegisterType<ISettingsProvider, AppConfigSettingsProvider>();
#endif

                var settings = container.Resolve<ISettingsProvider>();

                logger.Trace("Начали");

                var exporter = new DbExporter(settings.ConnectionString);
                if (!exporter.EnsureTableExists(settings.SourceTable))
                {
                    throw new ArgumentNullException("Source Table");
                }

                if (!exporter.EnsureTableExists(settings.DestinationTable))
                {
                    throw new ArgumentNullException("Destination Table");
                }

                var count = exporter.GetRowCount(settings.SourceTable);
                var rowsPerThread = (int)Math.Ceiling(count *1.0f/settings.MaxNumberOfThreads);
                var chunks = new List<ChunkDescription>();
                for (var i = 1; i <= 10; i++)
                {
                    chunks.Add(new ChunkDescription { MinId = (i - 1) * rowsPerThread + 1, MaxId = i * rowsPerThread });
                }
                //exporter.InsertRecord("dbo.Destination", 2016, "Happy", "New Year!");
                exporter.Export(chunks, settings.MaxNumberOfThreads);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
            }
            finally
            {
                logger.Trace("Закончили");
            }
        }
    }
}
