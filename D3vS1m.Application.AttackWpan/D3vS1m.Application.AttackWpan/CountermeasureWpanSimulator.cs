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

                var newCheckVoltage = newAverageVoltage * 0.9;
                //var newCheckVoltage = newAverageVoltage * _countermeasuresArgs.detectPercent;

                //avergae voltage subtract from average voltage
                var cuttOfvoltage = ((BatteryPack)d.Parts.GetPowerSupply()).CutoffVoltage;

                var newCheckVoltage1 = (newAverageVoltage - cuttOfvoltage)*0.1;

                var checkVoltage2 = newAverageVoltage - newCheckVoltage1;


                //var newCheckCharge = newAverageCharge * 0.9;

                if (currentVoltage < checkVoltage2)
                //if (currentCharge < newCheckCharge)
                {

                    //Log.Info($"Current Voltage'{currentVoltage}':New Check Voltage'{newCheckVoltage}'");

                    //Log.Info($"averageVoltage '{averageVoltage}':newAverageVoltage'{newAverageVoltage}'");
                    //Log.Info($"totalVoltage '{totalVoltage}':New newSumVoltage '{newSumVoltage}'");
                    //Log.Info($"Current Voltage'{currentVoltage}':New Check Voltage'{newCheckVoltage}'");  

                    //Log.Info($"averageCharge '{averageCharge}':newAverageCharge'{newAverageCharge}'");
                    //Log.Info($"totalCharge '{totalCharge}':New newSumCharge '{newSumCharge}'");
                    //Log.Info($"Current Charge'{currentCharge}':New Check CHarge'{newCheckCharge}'");

                    //if (_countermeasuresArgs.applyCountermeasure)
                    //{
                    //    d.Controls.Off();
                    //}
                    //if (_countermeasuresArgs.generateAlert)
                    //{
                    //    Log.Info($"Alert!!! Attack detected::'{_countermeasuresArgs.responsibleStakeholder}':Please Check!!!'");
                    //}
                    //Check the flag for on and off of device
                    d.Controls.Off();
                    Log.Info($"Device turned off countermeasureApplied");

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
