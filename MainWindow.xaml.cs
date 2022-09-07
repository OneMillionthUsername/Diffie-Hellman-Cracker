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
			gmp_lib.mpz_pow_ui(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob);
			gmp_lib.mpz_mod(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);
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

			//init
			gmp_lib.gmp_randinit_default(rnd);
			gmp_lib.mpz_init(privateKeyAlice);
			gmp_lib.mpz_init(privateKeyBob);
			gmp_lib.mpz_init(exchangeKeyAlice);
			gmp_lib.mpz_init(exchangeKeyBob);
			gmp_lib.mpz_init(exchangeKeyAlice);
			gmp_lib.mpz_init(exchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);

			uint modulo = gmp_lib.gmp_urandomb_ui(rnd, 32);
			uint alicePrivate = gmp_lib.gmp_urandomb_ui(rnd, 8);
			uint basis = gmp_lib.gmp_urandomb_ui(rnd, 5);
			uint bobPrivate = gmp_lib.gmp_urandomb_ui(rnd, 8);

			generateAlicePublic.Text = modulo.ToString();
			generateBobPublic.Text = basis.ToString();
			generateAlicePrivate.Text = alicePrivate.ToString();
			generateBobPrivate.Text = bobPrivate.ToString();

			gmp_lib.mpz_ui_pow_ui(exchangeKeyAlice, basis, alicePrivate);
			gmp_lib.mpz_mod_ui(exchangeKeyAlice, exchangeKeyAlice, modulo);			
			
			gmp_lib.mpz_ui_pow_ui(exchangeKeyBob, basis, alicePrivate);
			gmp_lib.mpz_mod_ui(exchangeKeyBob, exchangeKeyBob, modulo);

			this.exchangeKeyAlice.Text = exchangeKeyAlice.ToString();
			this.exchangeKeyBob.Text = exchangeKeyBob.ToString();

			gmp_lib.mpz_pow_ui(sharedSecretKeyAlice, exchangeKeyBob, alicePrivate);
			gmp_lib.mpz_mod_ui(sharedSecretKeyAlice, sharedSecretKeyAlice, modulo);

			gmp_lib.mpz_pow_ui(sharedSecretKeyBob, exchangeKeyAlice, bobPrivate);
			gmp_lib.mpz_mod_ui(sharedSecretKeyBob, sharedSecretKeyBob, modulo);

			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			sharedSecretKeyBobBox.Text = sharedSecretKeyBob.ToString();
		}
	}
}
