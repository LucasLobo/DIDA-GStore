using System;
using System.Collections.Generic;

namespace Utils
{
    public class InitializationParser
    {
        // Examples:
        // Server server_id URL min_delay max_delay -l n [server_id,URL]{n} -p r partition_name [server_id]{r} -p partition_name r [server_id]{r}
        // Client username client_URL script_file -l n [server_id,URL]{n} -p r partition_name [server_id]{r} -p partition_name r [server_id]{r}
        //
        // It will only be passed the network configuration to this class (input after ' -l ')

        private readonly string[] InputLineSplited; // input splited by " -p "
        public InitializationParser(string toParse)
        {
            InputLineSplited = toParse.Split(" -p ");
        }

        public List<Tuple<string, string>> getServersConfiguration()
        {
            List<Tuple<string, string>> res = new List<Tuple<string, string>>();
            string[] serversArray = InputLineSplited[0].Split(" ");
        
            for (int i = 0; i < Int32.Parse(serversArray[2]); i++)
            {
                string[] serverAndUrl = serversArray[i+3].Split(",");
                res.Add(new Tuple<string, string>(serverAndUrl[0], serverAndUrl[1]));
                Console.WriteLine($"server: {serverAndUrl[0]} url{serverAndUrl[1]}");
            }
            return res;
        }

        public List<Tuple<string, List<string>>> getPartitionsConfiguration()
        {
            List<Tuple<string, List<string>>> res = new List<Tuple<string, List<string>>>();
            for (int i = 1; i < InputLineSplited.Length; i++)
            {
                string[] partitionInput = InputLineSplited[i].Split(" ");
                List<string> serverIDs = new List<string>();
                Console.WriteLine("PARTITION: " + partitionInput[1]);
                for (int j = 0; j < Int32.Parse(partitionInput[0]); j++)
                {
                    serverIDs.Add(partitionInput[j+2]);
                    Console.WriteLine("Server: " + partitionInput[j+2]);
                }
                res.Add(new Tuple<string, List<string>>(partitionInput[0], serverIDs));
            }
            return res;
        }
    }
}
