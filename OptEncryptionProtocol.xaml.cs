using Math.Gmp.Native;
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

namespace Diffie_Hellman_Crack {
	/// <summary>
	/// Interaktionslogik für OptEncryptionProtocol.xaml
	/// </summary>
	public partial class OptEncryptionProtocol : Window {
		
		public OptEncryptionProtocol() {
			InitializeComponent();
		}
		private void Key_length_8_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 8;
		}

		private void Key_length_16_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 16;
		}

		private void Key_length_32_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 32;
		}

		private void Key_length_64_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 64;
		}

		private void Key_length_128_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 128;
		}
		private void Prme_Key_length_8_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 8;
		}

		private void Prme_Key_length_16_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 16;
		}


		private void Prme_Key_length_32_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 32;
		}

		private void Prme_Key_length_64_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 64;
		}

		private void Prme_Key_length_128_Selected(object sender, RoutedEventArgs e) {
			MainWindow.BitStandard = 128;
		}
	}
}
