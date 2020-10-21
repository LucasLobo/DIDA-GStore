using Grpc.Core;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PuppetMaster.Commands
{
    class UnfreezeServerCommand : Command
    {
        private TextBox txtBoxOutput;
        private PuppetMasterServerServices.PuppetMasterServerServicesClient serverStub;
        public UnfreezeServerCommand(TextBox output, PuppetMasterServerServices.PuppetMasterServerServicesClient serverStub) : base(true)
        {
            this.txtBoxOutput = output;
            this.serverStub = serverStub;
    }

        public static int EXPECTED_ARGUMENTS = 1;
        public override async Task ExecuteAsync(List<string> arguments)
        {
            if (arguments.Count != EXPECTED_ARGUMENTS)
            {
                this.txtBoxOutput.AppendText(Environment.NewLine + "Expected " + EXPECTED_ARGUMENTS + " arguments but found " + arguments.Count + ".");
                return;
            }

            try
            {
                await serverStub.UnfreezeAsync(new Google.Protobuf.WellKnownTypes.Empty());
                txtBoxOutput.AppendText(Environment.NewLine + "Unfreeze DONE.");
            }
            catch (RpcException e)
            {
                txtBoxOutput.AppendText(Environment.NewLine + e.Message);
            }


        }
    }
}
