using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SqlDataScripter
{
    class Options
    {
        [Option('s', "server", HelpText = "SQL Server instance.", Default = ".")]
        public string Server { get; set; }

        [Option('d', "database", Required = true, HelpText = "Script database name.")]
        public string Database { get; set; }

        [Option('u', "user", HelpText = "SQL user name.")]
        public string UserName { get; set; }

        [Option('p', "password", HelpText = "SQL user password.")]
        public string Password { get; set; }

        [Option('o', "output", HelpText = "Output file name.")]
        public string OutputFileName { get; set; }

        [Option('t', "table", HelpText = "Table name(s) to script. Script all tables in database if empty.")]
        public IEnumerable<string> Tables { get; set; }

        [Option("schema", HelpText = "Script table schema.")]
        public bool ScriptSchema { get; set; }

        [Option("data", HelpText = "Script table data.")]
        public bool ScriptData { get; set; }

        [Option(HelpText = "Output file encoding.")]
        public string Encoding { get; set; }

        [Option(HelpText = "Append to file not overwrite.")]
        public bool AppendToFile { get; set; }

        [Option(HelpText = "Disables GO terminator in script.")]
        public bool NoBatchTerminator { get; set; }

        [Option(HelpText = "Add an existence check to script.")]
        public bool IncludeIfNotExists { get; set; }

        [Option(HelpText = "Include a header containing information about the object being scripted.")]
        public bool IncludeHeaders { get; set; }

        [Option(HelpText = "Script table indexes.")]
        public bool Indexes { get; set; }

        [Option('q', "quiet", HelpText = "Suppress any informational messages.")]
        public bool Quiet { get; set; }

    }
}
