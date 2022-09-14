using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1 {
	class Testkey {
		public Testkey(mpz_t g, mpz_t n, mpz_t exchangeAlice, mpz_t exchangeBob, mpz_t privateBob, mpz_t privateAlice, mpz_t secretBob, mpz_t secretAlice) {
			G = g;
			this.n = n;
			ExchangeAlice = exchangeAlice;
			ExchangeBob = exchangeBob;
			PrivateBob = privateBob;
			PrivateAlice = privateAlice;
			SecretBob = secretBob;
			SecretAlice = secretAlice;
		}

		public mpz_t G { get; set; }
		public mpz_t n { get; set; }
		public mpz_t ExchangeAlice { get; set; }
		public mpz_t ExchangeBob { get; set; }
		public mpz_t PrivateBob { get; set; }
		public mpz_t PrivateAlice { get; set; }
		public mpz_t SecretBob { get; set; }
		public mpz_t SecretAlice { get; set; }

	}
}
