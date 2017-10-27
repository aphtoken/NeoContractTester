using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for AddActionDialog.xaml
    /// </summary>
    public partial class AddActionDialog : Window
    {

        private Hashtable _Data;

        public Hashtable Data
        {
            get { return _Data; }
        }

        public AddActionDialog()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this._Data = new Hashtable();
            this._Data.Add("Name", this.FunctionNameTextBox.Text.Replace(" ", string.Empty));
            this._Data.Add("ParamTypes", this.ParameterTypesTextBox.Text.Replace(" ", string.Empty).ToLower());
            this._Data.Add("ReturnType", this.ReturnTypeTextBox.Text.Replace(" ", string.Empty).ToLower());
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
