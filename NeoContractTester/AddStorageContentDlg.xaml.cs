using System.Collections;
using System.Windows;

namespace NeoContractTester
{
    /// <summary>
    /// Interaction logic for AddStorageContent.xaml
    /// </summary>
    public partial class AddStorageContent : Window
    {
        private Hashtable _Data;

        public Hashtable Data {
            get { return _Data; }
        }

        public AddStorageContent()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            this._Data = new Hashtable();
            this._Data.Add("Key", this.KeyTextBox.Text.Replace(" ", string.Empty));
            this._Data.Add("Value", this.ValueTextBox.Text.Replace(" ", string.Empty));
            this._Data.Add("Type", this.TypeTextBox.Text.Replace(" ", string.Empty).ToLower());
            this.DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
