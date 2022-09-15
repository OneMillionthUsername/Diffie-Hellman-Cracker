using Diffie_Hellman_Crack;
using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace WpfApp1 {
	class Testkey {
		public Testkey(params string[] ps) {
			G = ps[0];
			this.n = ps[1];
			ExchangeAlice = ps[2];
			ExchangeBob = ps[3];
			PrivateBob = ps[4];
			PrivateAlice = ps[5];
			SecretBob = ps[6];
			SecretAlice = ps[7];
		}

		public string G { get; set; }
		public string n { get; set; }
		public string ExchangeAlice { get; set; }
		public string ExchangeBob { get; set; }
		public string PrivateBob { get; set; }
		public string PrivateAlice { get; set; }
		public string SecretBob { get; set; }
		public string SecretAlice { get; set; }

	}
}
