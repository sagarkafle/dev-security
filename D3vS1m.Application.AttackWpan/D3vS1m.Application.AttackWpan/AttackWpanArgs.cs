using D3vS1m.Domain.Data.Arguments;
using System;

namespace D3vS1m.Application.AttackWpan
{
    public class AttackWpanArgs : ArgumentsBase
    {
        public AttackWpanArgs() : base()
        {
            Key = AttackWpanModule.AttackInWpan.Key;
            Reset();
        }

        public override void Reset()
        {
            Name = AttackWpanModule.AttackInWpan.Name;
        }

        // -- properties

        public int Counter { get; set; }


        public string attackName { get; set; }
        public string victimNodeName { get; set; }
        public string normalNodeName { get; set; }
        public string victimNoderesultFilePath { get; set; }
        public string normalNoderesultFilePath { get; set; }
        public string CurrentStateVoltageCOnsumptionCsvFilePath { get; set; }
        //public string CurrentStateVoltageCOnsumptionOutputFilePath { get; set; }
        public string CurrentStateChargeCOnsumptionCsvFilePath { get; set; }
        //public string CurrentStateVoltageCOnsumptionOutputFilePath { get; set; }

        public float dichargeAmountNormal { get; set; }

        public float AverageVoltage { get; set; }
        public float AverageCharge { get; set; }
        public int sleepCounter { get; set; }

    }
}
