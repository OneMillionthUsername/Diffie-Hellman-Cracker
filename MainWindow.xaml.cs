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
			uint secretKeyAlice = 0;
			uint secretKeyBob = 0;
			ulong versuche = 0;
			uint exponent = 0;
			mpz_t result = new mpz_t();
			mpz_t sharedSecretKeyAlice = new mpz_t();
			mpz_t sharedSecretKeyBob = new mpz_t();
			mpz_t modulo = new mpz_t();
			mpz_t basis = new mpz_t();
			mpz_t ExchangeKeyAlice = new mpz_t();
			mpz_t ExchangeKeyBob = new mpz_t();
			//init
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);

			modulo = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;

			int i = 0;
			while (gmp_lib.mpz_cmp_ui(modulo, exponent) >= 0)
			{
				versuche++;
				exponent++;
				gmp_lib.mpz_pow_ui(result, basis, exponent);
				gmp_lib.mpz_mod(result, result, modulo);

				
				//Modulo vom Result rechnen!
				if (gmp_lib.mpz_cmp(result, ExchangeKeyAlice) == 0)
				{
					secretKeyAlice = exponent;
					i++;
				}
				if (gmp_lib.mpz_cmp(result, ExchangeKeyBob) == 0)
				{
					secretKeyBob = exponent;
					i++;
				}
				if (i >= 2)
				{
					break;
				}
			}
			gmp_lib.mpz_pow_ui(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice);
			gmp_lib.mpz_mod(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);
			gmp_lib.mpz_pow_ui(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob);
			gmp_lib.mpz_mod(sharedSecretKeyBob, sharedSecretKeyBob, modulo);

			ausgabeTopR.Text = secretKeyAlice.ToString();
			ausgabeTop1R.Text = secretKeyBob.ToString();
			ausgabeBottomR.Text = sharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = versuche.ToString();
		}

		private void BtnCreateKey(object sender, RoutedEventArgs e)
		{
			mpz_t privateKeyAlice = new mpz_t();
			mpz_t privateKeyBob = new mpz_t();
			mpz_t sharedSecretKeyAlice = new mpz_t();
			mpz_t sharedSecretKeyBob = new mpz_t();
			mpz_t exchangeKeyAlice = new mpz_t();
			mpz_t exchangeKeyBob = new mpz_t();
			gmp_randstate_t rnd = new gmp_randstate_t();
			uint seed = int.MaxValue/2;

			//init
			gmp_lib.gmp_randinit_default(rnd);
			gmp_lib.gmp_randseed_ui(rnd, seed);

			gmp_lib.mpz_init(privateKeyAlice);
			gmp_lib.mpz_init(privateKeyBob);
			gmp_lib.mpz_init(exchangeKeyAlice);
			gmp_lib.mpz_init(exchangeKeyBob);
			gmp_lib.mpz_init(exchangeKeyAlice);
			gmp_lib.mpz_init(exchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			//clear random state

			//erstelle öffentlichen Handshake
			uint modulo = gmp_lib.gmp_urandomb_ui(rnd, 32);
			uint basis = gmp_lib.gmp_urandomb_ui(rnd, 8);

			//erstelle private Schlüssel
			uint alicePrivate = gmp_lib.gmp_urandomb_ui(rnd, 8);
			uint bobPrivate = gmp_lib.gmp_urandomb_ui(rnd, 8);
			gmp_lib.gmp_randclear(rnd);

			//erstelle exchange key für Alice
			gmp_lib.mpz_ui_pow_ui(exchangeKeyAlice, basis, alicePrivate);
			gmp_lib.mpz_mod_ui(exchangeKeyAlice, exchangeKeyAlice, modulo);			

			//erstelle exchange key für Bob
			gmp_lib.mpz_ui_pow_ui(exchangeKeyBob, basis, bobPrivate);
			gmp_lib.mpz_mod_ui(exchangeKeyBob, exchangeKeyBob, modulo);

			//erstelle den secret shared Schlüssel
			gmp_lib.mpz_pow_ui(sharedSecretKeyAlice, exchangeKeyBob, alicePrivate);
			gmp_lib.mpz_mod_ui(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);

			gmp_lib.mpz_pow_ui(sharedSecretKeyBob, exchangeKeyAlice, bobPrivate);
			gmp_lib.mpz_mod_ui(sharedSecretKeyBob, sharedSecretKeyBob, modulo);

			publicAliceCopy = generateAlicePublic.Text = modulo.ToString();
			publicBobCopy = generateBobPublic.Text = basis.ToString();
			generateAlicePrivate.Text = alicePrivate.ToString();
			generateBobPrivate.Text = bobPrivate.ToString();
			exchangeAliceCopy = this.exchangeKeyAlice.Text = exchangeKeyAlice.ToString();
			exchangeBobCopy = this.exchangeKeyBob.Text = exchangeKeyBob.ToString();
			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			sharedSecretKeyBobBox.Text = sharedSecretKeyBob.ToString();
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
		}

		private void BtnCopyClick(object sender, RoutedEventArgs e)
		{
			publicKeyAinput.Text = publicAliceCopy;
			publicKeyBinput.Text = publicBobCopy;
			exchangeKeyAinput.Text = exchangeAliceCopy;
			exchangeKeyBinput.Text = exchangeBobCopy;
		}
	}
}
