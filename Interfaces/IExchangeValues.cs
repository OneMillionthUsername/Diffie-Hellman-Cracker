using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Cracker.Interfaces {
	interface IExchangeValues {
		string G { get; set; }
		string n { get; set; }
		string ExchangeAlice { get; set; }
		string ExchangeBob { get; set; }
		string PrivateBob { get; set; }
		string PrivateAlice { get; set; }
		string SecretBob { get; set; }
		string SecretAlice { get; set; }
		void SetValues();
		void GetValues();
	}
}
