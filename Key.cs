using Diffie_Hellman_Crack;
using Diffie_Hellman_Cracker.Interfaces;
using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Diffie_Hellman_Cracker {
	class Key : IExchangeValues {
		public string InterfaceG { get; set; }
		public string InterfaceN { get; set; }
		public string InterfaceExchangeAlice { get; set; }
		public string InterfaceExchangeBob { get; set; }
		public string InterfacePrivateBob { get; set; }
		public string InterfacePrivateAlice { get; set; }
		public string InterfaceSecretBob { get; set; }
		public string InterfaceSecretAlice { get; set; }
		#region DECLARATION
		private mpz_t exponent = new mpz_t();
		private mpz_t gcd = new mpz_t();
		private mpz_t ext_gcd_s = new mpz_t();
		private mpz_t ext_gcd_t = new mpz_t();
		private readonly gmp_randstate_t rnd = new gmp_randstate_t();
		private mpz_t group = new mpz_t();
		private mpz_t basis = new mpz_t();
		private readonly mpz_t alicePrivate = new mpz_t();
		private readonly mpz_t bobPrivate = new mpz_t();
		private readonly mpz_t sharedSecretKeyAlice = new mpz_t();
		private readonly mpz_t sharedSecretKeyBob = new mpz_t();
		private mpz_t ExchangeKeyAlice = new mpz_t();
		private mpz_t ExchangeKeyBob = new mpz_t();
		private readonly mpz_t secretKeyBob = new mpz_t();
		private readonly mpz_t secretKeyAlice = new mpz_t();
		private readonly mpz_t result = new mpz_t();
		#endregion
		#region PROPS
		public mpz_t Exponent { get => exponent; set => exponent = value; }
		public mpz_t Gcd { get => gcd; set => gcd = value; }
		public mpz_t Ext_gcd_s { get => ext_gcd_s; set => ext_gcd_s = value; }
		public mpz_t Ext_gcd_t { get => ext_gcd_t; set => ext_gcd_t = value; }
		public gmp_randstate_t Rnd => rnd;
		public mpz_t Group { get => group; set => group = value; }
		public mpz_t Basis { get => basis; set => basis = value; }
		public mpz_t AlicePrivate => alicePrivate;
		public mpz_t BobPrivate => bobPrivate;
		public mpz_t SharedSecretKeyAlice => sharedSecretKeyAlice;
		public mpz_t SharedSecretKeyBob => sharedSecretKeyBob;
		public mpz_t ExchangeAlice { get => ExchangeKeyAlice; set => ExchangeKeyAlice = value; }
		public mpz_t ExchangeBob { get => ExchangeKeyBob; set => ExchangeKeyBob = value; }
		public mpz_t SecretKeyBob => secretKeyBob;
		public mpz_t SecretKeyAlice => secretKeyAlice;
		public mpz_t Result => result; 
		#endregion
		public Key() {
			#region INIT VARS
			gmp_lib.mpz_init(Exponent);
			gmp_lib.gmp_randinit_mt(rnd);
			gmp_lib.gmp_randseed_ui(rnd, (uint)DateTime.UtcNow.Second);
			gmp_lib.mpz_init(alicePrivate);
			gmp_lib.mpz_init(bobPrivate);
			gmp_lib.mpz_init(Group);
			gmp_lib.mpz_init(Basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(Gcd);
			gmp_lib.mpz_init(Ext_gcd_s);
			gmp_lib.mpz_init(Ext_gcd_t);
			#endregion
		}
		public Key(string g, string n, string excAlice, string excBob, string privateBob, string privateAlice, string secretBob, string secretAlice) {
			InterfaceG = g;
			InterfaceN = n;
			InterfaceExchangeAlice = excAlice;
			InterfaceExchangeBob = excBob;
			InterfacePrivateBob = privateBob;
			InterfacePrivateAlice = privateAlice;
			InterfaceSecretBob = secretBob;
			InterfaceSecretAlice = secretAlice;
			#region INIT VARS
			gmp_lib.mpz_init(Exponent);
			gmp_lib.gmp_randinit_mt(rnd);
			gmp_lib.gmp_randseed_ui(rnd, (uint)DateTime.UtcNow.Second);
			gmp_lib.mpz_init(alicePrivate);
			gmp_lib.mpz_init(bobPrivate);
			gmp_lib.mpz_init(Group);
			gmp_lib.mpz_init(Basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(Gcd);
			gmp_lib.mpz_init(Ext_gcd_s);
			gmp_lib.mpz_init(Ext_gcd_t);
			#endregion
		}
		~Key() {
			gmp_lib.gmp_randclear(Rnd);
			gmp_lib.mpz_clears(Group, Basis, AlicePrivate, BobPrivate, SharedSecretKeyAlice, SharedSecretKeyBob, ExchangeKeyAlice, ExchangeKeyBob, SecretKeyBob, SecretKeyAlice, Result);
		}

		public void GetValues() {

		}
		public void SetValues() {

		}
	}
}
