using D3vS1m.Application.Energy;
using D3vS1m.Application.Network;
using D3vS1m.Domain.Data.Arguments;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Enumerations;
using Sin.Net.Domain.Persistence.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace D3vS1m.Application.AttackWpan
{
    public class AttackWpanSimpleSimulator : SimulatorBase
    {
        // -- fields

        private AttackWpanArgs _args;
        NetworkArgs _netargs;
        // -- constructor
        public AttackWpanSimpleSimulator() : this(null)
        {

        }
        public AttackWpanSimpleSimulator(RuntimeBase runtime) : base(runtime)
        {

        }
        // -- methods

        public override ISimulatable With(ArgumentsBase arguments)
        {
            
            if (ConvertArgs(arguments, ref _args)) return this;
            if (ConvertArgs(arguments, ref _netargs)) return this;
            else return ArgsNotAdded(arguments.Name);

        }

        public override void Run()
        {
            base.BeforeExecution();

            //check netarguments
            if (_netargs == null)
            {
                //throwing exception
                throw new System.ArgumentException("Network Arguments cannot be null", "_netArgs");
            }
            //my logic to deplete the battery
            //learn linq , code base about parent project d3ms1m, check properties throughly, 
            //extending the argument to accomodATE the attack name, 
            //'attackedDevice'
            var victimNodeName = _args.victimNodeName;

            var victimNode =  _netargs.Network.Items.FirstOrDefault(o => o.Name == victimNodeName);
            if (victimNode != null)
            {
                Log.Info($"Victim Node Found");
                if (victimNode.Parts.HasPowerSupply)
                {
                    var victimNodePowerSupply = victimNode.Parts.GetPowerSupply();
                    var battery = victimNodePowerSupply as BatteryPack;
                    var currentCharge = battery.State.Now.Charge;
                    var remainingCharge = battery.State.Initial.Charge - currentCharge;
                    string createText = _args.Counter +".  Victim Node Remaining Charge : " + remainingCharge+ "  Victim Node Charge Consumption : " + currentCharge + Environment.NewLine;
                    File.AppendAllText(_args.resultFilePath, createText);
                    Log.Info($"Victim Node Charge Consumption'{currentCharge}'.");
                    Log.Info($"Victim Node Remaining Charge'{remainingCharge}'.");
                }
                //hasPowerSupply
                //victimNode.Parts
            }
                _args.Counter++;

            base.AfterExecution();
        }

        // -- properties


        public override string Name => AttackWpanModule.AttackInWpan.Name;
        public override SimulationTypes Type => SimulationTypes.Custom;
        public override ArgumentsBase Arguments => _args;

        public override string Key => AttackWpanModule.AttackInWpan.Name;

    }
}
