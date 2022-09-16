using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diffie_Hellman_Cracker.Interfaces {
	interface IExchangeValues {
		string InterfaceG { get; set; }
		string InterfaceN { get; set; }
		string InterfaceExchangeAlice { get; set; }
		string InterfaceExchangeBob { get; set; }
		string InterfacePrivateBob { get; set; }
		string InterfacePrivateAlice { get; set; }
		string InterfaceSecretBob { get; set; }
		string InterfaceSecretAlice { get; set; }
		void SetValues();
		void GetValues();
	}
}
