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

            var scripter = new Scripter(server);
            scripter.Options.IncludeIfNotExists = options.IncludeIfNotExists;
            scripter.Options.IncludeHeaders = options.IncludeHeaders;
            scripter.Options.ScriptSchema = options.ScriptSchema;
            scripter.Options.ScriptData = options.ScriptData;
            //scripter.Options.ScriptBatchTerminator = true;
            //scripter.Options.NoCommandTerminator = false;
            //scripter.Options.FileName = "test.sql"; 
            //scripter.Options.ToFileOnly = true;
            if (String.IsNullOrEmpty(options.OutputFileName))
            {
                ScriptTables(Console.Out, scripter, GetTables(database, options.Tables), quiet);
            }
            else
            {
                using (StreamWriter file = new StreamWriter(options.OutputFileName))
                {
                    ScriptTables(file, scripter, GetTables(database, options.Tables), quiet);
                }
            }

            if (!quiet)
                Console.WriteLine("Scripting done.");

            return 0;
        }

        private static void ScriptTables(TextWriter output, Scripter scripter, IEnumerable<Table> tables, bool quiet)
        {
            foreach (var table in tables)
            {
                if (table != null && !table.IsSystemObject)
                {
                    if (!quiet)
                        Console.WriteLine("Scripting table {0}", table.Name);
                    foreach (string s in scripter.EnumScript(new Urn[] { table.Urn }))
                    {
                        output.WriteLine(s);
                    }
                }
            }
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
