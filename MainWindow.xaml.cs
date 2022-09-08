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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using Math.Gmp.Native;

namespace WpfApp1
{
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public string publicAliceCopy { get; set; }
		public string publicBobCopy { get; set; }
		public string exchangeAliceCopy { get; set; }
		public string exchangeBobCopy { get; set; }

		public MainWindow()
		{
			InitializeComponent();
		}
		private void BtnCrackKey(object sender, RoutedEventArgs e)
		{
			//uint secretKeyAlice = 0;
			uint exponent = 0;
			//uint secretKeyBob = 0;
			ulong versuche = 0;
			mpz_t secretKeyBob = new mpz_t();
			mpz_t secretKeyAlice = new mpz_t();
			//mpz_t exponent = new mpz_t();
			mpz_t result = new mpz_t();
			mpz_t sharedSecretKeyAlice = new mpz_t();
			mpz_t sharedSecretKeyBob = new mpz_t();
			mpz_t modulo = new mpz_t();
			mpz_t basis = new mpz_t();
			mpz_t ExchangeKeyAlice = new mpz_t();
			mpz_t ExchangeKeyBob = new mpz_t();
			//init
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);
			//gmp_lib.mpz_init(exponent);
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);

			Stopwatch stopwatch = new Stopwatch();

			if (publicKeyAinput.Text.Length <= 0 || publicKeyBinput.Text.Length <= 0
				|| exchangeKeyAinput.Text.Length <= 0 || exchangeKeyBinput.Text.Length <= 0)
			{
				CopyPublicData();
			}

			modulo = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;

			//set default values if input is empty
			//if (publicKeyAinput.Text.Length <= 0 || publicKeyBinput.Text.Length <= 0
			//	|| exchangeKeyAinput.Text.Length <= 0 || exchangeKeyBinput.Text.Length <= 0)
			//{
			//	publicKeyAinput.Text = 10.ToString();
			//	publicKeyBinput.Text = 10.ToString();
			//	exchangeKeyAinput.Text = 10.ToString();
			//	exchangeKeyBinput.Text = 10.ToString();
			//}

			int i = 0;
			stopwatch.Start();
			while (gmp_lib.mpz_cmp_ui(modulo, exponent) >= 0)
			{
				versuche++;
				exponent++;
				
				//gmp_lib.mpz_pow_ui(result, basis, exponent);
				//gmp_lib.mpz_mod(result, result, modulo);
				gmp_lib.mpz_powm_ui(result, basis, exponent, modulo);

				if (gmp_lib.mpz_cmp(result, ExchangeKeyAlice) == 0)
				{
					gmp_lib.mpz_init_set_ui(secretKeyAlice, exponent);
					i++;
				}
				if (gmp_lib.mpz_cmp(result, ExchangeKeyBob) == 0)
				{
					gmp_lib.mpz_init_set_ui(secretKeyBob, exponent);
					i++;
				}
				if (i >= 2)
				{
					break;
				}
			}
			//gmp_lib.mpz_pow_ui(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice);
			//gmp_lib.mpz_mod(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);

			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice, modulo);

			//gmp_lib.mpz_pow_ui(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob);
			//gmp_lib.mpz_mod(sharedSecretKeyBob, sharedSecretKeyBob, modulo);

			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob, modulo);

			long time = stopwatch.ElapsedMilliseconds;
			stopwatch.Stop();

			ausgabeTopR.Text = secretKeyAlice.ToString();
			ausgabeTop1R.Text = secretKeyBob.ToString();
			ausgabeBottomR.Text = sharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = versuche.ToString();
			ZeitAusgabe.Text = time.ToString() + " ms";
			//gmp_lib.mpz_clears(result, sharedSecretKeyAlice, sharedSecretKeyBob, modulo, basis, ExchangeKeyAlice, ExchangeKeyBob);
		}

		private void BtnCreateKey(object sender, RoutedEventArgs e)
		{
			mpz_t modulo = new mpz_t();
			mpz_t basis = new mpz_t();
			mpz_t alicePrivate = new mpz_t();
			mpz_t bobPrivate = new mpz_t();
			mpz_t privateKeyAlice = new mpz_t();
			mpz_t privateKeyBob = new mpz_t();
			mpz_t sharedSecretKeyAlice = new mpz_t();
			mpz_t sharedSecretKeyBob = new mpz_t();
			mpz_t exchangeKeyAlice = new mpz_t();
			mpz_t exchangeKeyBob = new mpz_t();
			gmp_randstate_t rnd = new gmp_randstate_t();
			gmp_lib.gmp_randinit_mt(rnd);

			gmp_lib.gmp_randseed_ui(rnd, 100000U);

			//init
			gmp_lib.mpz_init(alicePrivate);
			gmp_lib.mpz_init(bobPrivate);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(privateKeyAlice);
			gmp_lib.mpz_init(privateKeyBob);
			gmp_lib.mpz_init(exchangeKeyAlice);
			gmp_lib.mpz_init(exchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);

			//erstelle öffentlichen Handshake
			gmp_lib.mpz_urandomb(modulo, rnd, 32);
			gmp_lib.mpz_urandomb(basis, rnd, 8);

			//erstelle private Schlüssel
			gmp_lib.mpz_urandomb(alicePrivate, rnd, 16);
			gmp_lib.mpz_urandomb(bobPrivate, rnd, 16);

			//uint alice_privat = gmp_lib.mpz_get_ui(alicePrivate);
			//uint bob_privat = gmp_lib.mpz_get_ui(bobPrivate);

			//erstelle exchange key für Alice
			//gmp_lib.mpz_pow_ui(exchangeKeyAlice, basis, alice_privat);
			//gmp_lib.mpz_mod(exchangeKeyAlice, exchangeKeyAlice, modulo);

			gmp_lib.mpz_powm(exchangeKeyAlice, basis, alicePrivate, modulo);

			//erstelle exchange key für Bob
			//gmp_lib.mpz_pow_ui(exchangeKeyBob, basis, bob_privat);
			//gmp_lib.mpz_mod(exchangeKeyBob, exchangeKeyBob, modulo);

			gmp_lib.mpz_powm(exchangeKeyBob, basis, bobPrivate, modulo);

			//erstelle den secret shared Schlüssel
			//gmp_lib.mpz_pow_ui(sharedSecretKeyAlice, exchangeKeyBob, alice_privat);
			//gmp_lib.mpz_mod(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);

			gmp_lib.mpz_powm(sharedSecretKeyAlice, exchangeKeyBob, alicePrivate, modulo);

			//gmp_lib.mpz_pow_ui(sharedSecretKeyBob, exchangeKeyAlice, bob_privat);
			//gmp_lib.mpz_mod(sharedSecretKeyBob, sharedSecretKeyBob, modulo);

			gmp_lib.mpz_powm(sharedSecretKeyBob, exchangeKeyAlice, bobPrivate, modulo);


			//output
			publicAliceCopy = generateAlicePublic.Text = modulo.ToString();
			publicBobCopy = generateBobPublic.Text = basis.ToString();
			generateAlicePrivate.Text = alicePrivate.ToString();
			generateBobPrivate.Text = bobPrivate.ToString();
			exchangeAliceCopy = this.exchangeKeyAlice.Text = exchangeKeyAlice.ToString();
			exchangeBobCopy = this.exchangeKeyBob.Text = exchangeKeyBob.ToString();
			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			sharedSecretKeyBobBox.Text = sharedSecretKeyBob.ToString();

			//clear random states and vars
			gmp_lib.gmp_randclear(rnd);
			//gmp_lib.mpz_clears(modulo, basis, alicePrivate, bobPrivate, privateKeyAlice, exchangeKeyBob, exchangeKeyAlice, exchangeKeyBob, sharedSecretKeyAlice, sharedSecretKeyBob);
		}
		private void BtnClearKey(object sender, RoutedEventArgs e)
		{
			publicKeyAinput.Clear();
			publicKeyBinput.Clear();
			exchangeKeyAinput.Clear();
			exchangeKeyBinput.Clear();
			ausgabeTopR.Clear();
			ausgabeTop1R.Clear();
			ausgabeBottomR.Clear();
			ausgabeBottomR1.Clear();
			generateAlicePublic.Clear();
			generateBobPublic.Clear();
			generateAlicePrivate.Clear();
			generateBobPrivate.Clear();
			exchangeKeyAlice.Clear();
			exchangeKeyBob.Clear();
			sharedSecretKeyAliceBox.Clear();
			sharedSecretKeyBobBox.Clear();
			ZeitAusgabe.Clear();
		}
		private void BtnCopyClick(object sender, RoutedEventArgs e)
		{
			CopyPublicData();
		}
		private void CopyPublicData()
		{
			publicKeyAinput.Text = generateAlicePublic.Text;
			publicKeyBinput.Text = generateBobPublic.Text;
			exchangeKeyAinput.Text = exchangeKeyAlice.Text;
			exchangeKeyBinput.Text = exchangeKeyBob.Text;
		}
	}
}
