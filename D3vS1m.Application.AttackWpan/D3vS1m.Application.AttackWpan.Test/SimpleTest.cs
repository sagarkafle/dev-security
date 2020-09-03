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
			var iternations = 512;
			var passed = 0;
			var victimNodeName = "victimNode";
			var resultFilePath = @"C:\Users\nepho\source\repos\dev-security\D3vS1m.Application.AttackWpan\D3vS1m.Application.AttackWpan.Test\output\output.txt";

			var runtime = new RuntimeController(new BasicValidator());
			var repo = new SimulatorRepository();
			var attackArgument = new AttackWpanArgs();
			attackArgument.resultFilePath = resultFilePath;
			attackArgument.attackName = "BatteryExhaustionAttack";
			//var simulator = repo.Add(new AttackWpanSimpleSimulator()
			//	.With(new AttackWpanArgs()));





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

			
			
            attackArgument.victimNodeName = victimNodeName;

            var simulator = repo.Add(new AttackWpanSimpleSimulator()
				.With(attackArgument).With(netArgs));



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

			//Is battery depleted then stop the simulation
			//Inject a method
			//
			await runtime.RunAsync((r) =>
			{

				var networkSimulator = r.Simulators[Domain.System.Enumerations.SimulationTypes.Network] as PeerToPeerNetworkSimulator;
				var networkArgs = networkSimulator.Arguments as NetworkArgs;
				var devices = networkArgs.Network.Items;
				//return maximumIteration or if battery gets depleted 
				return devices
					.Select(s => s.Parts.GetPowerSupply() as BatteryPack)
					.Any(s => s.State.IsDepleted);

				//return false;
			});


			// assert
			//Log.Info($"Counter Value='{ args.Counter}'.");
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
