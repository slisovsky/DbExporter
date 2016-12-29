//#define USECOMMANDLINEARGS

using System;
using System.Data.SqlClient;
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
            catch (SqlException sqlException)
            {
                foreach (SqlError error in sqlException.Errors)
                {
                    logger.Error("Error{0} [Message: {1}]", error.Number, error.Message);
                }
            }
            catch (AggregateException exception)
            {
                foreach(var innerEx in exception.InnerExceptions)
                    logger.Error(innerEx.Message);
            }
            catch (Exception exception)
            {
                logger.Error(exception.Message);
            }
            finally
            {
                logger.Trace("Закончили");
            }
        }

    }
}
