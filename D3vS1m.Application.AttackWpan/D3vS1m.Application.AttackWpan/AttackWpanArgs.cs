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
            throw new NotImplementedException();
        }

    }
}
