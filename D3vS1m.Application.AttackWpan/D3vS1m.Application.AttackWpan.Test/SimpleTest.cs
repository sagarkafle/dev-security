using System;
using System.Threading.Tasks;
using D3vS1m.Application.Channel;
using D3vS1m.Application.Communication;
using D3vS1m.Application.Energy;
using D3vS1m.Application.Network;
using D3vS1m.Application.Runtime;
using D3vS1m.Application.Scene;
using D3vS1m.Application.Validation;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sin.Net.Domain.Persistence.Logging;

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
			var iternations = 3;
			var passed = 0;

			var runtime = new RuntimeController(new BasicValidator());
			var repo = new SimulatorRepository();
			var simulator = repo.Add(new AttackWpanSimpleSimulator()
				.With(new AttackWpanArgs()));



			//Radio channel simualtor args
			var comArgs = new WirelessCommArgs();
			var radioArgs = base.GetRadioArgs();
			var sceneArgs = new InvariantSceneArgs();


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
			networkSimulator.With(netArgs);
			//networkSimulator.With(netArgs).With(comArgs).With(radioArgs);
			_network = netArgs.Network;
			_network.AddRange(ImportDevices().ToArray());


			//Added necessary argument to radio channel simulator
			var radioChannelSimulator = new AdaptedFriisSimulator(runtime)
				.With(radioArgs)                   
				.With(comArgs)                     
				.With(sceneArgs)
				.With(netArgs);                     

            //Battery simulation initialization
            _battery = new BatteryPack();
            _battery.CutoffVoltage = 1.2F;
            _battery.State.Init(_battery);
            BatteryState s = _battery.State;

            var batteryArgs = new BatteryArgs();
            batteryArgs.Batteries.Add(_battery);


            var batterySim = new BatteryPackSimulator();
            batterySim
              .With(batteryArgs).
			  With(runtime.Arguments);
			
			batterySim.Executed += (o, e) =>
			{
				var be = e.Arguments as BatteryArgs;
				//be.Batteries[0].State.Now.Charge;
				
				Log.Info($"Initial Battery");
				Log.Info(be.Batteries[0].State.Initial.Charge.ToString());
				Log.Info(s.Initial.Charge.ToString());
				
				Log.Info($"Battery Charge Now");
			    Log.Info(s.Now.Charge.ToString());
				Log.Info(be.Batteries[0].State.Now.Charge.ToString());
				
				//be.Batteries[0].State.Initial.Charge;

            };


            //Adding different simulator to the main repo
            Log.Info($"Adding to repo--Start");

            repo.Add(networkSimulator.
				With(batteryArgs)
                .With(radioArgs));
            repo.Add(radioChannelSimulator);
            repo.Add(batterySim);

			Log.Info($"Adding to repo--End");


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
			await runtime.RunAsync(iternations);


			// assert
			Log.Info($"Counter Value='{ args.Counter}'.");
			Assert.IsNotNull(args, "The argument should not be null");

			Assert.AreEqual(iternations, passed, $"The runtime should have run '{iternations}' times instead of '{passed}'.");
			Assert.IsTrue(args.Counter > 0, "The counter should be greater than zero.");
		}


	}
}
