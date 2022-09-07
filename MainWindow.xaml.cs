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
			mpz_t result = new mpz_t();
			//mpz_t secretKeyA = new mpz_t();
			uint secretKeyA = 0;
			//mpz_t secretKeyB = new mpz_t();
			uint secretKeyB = 0;
			mpz_t sharedSecretKeyA = new mpz_t();
			mpz_t sharedSecretKeyB = new mpz_t();
			//mpz_t index = new mpz_t();
			mpz_t publicModulo = new mpz_t();
			mpz_t publicBasis = new mpz_t();
			mpz_t publicKeyA = new mpz_t();
			mpz_t publicKeyB = new mpz_t();
			mpz_t resultMod = new mpz_t();
			ulong versuche = 0;
			uint index = 0;

			publicModulo = publicKeyAinput.Text;
			publicBasis = publicKeyBinput.Text;
			publicKeyA = exchangeKeyAinput.Text;
			publicKeyB = exchangeKeyBinput.Text;

			gmp_lib.mpz_init(result);

			//gmp_lib.mpz_init(index);

			int i = 0;
			while (gmp_lib.mpz_cmp_ui(publicModulo, index) >= 0)
			{
				versuche++;
				index++;
				gmp_lib.mpz_pow_ui(result, publicBasis, index);
				gmp_lib.mpz_mod(resultMod, result, publicKeyA);
				
				//Modulo vom Result rechnen!ddd
				if (gmp_lib.mpz_cmp(resultMod, publicKeyA) == 0 && i < 1)
				{
					secretKeyA = index;
					i++;
				}
				else if (gmp_lib.mpz_cmp(resultMod, publicKeyB) == 0)
				{
					secretKeyB = index;
					i++;
				}
				if (i >= 2)
				{
					break;
				}
			}
			gmp_lib.mpz_pow_ui(sharedSecretKeyA, publicKeyB, secretKeyA);
			gmp_lib.mpz_pow_ui(sharedSecretKeyB, publicKeyA, secretKeyB);
			gmp_lib.mpz_mod(sharedSecretKeyA, sharedSecretKeyA, publicKeyA);
			gmp_lib.mpz_mod(sharedSecretKeyB, sharedSecretKeyB, publicKeyA);
			ausgabeTopR.Text = secretKeyA.ToString();
			ausgabeTop1R.Text = secretKeyB.ToString();
			ausgabeBottomR.Text = sharedSecretKeyA.ToString();
			ausgabeBottomR1.Text = sharedSecretKeyB.ToString();
		}

	}
}
