using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Diffie_Hellman_Cracker {
	class Key {
		#region PROPs
		public string G { get; set; }
		public string n { get; set; }
		public string ExchangeAlice { get; set; }
		public string ExchangeBob { get; set; }
		public string PrivateBob { get; set; }
		public string PrivateAlice { get; set; }
		public string SecretBob { get; set; }
		public string SecretAlice { get; set; }
		#endregion
		public Key() {

		}
		public Key(string g, string n, string exchangeAlice, string exchangeBob) {
			G = g;
			this.n = n;
			ExchangeAlice = exchangeAlice;
			ExchangeBob = exchangeBob;
		}
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
	}
}
