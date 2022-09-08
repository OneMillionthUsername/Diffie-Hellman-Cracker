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

		List<TextBox> inputBoxes = new List<TextBox>();
		List<TextBox> generatedBoxes = new List<TextBox>();
		List<TextBox> allBoxes = new List<TextBox>();

		gmp_randstate_t rnd = new gmp_randstate_t();
		public MainWindow()
		{
			InitializeComponent();
			gmp_lib.gmp_randinit_mt(rnd);
			gmp_lib.gmp_randseed_ui(rnd, (uint)DateTime.UtcNow.Second);

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
		}
		~MainWindow()
		{
			gmp_lib.gmp_randclear(rnd);
		}
		private void BtnCrackKey(object sender, RoutedEventArgs e)
		{

			if (generateAlicePrivate.Text.Length > 0)
			{
				CopyPublicData();
			}
			foreach (var item in inputBoxes)
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

			uint exponent = 0;
			ulong versuche = 0;
			mpz_t secretKeyBob = new mpz_t();
			mpz_t secretKeyAlice = new mpz_t();
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
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(modulo);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);

			Stopwatch stopwatch = new Stopwatch();

			modulo = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;

			//set default values if input is empty

			int i = 0;
			stopwatch.Start();
			while (gmp_lib.mpz_cmp_ui(modulo, exponent) >= 0)
			{
				versuche++;
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
			ausgabeBottomR1.Text = versuche.ToString();
			ZeitAusgabe.Text = time.ToString() + " ms";
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

			//erstelle privaten Schlüssel
			gmp_lib.mpz_urandomb(alicePrivate, rnd, 16);
			gmp_lib.mpz_urandomb(bobPrivate, rnd, 16);

			//erstelle exchange keys
			gmp_lib.mpz_powm(exchangeKeyAlice, basis, alicePrivate, modulo);
			gmp_lib.mpz_powm(exchangeKeyBob, basis, bobPrivate, modulo);

			//erstelle die secret shared Schlüssel
			gmp_lib.mpz_powm(sharedSecretKeyAlice, exchangeKeyBob, alicePrivate, modulo);
			gmp_lib.mpz_powm(sharedSecretKeyBob, exchangeKeyAlice, bobPrivate, modulo);

			//output
			publicAliceCopy = generatePublicKeyAinput.Text = modulo.ToString();
			publicBobCopy = generatePublicKeyBinput.Text = basis.ToString();
			generateAlicePrivate.Text = alicePrivate.ToString();
			generateBobPrivate.Text = bobPrivate.ToString();
			exchangeAliceCopy = generateExchangeKeyAinput.Text = exchangeKeyAlice.ToString();
			exchangeBobCopy = generateExchangeKeyBinput.Text = exchangeKeyBob.ToString();
			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			sharedSecretKeyBobBox.Text = sharedSecretKeyBob.ToString();

			//clear random state
		}
		private void BtnClearKey(object sender, RoutedEventArgs e)
		{
			foreach (var item in allBoxes)
			{
				item.Clear();
			}
			foreach (var item in inputBoxes)
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
			CopyPublicData();
		}
		private void CopyPublicData()
		{
			if (generatedBoxes[0].Text.Length > 0)
			{
				for (int i = 0; i < generatedBoxes.Count; i++)
				{
					inputBoxes[i].Text = generatedBoxes[i].Text;
				}
			}
		}
	}
}
