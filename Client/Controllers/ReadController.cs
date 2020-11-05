using Client.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace Client.Controllers
{
    class ReadController
    {
        public static async Task<GStoreObject> Execute(ConnectionManager connectionManager, string partitionId, string serverId, string objectId)
        {
            Dictionary<string, Server> nonUsedReplicas = new Dictionary<string, Server>();
            try
            {
                Server server = connectionManager.ChooseServerForRead(partitionId, serverId, out Dictionary<string, Server> aux);
                nonUsedReplicas = aux;
                Console.WriteLine($"Trying: {server.Id}");
                return await SendReadRequest(partitionId, objectId, server);
            }
            catch (Grpc.Core.RpcException e) when (e.StatusCode == Grpc.Core.StatusCode.Internal)
            {
                connectionManager.DeclareDead(serverId);
                nonUsedReplicas.Remove(serverId);
                foreach (KeyValuePair<string, Server> replica in nonUsedReplicas)
                {
                    try
                    {
                        return await SendReadRequest(partitionId, objectId, replica.Value);
                    }
                    catch (Grpc.Core.RpcException exception) when (exception.StatusCode == Grpc.Core.StatusCode.Internal)
                    {
                        connectionManager.DeclareDead(replica.Key);
                        nonUsedReplicas.Remove(replica.Key);
                        continue;
                    }
                }

                return null;
            }
        }

        private static async Task<GStoreObject> SendReadRequest(string partitionId, string objectId, Server server)
        {
            GStoreReadRequest gStoreReadRequest = new GStoreReadRequest()
            {
                ObjectIdentifier = new DataObjectIdentifier
                {
                    PartitionId = partitionId,
                    ObjectId = objectId
                }
            };

            GStoreReadReply gStoreReadReply = await server.Stub.ReadAsync(gStoreReadRequest);
            return CreateObject(gStoreReadReply.Object);
        }

        private static GStoreObject CreateObject(DataObject gStoreObject)
        {
            GStoreObjectIdentifier gStoreObjectIdentifier = new GStoreObjectIdentifier(gStoreObject.ObjectIdentifier.PartitionId, gStoreObject.ObjectIdentifier.ObjectId);
            return new GStoreObject(gStoreObjectIdentifier, gStoreObject.Value);
        }
    }
}
