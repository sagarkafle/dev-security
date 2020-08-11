using D3vS1m.Domain.Data.Arguments;
using D3vS1m.Domain.Runtime;
using D3vS1m.Domain.Simulation;
using D3vS1m.Domain.System.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

namespace D3vS1m.Application.AttackWpan
{
    public class AttackWpanSimpleSimulator : SimulatorBase
    {
        // -- fields

        private AttackWpanArgs _args;

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
            else return ArgsNotAdded(arguments.Name);

        }

        public override void Run()
        {
            base.BeforeExecution();


            //Simulation logic goes here

            _args.Counter++;


            base.AfterExecution();
        }

        // -- properties

       
        public override string Id => AttackWpanModule.AttackInWpan.Name;
        public override string Name => AttackWpanModule.AttackInWpan.Name;
        public override SimulationTypes Type => SimulationTypes.Custom;
        public override ArgumentsBase Arguments => _args;
    }
}
