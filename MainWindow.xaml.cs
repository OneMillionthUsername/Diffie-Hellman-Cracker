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
		public mp_bitcnt_t BitStandard { get; set; } = new mp_bitcnt_t(32);
		public mp_bitcnt_t BitStandardPrime { get; set; } = new mp_bitcnt_t(16);

		public delegate bool CheckInput();

		List<TextBox> inputBoxes = new List<TextBox>();
		List<TextBox> generatedBoxes = new List<TextBox>();
		List<TextBox> allBoxes = new List<TextBox>();

		private ulong Versuche = 0;
		private uint exponent = 0;
		private gmp_randstate_t rnd = new gmp_randstate_t();
		private mpz_t modulo = new mpz_t();
		private mpz_t basis = new mpz_t();
		private mpz_t alicePrivate = new mpz_t();
		private mpz_t bobPrivate = new mpz_t();
		private mpz_t sharedSecretKeyAlice = new mpz_t();
		private mpz_t sharedSecretKeyBob = new mpz_t();
		private mpz_t ExchangeKeyAlice = new mpz_t();
		private mpz_t ExchangeKeyBob = new mpz_t();
		private mpz_t secretKeyBob = new mpz_t();
		private mpz_t secretKeyAlice = new mpz_t();
		private mpz_t result = new mpz_t();
		private Stopwatch stopwatch = new Stopwatch();

		public MainWindow()
		{
			InitializeComponent();
			#region INIT VARS
			gmp_lib.gmp_randinit_mt(rnd);
			gmp_lib.gmp_randseed_ui(rnd, (uint)DateTime.UtcNow.Second);
			gmp_lib.mpz_init(alicePrivate);
			gmp_lib.mpz_init(bobPrivate);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);
			gmp_lib.mpz_init(result);
			#endregion
			#region FILL LISTS
			inputBoxes.Add(publicKeyAinput);
			inputBoxes.Add(publicKeyBinput);
			inputBoxes.Add(exchangeKeyAinput);
			inputBoxes.Add(exchangeKeyBinput);

			generatedBoxes.Add(generatePublicKeyAinput);
			generatedBoxes.Add(generatePublicKeyBinput);
			generatedBoxes.Add(generateExchangeKeyAinput);
			generatedBoxes.Add(generateExchangeKeyBinput);

			allBoxes.Add(publicKeyAinput);
			allBoxes.Add(publicKeyBinput);
			allBoxes.Add(exchangeKeyAinput);
			allBoxes.Add(exchangeKeyBinput);
			allBoxes.Add(ausgabeTopR);
			allBoxes.Add(ausgabeTop1R);
			allBoxes.Add(ausgabeBottomR);
			allBoxes.Add(ausgabeBottomR1);
			allBoxes.Add(generatePublicKeyAinput);
			allBoxes.Add(generatePublicKeyBinput);
			allBoxes.Add(generateAlicePrivate);
			allBoxes.Add(generateBobPrivate);
			allBoxes.Add(generateExchangeKeyAinput);
			allBoxes.Add(generateExchangeKeyBinput);
			allBoxes.Add(sharedSecretKeyAliceBox);
			allBoxes.Add(sharedSecretKeyBobBox);
			allBoxes.Add(ZeitAusgabe); 
			#endregion
		}
		~MainWindow()
		{
			gmp_lib.gmp_randclear(rnd);
			gmp_lib.mpz_clears(modulo, basis, alicePrivate, bobPrivate, sharedSecretKeyAlice, sharedSecretKeyBob, ExchangeKeyAlice, ExchangeKeyBob, secretKeyBob, secretKeyAlice, result);
		}
		private void BtnCrackKey(object sender, RoutedEventArgs e)
		{
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);

			CheckInput checkInput = CheckInputInt;
			checkInput += CheckInputPrime;
			checkInput += CheckInputEmpty;
			foreach (CheckInput item in checkInput.GetInvocationList())
			{
				if (item.Invoke())
				{
					continue;
				}
				else
				{
					return;
				}
			}

			/////////
			if(CheckInputPrime(basis, modulo))
			{

			}

			/////////
			if (!string.IsNullOrWhiteSpace(generateAlicePrivate.Text))
			{
				CopyGeneratedData();
			}
			foreach (TextBox item in inputBoxes)
			{
				if (item.Text.Length <= 0)
				{
					item.Background = Brushes.OrangeRed;
					if (inputBoxes.IndexOf(item) == inputBoxes.Count - 1)
					{
						return;
					}
				}
				else
				{
					item.Background = Brushes.White;
				}
			}

			//set default values if input is empty
			modulo = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;

			int i = 0;
			stopwatch.Start();
			while (gmp_lib.mpz_cmp_ui(modulo, exponent) >= 0)
			{
				Versuche++;
				exponent++;

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
			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice, modulo);
			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob, modulo);

			long time = stopwatch.ElapsedMilliseconds;
			stopwatch.Stop();

			ausgabeTopR.Text = secretKeyAlice.ToString();
			ausgabeTop1R.Text = secretKeyBob.ToString();
			ausgabeBottomR.Text = sharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = Versuche.ToString();
			ZeitAusgabe.Text = time.ToString() + " ms";
		}

		private bool CheckInputEmpty()
		{
			int errors = 0;
			foreach (TextBox item in inputBoxes)
			{
				if (string.IsNullOrWhiteSpace(item.Text) || !item.Text.All(char.IsDigit))
				{
					errors++;
					item.Background = Brushes.Red;
					item.Text = "#VALUE?";
				}
			}
			return errors <= 0;
		}

		private void BtnCreateKey(object sender, RoutedEventArgs e)
		{
			//erstelle öffentlichen Handshake
			gmp_lib.mpz_urandomb(modulo, rnd, BitStandard);
			generatePublicKeyAinput.Text = modulo.ToString();
			do
			{
				gmp_lib.mpz_urandomb(basis, rnd, BitStandardPrime);
				generatePublicKeyBinput.Text = basis.ToString();
			} while (!CheckInputPrime(basis, modulo));

			//erstelle privaten Schlüssel
			gmp_lib.mpz_urandomb(alicePrivate, rnd, 16);
			generateAlicePrivate.Text = alicePrivate.ToString();
			gmp_lib.mpz_urandomb(bobPrivate, rnd, 16);
			generateBobPrivate.Text = bobPrivate.ToString();

			//erstelle exchange keys
			gmp_lib.mpz_powm(ExchangeKeyAlice, basis, alicePrivate, modulo);
			generateExchangeKeyAinput.Text = ExchangeKeyAlice.ToString();
			gmp_lib.mpz_powm(ExchangeKeyBob, basis, bobPrivate, modulo);
			generateExchangeKeyBinput.Text = ExchangeKeyBob.ToString();

			//erstelle die secret shared Schlüssel
			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, alicePrivate, modulo);
			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, bobPrivate, modulo);
			sharedSecretKeyBobBox.Text = sharedSecretKeyBob.ToString();
		}
		private void BtnClearKey(object sender, RoutedEventArgs e)
		{
			foreach (TextBox item in allBoxes)
			{
				item.Clear();
			}
			foreach (TextBox item in inputBoxes)
			{
				item.Background = Brushes.White;
			}
		}
		private void BtnCopyClick(object sender, RoutedEventArgs e)
		{
			if (generatedBoxes[0].Text.Length > 0)
			{
				foreach (var item in inputBoxes)
				{
					item.Background = Brushes.White;
				}
			}
			CopyGeneratedData();
		}
		private void CopyGeneratedData()
		{
			if (generatedBoxes[0].Text.Length > 0)
			{
				for (int i = 0; i < generatedBoxes.Count; i++)
				{
					inputBoxes[i].Text = generatedBoxes[i].Text;
				}
			}
		}
		private bool CheckInputInt()
		{
			int errors = 0;
			foreach (TextBox item in inputBoxes)
			{
				if (string.IsNullOrWhiteSpace(item.Text) || !item.Text.All(char.IsDigit))
				{
					errors++;
					item.Background = Brushes.Red;
					item.Text = "#VALUE?";
				}
			}
			return errors <= 0;
		}
		private bool CheckInputPrime()
		{
			mpz_t var = new mpz_t();
			gmp_lib.mpz_init(var);

			char_ptr str = new char_ptr(inputBoxes[1].Text); 
			gmp_lib.mpz_init_set_str(var, str, 10);
			if (gmp_lib.mpz_probab_prime_p(var, 25) != 2)
			{
				inputBoxes[1].Background = Brushes.Red;
				return false;
			}
			else
			{
				return true;
			}
		}
		private bool CheckInputPrime(mpz_t prime, mpz_t multipleOfPrime)
		{
			mpz_t modulo = new mpz_t();
			char_ptr str = new char_ptr(generatedBoxes[1].Text);

			if (gmp_lib.mpz_probab_prime_p(prime, 25) != 2)
			{
				inputBoxes[1].Background = Brushes.Red;
				return false;
			}

			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init_set_str(prime, str, 10);
			if (gmp_lib.mpz_divisible_p(prime, multipleOfPrime) > 0)
			{
				inputBoxes[1].Background = Brushes.Red;
				return false;
			}
			else
			{
				return true;
			}
		}
	}
}
