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
            var totalCharge = allDevices.Select(d => ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Charge).Sum();
            
            var averageVoltage = totalVoltage / allDevices.Count;
            var averageCharge = totalCharge / allDevices.Count;



            allDevices.ForEach(d =>
            {

                var currentDeviceBattery = ((BatteryPack)d.Parts.GetPowerSupply());
                var currentVoltage = currentDeviceBattery.State.Now.Voltage;
                var currentCharge = currentDeviceBattery.State.Now.Charge;

                var newSumVoltage = totalVoltage - currentVoltage;

                var newAverageVoltage = newSumVoltage / (allDevices.Count - 1);

                var cuttOfvoltage = currentDeviceBattery.CutoffVoltage;

                var checkVoltage = (newAverageVoltage - cuttOfvoltage)*0.1;

                var finalCheckVoltage = newAverageVoltage - checkVoltage;



                if (currentVoltage < finalCheckVoltage)
                //if (currentCharge < newCheckCharge)
                {

                  
                    if (_countermeasuresArgs.applyCountermeasure)
                    {
                        d.Controls.Off();
                    }
                    //if (_countermeasuresArgs.generateAlert)
                    //{
                    //    Log.Info($"Alert!!! Attack detected::'{_countermeasuresArgs.responsibleStakeholder}':Please Check!!!'");
                    //}
                
                    Log.Info($"Device turned off countermeasureApplied");

                }
            });
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
