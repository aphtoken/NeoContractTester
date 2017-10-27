using Neo.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoContractTester.Execution
{
    class CustomTransaction : Transaction
    {
        public CustomTransaction(TransactionType type) : base(type)
        {
            this.Version = 1;
            this.Inputs = new CoinReference[0];
            this.Outputs = new TransactionOutput[0];
            this.Attributes = new TransactionAttribute[0];
        }
    }
}
