using NeoContractTester.Execution;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NeoContractTester
{
    /// <summary>
    /// Interaction logic for RunFunctionDialog.xaml
    /// </summary>
    public partial class RunFunctionDialog : Window
    {

        public string FunctionName {
            get;
            set;
        }

        public string ParamTypes {
            get;
            set;
        }

        public string ReturnType {
            get;
            set;
        }

        public string FilePath {
            get;
            set;
        }

        public Hashtable Store {
            get;
            set;
        }

        public RunFunctionDialog()
        {
            InitializeComponent();
        }

        private void CanvelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            var executor = new FunctionExecutor();
            var paramValues = this.ParamValuesTextBox.Text;
            byte[] fileContents = null;

            try
            {
                fileContents = File.ReadAllBytes(this.FilePath);
            }
            catch(FileNotFoundException fe)
            {
                MessageBox.Show("Please Specify a Valid AVM File in the AVM File Path text box.");
                return;
            }
            catch( Exception ex )
            {
                MessageBox.Show("Please Specify a Valid AVM File in the AVM File Path text box");
                return;
            }

            if (fileContents == null || fileContents.Length == 0)
            {
                MessageBox.Show("Invalid AVM file. This file has nothing in it.");
                return; 
            }

            this.ResultTextBox.Text = executor.ExecuteFunction(fileContents, this.FunctionName, this.ParamTypes.Split(','), paramValues.Replace(" ", string.Empty).Split(','), this.ReturnType, this.Store);
        }
    }
}
