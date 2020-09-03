using D3vS1m.Application.Energy;
using D3vS1m.Application.Network;
using D3vS1m.Domain.Data.Arguments;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Enumerations;
using Sin.Net.Domain.Persistence.Logging;
using Sin.Net.Domain.System.UserManagement;
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
        private NetworkArgs _netargs;
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

           if(_args.attackName== "BatteryExhaustionAttack")
            {
                BatteryExhaustionAttack();
            }

            base.AfterExecution();
        }

        // -- properties
        private void BatteryExhaustionAttack()
        {
            //check netarguments
            if (_netargs == null)
            {
                //throwing exception
                throw new System.ArgumentException("Network Arguments cannot be null", "_netArgs");
            }
 
            var victimNodeName = _args.victimNodeName;

            var victimNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == victimNodeName);
            //var normalNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == "Tag_0x11");

            if (victimNode != null)
            {
                Log.Info($"Victim Node Found");
                if (victimNode.Parts.HasPowerSupply)
                {
                    var victimNodePowerSupply = victimNode.Parts.GetPowerSupply();
                    var battery = victimNodePowerSupply as BatteryPack;
                    var currentCharge = battery.State.Now.Charge;
                    var remainingCharge = battery.State.Initial.Charge - currentCharge;
                    string createText = _args.Counter+1 + ".  Victim Node Remaining Charge : " + remainingCharge + "  Victim Node Charge Consumption : " + currentCharge + Environment.NewLine;

                    //discharge  the battery manually with some number or by percentage
                    //Use the discharge function to dicharge the battery of the victimNode by provideing time and discharge amount
                    //instance of battery pack simulator
                    //var batteryPackSimulator = new BatteryPackSimulator();
                    //batteryPackSimulator.Discharge(),
                    //batterPackSimulator.With()
                    
                    //if (currentCharge > battery.State.Initial.Charge)
                    //{
                    //    Log.Info($"Battery Charge is finished. ");
                    //}
                    File.AppendAllText(_args.resultFilePath, createText);
                    Log.Info($"Victim Node Charge Consumption'{currentCharge}'.");
                    Log.Info($"Victim Node Remaining Charge'{remainingCharge}'.");
                }
                //hasPowerSupply
                //victimNode.Parts
            }
            _args.Counter++;
        }

        public override string Name => AttackWpanModule.AttackInWpan.Name;
        public override SimulationTypes Type => SimulationTypes.Custom;
        public override ArgumentsBase Arguments => _args;

        public override string Key => AttackWpanModule.AttackInWpan.Name;

    }
}
