﻿using D3vS1m.Application.Energy;
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
            var normalNodeName = _args.normalNodeName;
            //var iterationNumberForVictimNodeBatteryDepletion;

            var victimNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == victimNodeName);
            //var normalNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == "Tag_0x11");

            if (victimNode != null)
            {
                //Log.Info($"Victim Node Found");
                if (victimNode.Parts.HasPowerSupply)
                {
                    var victimNodePowerSupply = victimNode.Parts.GetPowerSupply();
                    var battery = victimNodePowerSupply as BatteryPack;

                    if (!battery.State.IsDepleted)
                    {
                        var currentCharge = battery.State.Now.Charge;
                        var remainingCharge = battery.State.Initial.Charge - currentCharge;
                        var remainingVoltageVictimNode = battery.State.Initial.Voltage - battery.State.Now.Voltage;
                        //string createText = _args.Counter+1 + ".  Victim Node Remaining Charge : " + remainingCharge + "  Victim Node Charge Consumption : " + currentCharge + Environment.NewLine;

                        //discharge  the battery manually with some number or by percentage
                        //Use the discharge function to dicharge the battery of the victimNode by provideing time and discharge amount
                        //instance of battery pack simulator
                        var batteryPackSimulator = new BatteryPackSimulator();
                        var sleepTime = _args.Counter % 2;
                        if(sleepTime == 0)
                        {
                            batteryPackSimulator.Discharge(battery, 500, new TimeSpan(0, 0, 0, 10, 0));
                        }
                        //batteryPackSimulator.Discharge(battery, 500, new TimeSpan(0, 0, 0, 10, 0));

                        
                        //Log.Info($"Victim Node Charge Consumption'{currentCharge}'.Victim Node Remaining Charge'{remainingCharge}'.");
                        Log.Info($"Victim Node Initial Volatage'{battery.State.Initial.Voltage}'.  Victim Node Remaining Voltage '{remainingVoltageVictimNode}'.");
                        //Log.Info($"Self Discharge of victim Node::'{battery.State.Initial.SelfDischarge}'");
                        //Log.Info($"Self Discharge of victim Node::'{battery.State.Now.SelfDischarge}'");

                        //Log.Info($"SDR Initial victim Node::'{battery.State.Initial.SDR}'");
                        //Log.Info($"SDR Current victim Node::'{battery.State.Now.SDR}'");

                        var first = battery.State.Now.ElapsedTime.TotalSeconds;
                        var second = currentCharge.ToString();
                        var third = remainingCharge.ToString();
                        var fourth = _args.Counter;
                        var fifth = battery.State.Now.Voltage;
                        //Suggestion made by KyleMit
                        var newLine = string.Format("{0},{1}", fourth, fifth);
                        victimNodeCsv.AppendLine(newLine);
                    }
                   
                }
                //hasPowerSupply
                //victimNode.Parts
            }
            var normalNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == normalNodeName);
            //var normalNode = _netargs.Network.Items.FirstOrDefault(o => o.Name == "Tag_0x11");

            if (normalNode != null)
            {
                //Log.Info($"Victim Node Found");
                if (normalNode.Parts.HasPowerSupply)
                {
                    var normalNodePowerSupply = normalNode.Parts.GetPowerSupply();
                    var normalNodebattery = normalNodePowerSupply as BatteryPack;

                    if (!normalNodebattery.State.IsDepleted)
                    {
                        var currentChargenormalNode = normalNodebattery.State.Now.Charge;
                        var remainingChargeNormalNode = normalNodebattery.State.Initial.Charge - currentChargenormalNode;
                        var remainingVoltageNormalNode = normalNodebattery.State.Initial.Voltage - normalNodebattery.State.Now.Voltage;



                        //Log.Info($"Normal Node Charge Consumption'{currentChargenormalNode}'.  Normal Node Remaining Charge'{remainingChargeNormalNode}'.");
                        Log.Info($"Normal Node Initial Volatage'{normalNodebattery.State.Initial.Voltage}'.  Normal Node Remaining Voltage '{remainingVoltageNormalNode}'.");

                        //Log.Info($"Self Discharge of victim Node::'{normalNodebattery.State.Initial.SelfDischarge}'");
                        //Log.Info($"Self Discharge of victim Node::'{normalNodebattery.State.Now.SelfDischarge}'");
                        //Log.Info($"SDR Initial victim Node::'{normalNodebattery.State.Initial.SDR}'");
                        //Log.Info($"SDR Current victim Node::'{normalNodebattery.State.Now.SDR}'");

                        //Log.Info($"Self Discharge of victim Node::'{normalNodebattery.Polynom.}'");

                        //var first = currentChargenormalNode.ToString();
                        //var second = remainingChargeNormalNode.ToString();

                        var first = normalNodebattery.State.Now.ElapsedTime.TotalSeconds;
                        var second = currentChargenormalNode.ToString();
                        var third = remainingChargeNormalNode.ToString();
                        var fourth = _args.Counter;
                        var fifth = normalNodebattery.State.Now.Voltage;
                        //Suggestion made by KyleMit
                        var newLine = string.Format("{0},{1}", fourth, fifth);
                        normalNodeCsv.AppendLine(newLine);
                    }
                   
                }
            }
                _args.Counter++;
            File.WriteAllText(_args.victimNoderesultFilePath, victimNodeCsv.ToString());
            File.WriteAllText(_args.normalNoderesultFilePath, normalNodeCsv.ToString());
        }

        public override string Name => AttackWpanModule.AttackInWpan.Name;
        public override SimulationTypes Type => SimulationTypes.Custom;
        public override ArgumentsBase Arguments => _args;

        public override string Key => AttackWpanModule.AttackInWpan.Name;

    }
}
