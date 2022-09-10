using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WpfApp1 {
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public mp_bitcnt_t BitStandard { get; set; } = new mp_bitcnt_t(32);
		public mp_bitcnt_t BitStandardPrime { get; set; } = new mp_bitcnt_t(16);

		//declartion
		public delegate bool CheckInput();
		readonly List<TextBox> inputBoxes = new List<TextBox>();
		readonly List<TextBox> generatedBoxes = new List<TextBox>();
		readonly List<TextBox> allBoxes = new List<TextBox>();
		private ulong Versuche = 0;
		private uint exponent = 0;
		private readonly gmp_randstate_t rnd = new gmp_randstate_t();
		private mpz_t modulo = new mpz_t();
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
		private readonly Stopwatch stopwatch = new Stopwatch();

		public MainWindow() {
			InitializeComponent();
			//CheckInputInRealTime();
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

		//async checken wann das Literal legal ist.
		//private async Task CheckInputInRealTime() {
		//	MainWindow mainWindow = await CheckInputSyntax();
		//}

		~MainWindow() {
			gmp_lib.gmp_randclear(rnd);
			gmp_lib.mpz_clears(modulo, basis, alicePrivate, bobPrivate, sharedSecretKeyAlice, sharedSecretKeyBob, ExchangeKeyAlice, ExchangeKeyBob, secretKeyBob, secretKeyAlice, result);
		}
		private void BtnCrackKey(object sender, RoutedEventArgs e) {
			CheckInput checkInput = SetValues;
			checkInput += CheckInputSyntax;
			checkInput += CheckInputPrime;
			foreach (CheckInput item in checkInput.GetInvocationList()) {
				if (item.Invoke()) {
					continue;
				}
				else {
					return;
				}
			}
			int i = 0;
			Versuche = 0;
			exponent = 0;
			stopwatch.Start();
			while (gmp_lib.mpz_cmp_ui(modulo, exponent) >= 0) {
				Versuche++;
				exponent++;

				gmp_lib.mpz_powm_ui(result, basis, exponent, modulo);

				if (gmp_lib.mpz_cmp(result, ExchangeKeyAlice) == 0) {
					gmp_lib.mpz_init_set_ui(secretKeyAlice, exponent);
					i++;
				}
				if (gmp_lib.mpz_cmp(result, ExchangeKeyBob) == 0) {
					gmp_lib.mpz_init_set_ui(secretKeyBob, exponent);
					i++;
				}
				if (i >= 2) {
					break;
				}
			}
			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice, modulo);
			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob, modulo);

			stopwatch.Stop();
			ZeitAusgabe.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
			stopwatch.Reset();

			ausgabeTopR.Text = secretKeyAlice.ToString();
			ausgabeTop1R.Text = secretKeyBob.ToString();
			ausgabeBottomR.Text = sharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = Versuche.ToString();
		}
		private bool SetValues() {
			//bevorzuge immer Wert aus input
			modulo = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;
			return true;
		}
		private void BtnCreateKey(object sender, RoutedEventArgs e) {
			//erstelle öffentlichen Handshake
			gmp_lib.mpz_urandomb(modulo, rnd, BitStandard);
			generatePublicKeyAinput.Text = modulo.ToString();
			//garantiere prime und kein Vielfaches
			do {
				gmp_lib.mpz_urandomb(basis, rnd, BitStandardPrime);
			} while (!CheckInputPrime(basis, modulo));
			generatePublicKeyBinput.Text = basis.ToString();

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
		private void BtnClearKey(object sender, RoutedEventArgs e) {
			foreach (TextBox item in allBoxes) {
				item.Clear();
			}
			foreach (TextBox item in inputBoxes) {
				item.Background = Brushes.White;
			}
		}
		private void BtnCopyClick(object sender, RoutedEventArgs e) {
			//kopiere nur wenn etwas generiert wurde
			if (generatedBoxes[0].Text.Length > 0) {
				foreach (TextBox item in inputBoxes) {
					item.Background = Brushes.White;
				}
				CopyGeneratedData();
			}
		}
		private void CopyGeneratedData() {
			int i = 0;
			if (generatedBoxes[0].Text.Length > 0) {
				foreach (TextBox item in inputBoxes) {
					if (item.Text != generatedBoxes[i].Text) {
						item.Text = generatedBoxes[i].Text;
					}
					i++;
				}
			}
		}
		private bool CheckInputSyntax() {
			int errors = 0;
			foreach (TextBox item in inputBoxes) {
				if (string.IsNullOrWhiteSpace(item.Text) || !item.Text.All(char.IsDigit)) {
					errors++;
					item.Background = Brushes.Red;
					item.Text = "#VALUE?";
				}
				else {
					item.Background = Brushes.White;
				}
			}
			if (errors > 0)
				ErrorMessageBox("Syntaxerror!", "Syntax");
			return errors <= 0;
		}
		private bool CheckInputPrime() {
			if (gmp_lib.mpz_probab_prime_p(basis, 25) != 2) {
				inputBoxes[1].Background = Brushes.Red;
				ErrorMessageBox("Number is not prime!", "Number");
				return false;
			}
			else {
				return true;
			}
		}
		private bool CheckInputPrime(mpz_t prime, mpz_t multipleOfPrime) {
			return gmp_lib.mpz_probab_prime_p(prime, 25) == 2 && gmp_lib.mpz_divisible_p(prime, multipleOfPrime) == 0;
		}
		private void ErrorMessageBox(string errMessage, string errCaption) {
			string messageBoxText = errMessage;
			string caption = errCaption;
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Error;
			MessageBoxResult result;

			result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
		}
	}
}
