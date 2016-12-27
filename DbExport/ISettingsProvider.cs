using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbExport
{
    public interface ISettingsProvider
    {
        string ConnectionString { get; }
        int MaxNumberOfThreads { get; }
        string SourceTable { get; }
        string DestinationTable { get; }

    }
}
