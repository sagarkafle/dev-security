using Microsoft.VisualStudio.TestTools.UnitTesting;
using D3vS1m.Application.AttackWpan;
using System;
using System.Collections.Generic;
using System.Text;

namespace D3vS1m.Application.AttackWpan.Test
{
    [TestClass]
    public class SimpleTest : TestBase
    {
        AttackWpanArgs _attackWpanArgs;


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
        public void SimpleSimilatorTest()
        {
            // arrange
            //initialization of different arguments required for the simulation test

            var attackWpanSim = new AttackWpanSimpleSimulator();
            _attackWpanArgs = new AttackWpanArgs();
            //LoadAntennaData(_attackWpanArgs);

            // assert
            Assert.IsTrue(true,"yay");
        }


    }
}
