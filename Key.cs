using Diffie_Hellman_Crack;
using Diffie_Hellman_Cracker.Interfaces;
using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1 {
	class Key : IExchangeValues {
		public Key(string g, string n, string exchangeAlice, string exchangeBob, string privateBob, string privateAlice, string secretBob, string secretAlice) {
			G = g;
			this.n = n;
			ExchangeAlice = exchangeAlice;
			ExchangeBob = exchangeBob;
			PrivateBob = privateBob;
			PrivateAlice = privateAlice;
			SecretBob = secretBob;
			SecretAlice = secretAlice;
		}

		public string G { get; set; }
		public string n { get; set; }
		public string ExchangeAlice { get; set; }
		public string ExchangeBob { get; set; }
		public string PrivateBob { get; set; }
		public string PrivateAlice { get; set; }
		public string SecretBob { get; set; }
		public string SecretAlice { get; set; }
		public void GetValues() {

		}
		public void SetValues() {

		}
	}
}
