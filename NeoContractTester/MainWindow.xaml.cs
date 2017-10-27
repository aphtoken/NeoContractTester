using NeoContractTester.Support;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NeoContractTester
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private LinkedList<StoreEntryDataGridItem> storeGridData;
        private LinkedList<ActionEntryDataGridItem> actionsGridData;

        public MainWindow()
        {
            InitializeComponent();
            this.storeGridData = new LinkedList<StoreEntryDataGridItem>();
            this.actionsGridData = new LinkedList<ActionEntryDataGridItem>();
            this.StoreDataGrid.ItemsSource = storeGridData;
            this.ActionsDataGrid.ItemsSource = actionsGridData;
        }

        private void Button_Click()
        {

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var dialog = new AddStorageContent();
            dialog.ShowDialog();
            if (dialog.DialogResult == false) {
                return;
            }
            var data = dialog.Data;
            storeGridData.AddLast(new StoreEntryDataGridItem((string)data["Key"], (string)data["Value"], (string)data["Type"]));
            this.StoreDataGrid.Items.Refresh();
        }

        private void AddActionButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddActionDialog();
            dialog.ShowDialog();
            if (dialog.DialogResult == false)
            {
                return;
            }
            var data = dialog.Data;
            actionsGridData.AddLast(new ActionEntryDataGridItem((string)data["Name"], (string)data["ParamTypes"], (string)data["ReturnType"]));
            this.ActionsDataGrid.Items.Refresh();

        }

        private void RunFunctionButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new RunFunctionDialog();
            var selectItem = (ActionEntryDataGridItem)this.ActionsDataGrid.SelectedItem;
            if (selectItem == null)
                return;
            dialog.FunctionName = selectItem.Name;
            dialog.ParamTypes = selectItem.ParameterTypes;
            dialog.ReturnType = selectItem.ReturnType;
            dialog.FilePath = FilePathTextBox.Text;
            var data = new Hashtable();

            foreach (StoreEntryDataGridItem item in this.storeGridData) {
                if (item.Type == "integer") {
                    data[item.Key] = BigInteger.Parse(item.Value).ToByteArray();
                }

                if (item.Type == "string") {
                    data[item.Key] = Encoding.UTF8.GetBytes(item.Value);
                }
            }
            dialog.Store = data;
            dialog.ShowDialog();

            foreach (string key in data.Keys) {
                var found = false;
                foreach (StoreEntryDataGridItem item in this.storeGridData) {
                    if (item.Key == key) {
                        found = true;
                        if (item.Type == "integer")
                        {
                            item.Value = new BigInteger((byte[])data[key]).ToString();
                        }

                        if (item.Type == "string")
                        {
                            item.Value = Encoding.UTF8.GetString((byte[])data[key]);
                        }
                    }
                }

                if (!found) {
                    storeGridData.AddLast(new StoreEntryDataGridItem(key,(string)data[key], "string"));                    
                }
            }
            this.StoreDataGrid.Items.Refresh();
        }
    }
}
