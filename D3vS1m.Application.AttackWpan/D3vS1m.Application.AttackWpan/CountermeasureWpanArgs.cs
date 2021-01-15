using D3vS1m.Domain.Data.Arguments;
using System;
using System.Collections.Generic;
using System.Text;

namespace D3vS1m.Application.AttackWpan
{
    public class CountermeasureWpanArgs : ArgumentsBase
    {
        public CountermeasureWpanArgs() : base()
        {
            Key = CountermeasureWpanModel.CountermeasureInWpan.Key;
            //Reset();
        }

        public float averageVoltage { get; set; }
        public float averageCharge { get; set; }

        public bool applyCountermeasure { get; set; }
        public bool generateAlert { get; set; }

       public String responsibleStakeholder { get; set; }

        public float detectPercent { get; set; }

        public override void Reset()
        {
            //throw new NotImplementedException();
        }
    }
}
