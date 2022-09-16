using Diffie_Hellman_Cracker.Interfaces;
using Newtonsoft.Json;
using System;
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

namespace WpfApp1 {
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class File_Explorer : Window, IExchangeValues {
		public string G { get; set; }
		public string n { get; set; }
		public string ExchangeAlice { get; set; }
		public string ExchangeBob { get; set; }
		public string PrivateBob { get; set; }
		public string PrivateAlice { get; set; }
		public string SecretBob { get; set; }
		public string SecretAlice { get; set; }

		public File_Explorer() {
			InitializeComponent();
		}

		private void FileNames_Selected(object sender, RoutedEventArgs e) {
			//set values to selected row
		}

		private void Double_Click(object sender, MouseButtonEventArgs e) {
			for (int i = 0; i < fileNames.Columns.Count; i++) {
				DataGridCellInfo dataGridCellInfo = new DataGridCellInfo();

				string deserialize = DataGrid.CurrentItemProperty.ToString();
				Testkey item = JsonConvert.DeserializeObject<Testkey>(deserialize);

			}
		}

		public void SetValues() {
			throw new NotImplementedException();
		}

		public void GetValues() {
			throw new NotImplementedException();
		}
	}

}
