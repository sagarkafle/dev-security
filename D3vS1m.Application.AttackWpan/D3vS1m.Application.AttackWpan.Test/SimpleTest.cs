using System;
using System.Threading.Tasks;
using D3vS1m.Application.Channel;
using D3vS1m.Application.Network;
using D3vS1m.Application.Runtime;
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

		[TestMethod]
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
			Log.Info($"Counter Value='{ args.Counter}'.");
			//Assert.AreEqual(iternations, passed, $"The runtime should have run '{iternations}' times instead of '{passed}'.");
			Assert.IsTrue(args.Counter > 0, $"The counter should be greater than zero");
		}

		//[TestMethod]
		public async Task RunAttackSimulatorWithRuntime()
		{
			// arrange
			var iternations = 3;
			var passed = 0;

			var runtime = new RuntimeController(new BasicValidator());
			var repo = new SimulatorRepository();
			var simulator = repo.Add(new AttackWpanSimpleSimulator()
				.With(new AttackWpanArgs()));

			/*
			 * TODO: add more simulators and run the simualtion
			 * - You need: Network, Devices, Energy, Radio Channel
			 * - Add all arguments you need to your simulator with the With() method
			 * - Create a break condition to end the simulation runtime
			 */
			var radioArgs = new AdaptedFriisArgs();
			radioArgs.AttenuationOffset = 2;

			repo.Add(new PeerToPeerNetworkSimulator(runtime)
				.With(new NetworkArgs())
				.With(radioArgs));
			repo.Add(new AdaptedFriisSimulator(runtime).With(radioArgs));

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
			Assert.IsNotNull(args, "The argument should not be null");
			Assert.AreEqual(iternations, passed, $"The runtime should have run '{iternations}' times instead of '{passed}'.");
			Assert.IsTrue(args.Counter > 0, "The counter should be greater than zero.");
		}


	}
}
