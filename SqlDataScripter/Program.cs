using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;
using CommandLine.Text;
using Microsoft.SqlServer.Management.Common;
using Microsoft.SqlServer.Management.Sdk.Sfc;
using Microsoft.SqlServer.Management.Smo;

namespace SqlDataScripter
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                return Parser.Default.ParseArguments<Options>(args)
                       .MapResult(o => ScriptDatabase(o),
                       _ => 1); ;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(CommandLine.Text.SentenceBuilder.Factory.Invoke().ErrorsHeadingText.Invoke());
                Console.Error.WriteLine(ex.Message);
                return 1;
            }
        }

        static int ScriptDatabase(Options options)
        {
            ServerConnection connection = String.IsNullOrEmpty(options.UserName) ?
                new ServerConnection(options.Server) :
                new ServerConnection(options.Server, options.UserName, options.Password);
            Server server = new Server(connection);

            bool quiet = options.Quiet || String.IsNullOrEmpty(options.OutputFileName);

            if (!quiet)
            {
                Console.WriteLine(HeadingInfo.Default.ToString());
                Console.WriteLine(CopyrightInfo.Default.ToString());
                Console.WriteLine("SQL Server '{0}' version {1}", server.Name, server.Version);
            }

            Database database = server.Databases[options.Database];
            if (database == null)
                throw new Exception(String.Format("Cannot find database {0}.", options.Database));

            var scripter = new TableScripter(server, quiet ? null : Console.Out);
            scripter.Options.IncludeIfNotExists = options.IncludeIfNotExists;
            scripter.Options.IncludeHeaders = options.IncludeHeaders;
            scripter.Options.ScriptSchema = options.ScriptSchema;
            scripter.Options.ScriptData = options.ScriptData;
            scripter.Options.ScriptBatchTerminator = !options.NoBatchTerminator;
            scripter.Options.NoCommandTerminator = options.NoBatchTerminator;
            if (!String.IsNullOrEmpty(options.OutputFileName))
            {
                scripter.Options.FileName = options.OutputFileName;
                scripter.Options.ToFileOnly = true;
                scripter.Options.AppendToFile = options.AppendToFile;
            }
            scripter.Options.Indexes = options.Indexes;
            if (!String.IsNullOrEmpty(options.Encoding))
            {
                scripter.Options.Encoding = Encoding.GetEncoding(options.Encoding);                                
            }

            var tables = GetTables(database, options.Tables).ToArray();
            foreach (string s in scripter.ScriptTables(tables))
            {
                Console.WriteLine(s);
            }

            if (!quiet)
                Console.WriteLine("Scripting done.");

            return 0;
        }

        static IEnumerable<Table> GetTables(Database database, IEnumerable<string> names)
        {
            if (names == null || !names.Any())
            {
                foreach (var table in database.Tables)
                {
                    yield return (Table)table;
                }
            }
            else
            {
                foreach (var name in names)
                {
                    if (!database.Tables.Contains(name))
                        throw new ArgumentOutOfRangeException("table");
                    yield return database.Tables[name];
                }
            }
        }
    }
}
