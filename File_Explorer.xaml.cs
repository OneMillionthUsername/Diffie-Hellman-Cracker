using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq;
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
using Diffie_Hellman_Cracker;
using Diffie_Hellman_Crack;

namespace WpfApp1 {
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class File_Explorer : Window {
		public File_Explorer() {
			InitializeComponent();
		}

		private void FileNames_Selected(object sender, RoutedEventArgs e) {
			//set values to selected row
		}

		private void Double_Click(object sender, MouseButtonEventArgs e) {
			object obj = fileNames.SelectedItem;
			//MainWindow mw = new MainWindow();
			//mw.generateAlicePrivate.Text = ((Key)obj).PrivateAlice;
			//mw.generateBobPrivate.Text = ((Key)obj).PrivateBob;
			//mw.generateExchangeKeyAinput.Text = ((Key)obj).ExchangeAlice;
			//mw.generateExchangeKeyBinput.Text = ((Key)obj).ExchangeBob;
			//mw.generatePublicKeyAinput.Text = ((Key)obj).G;
			//mw.generatePublicKeyBinput.Text = ((Key)obj).n;
			//mw.sharedSecretKeyAliceBox.Text = ((Key)obj).SecretAlice;
			//mw.sharedSecretKeyBobBox.Text = ((Key)obj).SecretBob;
			//mw.Show();
		}
	}
}
