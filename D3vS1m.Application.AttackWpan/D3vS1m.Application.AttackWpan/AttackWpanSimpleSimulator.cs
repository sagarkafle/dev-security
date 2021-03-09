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

         private StringBuilder victimNodeCsv  = new StringBuilder();
         private StringBuilder normalNodeCsv  = new StringBuilder();
         private StringBuilder CurrentStateVoltageCOnsumptionCsv  = new StringBuilder();
         private StringBuilder CurrentStateChargeCOnsumptionCsv  = new StringBuilder();

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
            var allDevices = _netargs.Network.Items;

            var victimNodeName = _args.victimNodeName;
            var normalNodeName = _args.normalNodeName;
            //var iterationNumberForVictimNodeBatteryDepletion;

            var victimNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == victimNodeName);

            var networkAvailability = _netargs.Network.Availability();
            float victimNodeVoltageNowCsv = 0;
            float normalNodeVoltageNowCsv = 0;

            float victimNodeCurrentCharge = 0;
            float normalNodeCurrentCharge = 0;

            float victimNodeRemainingChargeCsv = 0;
            float normalNodeRemainingChargeCsv = 0;

            var normalNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == normalNodeName);

            //Normalnode 
            if (normalNode != null)
            {
                if (normalNode.Parts.HasPowerSupply)
                {
                    var normalNodePowerSupply = normalNode.Parts.GetPowerSupply();
                    var normalNodebattery = normalNodePowerSupply as BatteryPack;

                    if (!normalNodebattery.State.IsDepleted)
                    {
                        var currentChargenormalNode = normalNodebattery.State.Now.Charge;
                        var remainingChargeNormalNode = normalNodebattery.State.Initial.Charge - currentChargenormalNode;
                        var remainingVoltageNormalNode = normalNodebattery.State.Initial.Voltage - normalNodebattery.State.Now.Voltage;


                        _args.dichargeAmountNormal = (float)normalNodebattery.State.Now.Voltage;


                        Log.Info($"Normal Node Initial Volatage'{normalNodebattery.State.Initial.Voltage}'.  Normal Node Remaining Voltage '{normalNodebattery.State.Now.Voltage}'.");

                        
                        var first = normalNodebattery.State.Now.ElapsedTime.TotalSeconds;
                        normalNodeCurrentCharge = normalNodebattery.State.Now.Charge;
                        normalNodeRemainingChargeCsv = remainingChargeNormalNode;
                        var fourth = _args.Counter;
                        var fifth = normalNodebattery.State.Now.Voltage;
                        normalNodeVoltageNowCsv = normalNodebattery.State.Now.Voltage;

                        var newLine = string.Format("{0},{1}", fourth, fifth);
                        normalNodeCsv.AppendLine(newLine);
                    }

                }
            }

            if (victimNode != null)
            {
                if (victimNode.Parts.HasPowerSupply)
                {
                    var victimNodeControls = victimNode.Controls;

                    var victimNodePowerSupply = victimNode.Parts.GetPowerSupply();
                    var battery = victimNodePowerSupply as BatteryPack;

                    if (!battery.State.IsDepleted)
                    {
                        var currentCharge = battery.State.Now.Charge;
                        var remainingCharge = battery.State.Initial.Charge - currentCharge;
                        var remainingVoltageVictimNode = battery.State.Initial.Voltage - battery.State.Now.Voltage;
                       
                        
                        //instance of battery pack simulator


                        var batteryPackSimulator = new BatteryPackSimulator();

                        //Sleep time description and attack during sleep time 
                        var sleepTimeArgument = _args.Counter % 500;
                        if (sleepTimeArgument == 0){
                         

                            _args.sleepCounter ++;
                            Log.Info($"Argument COunter'{_args.Counter}'.  SleepCounter '{_args.sleepCounter}'.");
                        }
                        var sleepTime = _args.sleepCounter % 4;
                        if (victimNode.IsActive)
                        {
                            Log.Info($"Victim Node Active");

                        }
                        else
                        {
                            Log.Info($"Victim Node Not Active");
                        }
                        if(sleepTime == 0  && victimNode.IsActive)
                        {
                            Log.Info($"Attack Initiated.Check2");
                            //Use the discharge function to dicharge the battery of the victimNode by provideing time and discharge amount
                            batteryPackSimulator.Discharge(battery, 1000, new TimeSpan(0, 0, 0, 10, 0));
                        }
                        if (!victimNode.IsActive)
                        {
                            Log.Info("Battery Exhausted");
                        }
                        

                        


                        Log.Info($"Victim Node Initial Volatage'{battery.State.Initial.Voltage}'.  Victim Node Remaining Voltage '{battery.State.Now.Voltage}'.");
                       

                        var first = battery.State.Now.ElapsedTime.TotalSeconds;
                         victimNodeCurrentCharge = battery.State.Now.Charge;
                        victimNodeRemainingChargeCsv = remainingCharge;
                        var fourth = _args.Counter;
                        var fifth = battery.State.Now.Voltage;
                         victimNodeVoltageNowCsv = battery.State.Now.Voltage;
                        //Suggestion made by KyleMit
                        var newLine = string.Format("{0},{1}", fourth, fifth);
                       
                        victimNodeCsv.AppendLine(newLine);
                    }
                   
                }
                
            }

            //average remaining voltage and charge
            var totalVoltage = allDevices.Select(d => ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Voltage).Sum();
            var totalCharge = allDevices.Select(d => ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Charge).Sum();

            var averageVoltage = totalVoltage / allDevices.Count;
            var averageCharge = totalCharge / allDevices.Count;
            Log.Info("Voltage Consumption Average:" + averageVoltage);
            Log.Info("Charge Consumption Average:" + averageCharge);


            //Current node Volt consumption append into csv 
            //Current node Volt consumption append into csv 
            string addNewLine = string.Format("{0},{1},{2},{3}", _args.Counter, normalNodeVoltageNowCsv, victimNodeVoltageNowCsv,averageVoltage);
            CurrentStateVoltageCOnsumptionCsv.AppendLine(addNewLine);
            //Current node Charge consumption append into csv 
            string addNewLineCharge = string.Format("{0},{1},{2},{3}", _args.Counter, normalNodeRemainingChargeCsv, victimNodeRemainingChargeCsv,2700-averageCharge);
            CurrentStateChargeCOnsumptionCsv.AppendLine(addNewLineCharge);
            //csv.AppendFormat(...)
            _args.Counter++;




            //File.WriteAllText(_args.CurrentStateVoltageCOnsumptionCsvFilePath, CurrentStateVoltageCOnsumptionCsv.ToString());

            //File.WriteAllText(_args.CurrentStateChargeCOnsumptionCsvFilePath, CurrentStateChargeCOnsumptionCsv.ToString());

        }

        public override string Name => AttackWpanModule.AttackInWpan.Name;
        public override SimulationTypes Type => SimulationTypes.Custom;
        public override ArgumentsBase Arguments => _args;

        public override string Key => AttackWpanModule.AttackInWpan.Name;

    }
}
