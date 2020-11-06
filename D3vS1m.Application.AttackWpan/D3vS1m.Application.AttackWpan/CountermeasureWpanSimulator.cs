using D3vS1m.Application.Energy;
using D3vS1m.Application.Network;
using D3vS1m.Domain.Data.Arguments;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Enumerations;
using Sin.Net.Domain.Persistence.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace D3vS1m.Application.AttackWpan
{
    public class CountermeasureWpanSimulator : SimulatorBase
    {
        public override ArgumentsBase Arguments => _countermeasuresArgs;

        private CountermeasureWpanArgs _countermeasuresArgs;
        private NetworkArgs _netArgs;




        public override string Name => CountermeasureWpanModel.CountermeasureInWpan.Name;

        public override string Key => CountermeasureWpanModel.CountermeasureInWpan.Key;

        public override SimulationTypes Type { get; }

        public override void Run()
        {

            base.BeforeExecution();
            var allDevices = _netArgs.Network.Items;
            var totalVoltage = allDevices.Select(d => ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Voltage).Sum();
            var averageVoltage = totalVoltage / allDevices.Count;

            var checkVoltage = averageVoltage * 0.9;

            allDevices.ForEach(d =>
            {
                
            var currentVoltage = ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Voltage;
                var newSumVoltage = totalVoltage - currentVoltage;
                var newAverageVoltage = newSumVoltage / (allDevices.Count - 1);
                var newCheckVoltage = newAverageVoltage * 0.8;
                if (currentVoltage< newCheckVoltage)
                {
                    Log.Info("AlertCheck the attack is made");

                    //Check the flag for on and off of device
                }
            });
            //Log.Info($"Average Voltage'{averageVoltage}'");
            base.AfterExecution();
        }
        public CountermeasureWpanSimulator() : this(null)
        {

        }
        public CountermeasureWpanSimulator(RuntimeBase runtime) : base(runtime)
        {

        }


        public override ISimulatable With(ArgumentsBase arguments)
        {
            if (ConvertArgs(arguments, ref _countermeasuresArgs)) return this;
            if (ConvertArgs(arguments, ref _netArgs)) return this;
            else return ArgsNotAdded(arguments.Name);
            //throw new NotImplementedException();
        }
    }
}
