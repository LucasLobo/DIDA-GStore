using Google.Protobuf.WellKnownTypes;
using PuppetMaster.Domain;
using System.Threading.Tasks;

namespace PuppetMaster.Controllers.ServerDebugControllers
{
    class UnfreezeServerController
    {
        public static async Task<Empty> Execute(ConnectionManager connectionManager, string serverId)
        {
            Domain.Server server = connectionManager.GetServer(serverId);
            return server.Stub.Unfreeze(new Empty());
        }
    }
}