//#define USECOMMANDLINEARGS

using System;
using DbExport.Interfaces;
using DbExport.Settings;
using Microsoft.Practices.Unity;
using NLog;

namespace DbExport
{
    class Program
    {
        static void Main(string[] args)
        {
            ILogger logger = LogManager.GetLogger("mainlogger");
            var container = new UnityContainer();
            try
            {
                container.RegisterInstance(logger)
#if USECOMMANDLINEARGS
                .RegisterType<ISettingsProvider, CommandLineSettingsProvider>(
                    new InjectionConstructor(args.ToList()))
#else
                .RegisterType<ISettingsProvider, AppConfigSettingsProvider>()
#endif
                .RegisterType<IDbProvider, DbProvider>();

                var exporter = container.Resolve<DbExporter>();

                logger.Trace("Начали");
                exporter.Export();
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
