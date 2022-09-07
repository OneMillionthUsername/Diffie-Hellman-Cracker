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
	}
}
