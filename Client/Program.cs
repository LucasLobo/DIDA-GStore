using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Client.Commands;
using Client.Domain;
using Google.Protobuf.WellKnownTypes;
using Utils;

namespace Client
{

    class Program
    {

        private static void RegisterCommands(CommandDispatcher commandDispatcher, ConnectionManager connectionManager)
        {
            commandDispatcher.Register("read", new ReadCommand(connectionManager));
            commandDispatcher.Register("write", new WriteCommand(connectionManager));
            commandDispatcher.Register("listServer", new ListServerCommand(connectionManager));
            commandDispatcher.Register("listGlobal", new ListGlobalCommand(connectionManager));
            commandDispatcher.Register("wait", new WaitCommand());
        }

        public static ConnectionManager CreateConnectionManager()
        {
            ConnectionManager connectionManager = new ConnectionManager();
            connectionManager.PrintPartitions();
            Console.WriteLine();
            return connectionManager;
        }

        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true);

            if (args.Length == 0)
            {
                Console.WriteLine("ERROR: Expected a script name but received none.");
                return;
            }

            else if (args.Length > 1)
            {
                Console.WriteLine("WARNING: Expected 1 argument but received " + (args.Length - 1) + ".");
                Console.WriteLine();
            }

            String filename = args[0];

            string[] lines;
            try
            {
                lines = System.IO.File.ReadAllLines(filename);
            }
            catch (System.IO.FileNotFoundException e)
            {
                Console.WriteLine("ERROR: File " + filename + " not found in current directory.");
                Console.WriteLine(e);
                return;
            }

            CommandDispatcher commandDispatcher = new CommandDispatcher();
            ConnectionManager connectionManager = CreateConnectionManager();
            RegisterCommands(commandDispatcher, connectionManager);

            try
            {
                List<string> preprocessed = CommandPreprocessor.Preprocess(lines);

                Task dispatcher = commandDispatcher.ExecuteAllAsync(preprocessed.ToArray());

                for (int i = 0; i < 15; i++)
                {
                    Console.WriteLine("---");
                    await Task.Delay(500);
                }

                await dispatcher;
            }
            catch (PreprocessingException e)
            {
                Console.WriteLine(e.Message);
                return;
            }
        }
    }
}
