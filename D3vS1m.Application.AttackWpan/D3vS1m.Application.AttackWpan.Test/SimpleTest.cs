using System;
using System.Threading.Tasks;
using D3vS1m.Application.Channel;
using D3vS1m.Application.Communication;
using D3vS1m.Application.Energy;
using D3vS1m.Application.Devices;
using D3vS1m.Application.Network;
using D3vS1m.Application.Runtime;
using D3vS1m.Application.Scene;
using D3vS1m.Application.Validation;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sin.Net.Domain.Persistence.Logging;
using System.Reflection.Metadata.Ecma335;
using System.Linq;
using FluentValidation.Validators;

namespace D3vS1m.Application.AttackWpan.Test
{
	[TestClass]
	public class SimpleTest : TestBase
	{
		private PeerToPeerNetwork _network;
        private BatteryPack _battery;

        [TestInitialize]
		public override void Arrange()
		{
			base.Arrange();
		}

		[TestCleanup]
		public override void Cleanup()
		{
			base.Cleanup();
		}

		//[TestMethod]
		public void RunAttackSimulator()
		{
			// arrange
			var args = new AttackWpanArgs();
			
			var simulator = new AttackWpanSimpleSimulator();

			
			simulator.OnExecuting += (o, e) =>
			{
				var attackArgs = e.Arguments as AttackWpanArgs;
				Log.Info($"Simulation will start now with '{attackArgs.Name}'.");
			};
			simulator.Executed += (o, e) =>
			{
				Log.Info("Simulation done");
			};
			
			// act
			simulator
				.With(args)
				.Run();

			// assert
			//Log.Info($"Counter Value='{ args.Counter}'.");
			//Assert.AreEqual(iternations, passed, $"The runtime should have run '{iternations}' times instead of '{passed}'.");
			Assert.IsTrue(args.Counter > 0, $"The counter should be greater than zero");
		}

        [TestMethod]
        public async Task RunAttackSimulatorWithRuntime()
		{
            // arrange
            //var iternations = 10000;
            //var iternations = 5000;
            var iternations = 9718;
            //var iternations = 100;
            //var iternations = 500;
            var passed = 0;
			var victimNodeName = "victimNode";
			var normalNodeName = "Anchor_1";


			//result filenames 
			var victimNodeResultFilePath = @"C:\Users\nepho\source\repos\dev-security\D3vS1m.Application.AttackWpan\D3vS1m.Application.AttackWpan.Test\output\outputVictimNode.csv";
			var normalNodeResultFilePath = @"C:\Users\nepho\source\repos\dev-security\D3vS1m.Application.AttackWpan\D3vS1m.Application.AttackWpan.Test\output\outputNormalNode.csv";
			var VoltageCOnsumptionResultFilePath = @"C:\Users\nepho\source\repos\dev-security\D3vS1m.Application.AttackWpan\D3vS1m.Application.AttackWpan.Test\output\CurrentStateVOltageCOnsumption.csv";
			var ChargeCOnsumptionResultFilePath = @"C:\Users\nepho\source\repos\dev-security\D3vS1m.Application.AttackWpan\D3vS1m.Application.AttackWpan.Test\output\CurrentStateChargeCOnsumption.csv";

			var runtime = new RuntimeController(new BasicValidator());
			var repo = new SimulatorRepository();
			var attackArgument = new AttackWpanArgs();
			attackArgument.victimNoderesultFilePath = victimNodeResultFilePath;
			attackArgument.normalNoderesultFilePath = normalNodeResultFilePath;
			attackArgument.CurrentStateVoltageCOnsumptionCsvFilePath = VoltageCOnsumptionResultFilePath;
			attackArgument.CurrentStateChargeCOnsumptionCsvFilePath = ChargeCOnsumptionResultFilePath;
			attackArgument.attackName = "BatteryExhaustionAttack";

			attackArgument.dichargeAmountNormal = 0;
			attackArgument.sleepCounter = 0;
			//var simulator = repo.Add(new AttackWpanSimpleSimulator()
			//	.With(new AttackWpanArgs()));

			var countermeasuresArgument = new CountermeasureWpanArgs();
			


			/*
			 * TODO: add more simulators and run the simualtion
			 * - You need: Network, Devices, Energy, Radio Channel
			 * - Add all arguments you need to your simulator with the With() method
			 * - Create a break condition to end the simulation runtime
			 */

			//Initialization of network simulator

			var networkSimulator = new PeerToPeerNetworkSimulator(runtime);
			var netArgs = new NetworkArgs();

            //Added necessary argument to network simulator#
            //networkSimulator.With(netArgs).With(comArgs).With(radioArgs);
            networkSimulator.With(netArgs);
            _network = netArgs.Network;
            _network.AddRange(ImportDevices().ToArray());



            //Battery simulation initialization
            
           

            var testNetworkDevice = netArgs.Network;

            var batteryArgs = new BatteryArgs();
            batteryArgs.Batteries.Add(_battery);

		
			netArgs.Network.Items.ForEach(d =>
			{
				_battery = new BatteryPack();
				_battery.CutoffVoltage = 1.2F;
                _battery.State.Init(_battery);

                //var powerSupply = d.Parts.GetPowerSupply();
                d.Parts.Add(_battery);
			});

			
			


			var batterySim = new BatteryPackSimulator();
            batterySim
              .With(batteryArgs)
			  .With(netArgs).
			  With(runtime.Arguments);



			batterySim.Executed += (o, e) =>
			{
				var be = e.Arguments as BatteryArgs;
            };


            repo.Add(networkSimulator);
            //repo.Add(radioChannelSimulator);
            repo.Add(batterySim);

			//var countermeasureSimulator = new CountermeasureWpanSimulator();
			
			
            attackArgument.victimNodeName = victimNodeName;
            attackArgument.normalNodeName = normalNodeName;

            var simulator = repo.Add(new AttackWpanSimpleSimulator()
				.With(attackArgument).With(netArgs));

			//added countermeasure Simulator
			var countermeasureSimulator = repo.Add(new CountermeasureWpanSimulator().With(countermeasuresArgument).With(netArgs));

			runtime.BindSimulators(repo);
			runtime.IterationPassed += (o, e) =>
			{
				passed++;
				
			};

			var args = default(AttackWpanArgs);
			simulator.OnExecuting += (o, e) =>
			{
				args = e.Arguments as AttackWpanArgs;
				Log.Info($"Simulation will start now with '{args.Name}'.");
			};
			simulator.Executed += (o, e) =>
			{
				Log.Info($"Simulation done.");
			};

            // act
            

            if (runtime.Validate() == false)
			{
				throw new RuntimeException("The runtime validation failed.");
			}


			//calculation of average thresshold voltage for countermeasures. 

			//var allDevices = netArgs.Network.Items;
			//var sumVoltage = allDevices.Select(d => ((BatteryPack)d.Parts.GetPowerSupply()).State.Now.Voltage).Sum();
			//var averageVoltage = sumVoltage / allDevices.Count;
			
			//assigning average value to arguments
			


			//Is battery depleted then stop the simulation
			//Inject a method
			//
			//await runtime.RunAsync((r) =>
			//{

			//    var networkSimulator = r.Simulators[Domain.System.Enumerations.SimulationTypes.Network] as PeerToPeerNetworkSimulator;
			//    var networkArgs = networkSimulator.Arguments as NetworkArgs;
			//    var devices = networkArgs.Network.Items;
			//            //return maximumIteration or if battery gets depleted 
			//            //divide the code base for better debug and functionality.
			//            return !devices
			//        //.Select(s => s.Parts.GetPowerSupply() as BatteryPack)
			//        //.Any(s => s.State.IsDepleted);

			//        .Select(s => s.Parts.GetPowerSupply() as BatteryPack)
			//        .All(s => s.State.IsDepleted);

			//            //return false;
			//        });


			await runtime.RunAsync(iternations);

            //add other assert for different events 

            // assert
            Log.Info($"Counter Value='{ args.Counter}'.");

            Assert.IsNotNull(args, "The argument should not be null");

			Assert.AreEqual(iternations, passed, $"The runtime should have run '{iternations}' times instead of '{passed}'.");
			Assert.IsTrue(args.Counter > 0, "The counter should be greater than zero.");
		}

		public bool AttackSimulator(String attackName)
        {
            if (attackName=="BatteryAttack")
            {

            }

			return true;
        }


	}
}
