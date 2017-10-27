using Neo;
using Neo.Core;
using Neo.Cryptography;
using Neo.SmartContract;
using Neo.VM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NeoContractTester.Execution
{
    class CustomStorageContext : IInteropInterface
    {
        public UInt160 ScriptHash;

        public Hashtable data;

        public CustomStorageContext()
        {
            data = new Hashtable();
        }

        public byte[] ToArray()
        {
            return ScriptHash.ToArray();
        }
    }


    class CustomInteropService : InteropService
    {

        public CustomStorageContext storageContext;

        public Hashtable transactions;

        public CustomInteropService()
        {
            Register("Neo.Storage.GetContext", Storage_GetContext);
            Register("Neo.Storage.Get", Storage_Get);
            Register("Neo.Storage.Put", Storage_Put);
            Register("Neo.Runtime.CheckWitness", Runtime_CheckWitness);
            Register("Neo.Transaction.GetInputs", Transaction_GetInputs);
            Register("Neo.Transaction.GetOutputs", Transaction_GetOutputs);
            Register("Neo.Blockchain.GetTransaction", Blockchain_GetTransaction);
            Register("Neo.Input.GetHash", Input_GetHash);
            Register("Neo.Input.GetIndex", Input_GetIndex);
            Register("Neo.Output.GetScriptHash", Output_GetScriptHash);
            Register("Neo.Runtime.Notify", Runtime_Notify);
            this.storageContext = new CustomStorageContext();
            this.transactions = new Hashtable();
        }

        protected virtual bool Runtime_CheckWitness(ExecutionEngine engine)
        {
            return true;
        }

        public bool Storage_GetContext(ExecutionEngine engine)
        {
            this.storageContext.ScriptHash = new UInt160(engine.CurrentContext.ScriptHash);
            engine.EvaluationStack.Push(StackItem.FromInterface(this.storageContext));
            return true;
        }

        protected bool Storage_Get(ExecutionEngine engine)
        {
            CustomStorageContext context = engine.EvaluationStack.Pop().GetInterface<CustomStorageContext>();
            byte[] key = engine.EvaluationStack.Pop().GetByteArray();
            StorageItem item = new StorageItem
            {
                Value = (byte[])context.data[Encoding.UTF8.GetString(key)]
            };
            engine.EvaluationStack.Push(item?.Value ?? new byte[0]);
            return true;
        }

        protected bool Storage_Put(ExecutionEngine engine)
        {
            CustomStorageContext context = engine.EvaluationStack.Pop().GetInterface<CustomStorageContext>();
            byte[] key = engine.EvaluationStack.Pop().GetByteArray();
            if (key.Length > 1024) return false;
            byte[] value = engine.EvaluationStack.Pop().GetByteArray();
            context.data[Encoding.UTF8.GetString(key)] = value;
            return true;
        }

        protected bool Transaction_GetInputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Inputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected bool Blockchain_GetTransaction(ExecutionEngine engine)
        {
            byte[] hash = engine.EvaluationStack.Pop().GetByteArray();
            Transaction tx = (Transaction)this.transactions[hash];
            engine.EvaluationStack.Push(StackItem.FromInterface(tx));
            return true;
        }

        protected virtual bool Input_GetHash(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push(input.PrevHash.ToArray());
            return true;
        }

        protected virtual bool Output_GetScriptHash(ExecutionEngine engine)
        {
            TransactionOutput output = engine.EvaluationStack.Pop().GetInterface<TransactionOutput>();
            if (output == null) return false;
            engine.EvaluationStack.Push(output.ScriptHash.ToArray());
            return true;
        }

        protected virtual bool Input_GetIndex(ExecutionEngine engine)
        {
            CoinReference input = engine.EvaluationStack.Pop().GetInterface<CoinReference>();
            if (input == null) return false;
            engine.EvaluationStack.Push((int)input.PrevIndex);
            return true;
        }

        protected virtual bool Transaction_GetOutputs(ExecutionEngine engine)
        {
            Transaction tx = engine.EvaluationStack.Pop().GetInterface<Transaction>();
            if (tx == null) return false;
            engine.EvaluationStack.Push(tx.Outputs.Select(p => StackItem.FromInterface(p)).ToArray());
            return true;
        }

        protected virtual bool Runtime_Notify(ExecutionEngine engine)
        {
            StackItem state = engine.EvaluationStack.Pop();
            return true;
        }

    }

    class FunctionExecutor
    {
        public String ExecuteFunction(byte[] fileContents, string function, string[] parameterTypes, string[]parameterValues, string returnType, Hashtable store)
        {
            if( fileContents == null || fileContents.Length == 0)
                return "ERROR";

            var generator = new RNGCryptoServiceProvider();

            /** CREATE FAKE PREVIOUS TRANSACTION */
            var initialTransaction = new CustomTransaction(TransactionType.ContractTransaction);
            var transactionOutput = new TransactionOutput
            {
                ScriptHash = UInt160.Parse("A518E4F561F37782B39AB4F28B8D538F47B8AA6C"),
                Value = new Neo.Fixed8(10),
                AssetId = UInt256.Parse("B283C915F482DBC3A89189D865C4B42E74210BED735DCD307B1915C4E0A46C01")

            };

            initialTransaction.Outputs = new TransactionOutput[] { transactionOutput };

            /** CREATE FAKE CURRENT TRANSACTION */
            var coinRef = new CoinReference();
            coinRef.PrevHash = initialTransaction.Hash;
            coinRef.PrevIndex = 0;
            var currentTransaction = new CustomTransaction(TransactionType.ContractTransaction);
            currentTransaction.Inputs = new CoinReference[] { coinRef };
            var hash = currentTransaction.Hash;

            CustomInteropService service = new CustomInteropService();
            service.storageContext.data = store;

            service.transactions.Add(initialTransaction.Hash.ToArray(), initialTransaction);
            service.transactions.Add(currentTransaction.Hash.ToArray(), currentTransaction);

            var engine = new ExecutionEngine(currentTransaction, Crypto.Default, null, service);
            engine.LoadScript( fileContents );
            IList<ContractParameter> parameters = new List<ContractParameter>();

            for (int i = 0; i < parameterTypes.Length; i++) {
                var param = parameterTypes[i];
                if (param == "string") {
                    parameters.Add(new ContractParameter
                    {
                        Type = ContractParameterType.String,
                        Value = parameterValues[i]
                    });
                }

                if (param == "integer")
                {
                    parameters.Add(new ContractParameter
                    {
                        Type = ContractParameterType.Integer,
                        Value = BigInteger.Parse(parameterValues[i])
                    });
                }

            }

            var parameter = new ContractParameter
            {
                Type = ContractParameterType.Array,
                Value = parameters
            };

            using (ScriptBuilder sb = new ScriptBuilder())
            {

                Neo.VM.Helper.EmitPush(sb, parameter);
                sb.EmitPush(function);
                engine.LoadScript(sb.ToArray());
            }

            engine.Execute(); // start execution

            //var result = engine.EvaluationStack.Peek().GetBoolean(); // set the return value here
            if (engine.State == VMState.BREAK || engine.State == VMState.FAULT) {
                return "ERROR";
            }

            if (returnType == "boolean") {
                return engine.EvaluationStack.Peek().GetBoolean().ToString();
            }

            if (returnType == "string")
            {
                return engine.EvaluationStack.Peek().GetString();
            }

            if (returnType == "integer")
            {
                return engine.EvaluationStack.Peek().GetBigInteger().ToString();
            }

            return "Unknown Result Type";
           
        }
        
    }
}
