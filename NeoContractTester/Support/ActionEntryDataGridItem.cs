using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeoContractTester.Support
{   
    class ActionEntryDataGridItem
    {
        public string Name
        {
            get;
            set;
        }

        public string ParameterTypes
        {
            get;
            set;
        }

        public string ReturnType {
            get;
            set;
        }

        public ActionEntryDataGridItem(string nameParam, string paramTypesParam, string returnTypeParam) {
            this.Name = nameParam;
            this.ParameterTypes = paramTypesParam;
            this.ReturnType = returnTypeParam;
        }

    }
}
