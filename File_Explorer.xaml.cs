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

namespace Diffie_Hellman_Cracker {
	/// <summary>
	/// Interaktionslogik für Window1.xaml
	/// </summary>
	public partial class File_Explorer : Window, IExchangeValues {
		public string InterfaceG { get; set; }
		public string InterfaceN { get; set; }
		public string InterfaceExchangeAlice { get; set; }
		public string InterfaceExchangeBob { get; set; }
		public string InterfacePrivateBob { get; set; }
		public string InterfacePrivateAlice { get; set; }
		public string InterfaceSecretBob { get; set; }
		public string InterfaceSecretAlice { get; set; }

		public File_Explorer() {
			InitializeComponent();
		}
		public void SetValues() {
			throw new NotImplementedException();
		}
		public void GetValues() {
			throw new NotImplementedException();
		}
	}

}
