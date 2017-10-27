using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoContractTester.Support
{   
    class StoreEntryDataGridItem
    {
        public string Key
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        public string Type {
            get;
            set;
        }

        public StoreEntryDataGridItem(string keyParam, string valueParam, string typeParam) {
            this.Key = keyParam;
            this.Value = valueParam;
            this.Type = typeParam;
        }

    }
}
