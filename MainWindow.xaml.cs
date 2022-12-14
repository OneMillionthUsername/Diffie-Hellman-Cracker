using Math.Gmp.Native;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Newtonsoft.Json;

namespace Diffie_Hellman_Cracker {
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public static mp_bitcnt_t BitStandard { get; set; } = new mp_bitcnt_t(32);
		public static mp_bitcnt_t BitStandardPrime { get; set; } = new mp_bitcnt_t(8);
		public string Crackzeit { get; set; }

		#region DECLARATION
		public delegate bool CheckInput();
		readonly List<TextBox> inputBoxes = new List<TextBox>();
		readonly List<TextBox> generatedBoxes = new List<TextBox>();
		readonly List<TextBox> allBoxes = new List<TextBox>();
		readonly List<Key> testkeys = new List<Key>();
		private ulong Versuche = 0;
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
		private readonly Stopwatch stopwatch = new Stopwatch();
		#endregion
		public MainWindow() {
			InitializeComponent();
			//CheckInputInRealTime();
			#region INIT VARS
			gmp_lib.mpz_init(exponent);
			gmp_lib.gmp_randinit_mt(rnd);
			gmp_lib.gmp_randseed_ui(rnd, (uint)DateTime.UtcNow.Second);
			gmp_lib.mpz_init(alicePrivate);
			gmp_lib.mpz_init(bobPrivate);
			gmp_lib.mpz_init(group);
			gmp_lib.mpz_init(basis);
			gmp_lib.mpz_init(ExchangeKeyAlice);
			gmp_lib.mpz_init(ExchangeKeyBob);
			gmp_lib.mpz_init(sharedSecretKeyAlice);
			gmp_lib.mpz_init(sharedSecretKeyBob);
			gmp_lib.mpz_init(secretKeyAlice);
			gmp_lib.mpz_init(secretKeyBob);
			gmp_lib.mpz_init(result);
			gmp_lib.mpz_init(gcd);
			gmp_lib.mpz_init(ext_gcd_s);
			gmp_lib.mpz_init(ext_gcd_t);
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
		~MainWindow() {
			gmp_lib.gmp_randclear(rnd);
			gmp_lib.mpz_clears(group, basis, alicePrivate, bobPrivate, sharedSecretKeyAlice, sharedSecretKeyBob, ExchangeKeyAlice, ExchangeKeyBob, secretKeyBob, secretKeyAlice, result);
		}
		private async void BtnCrackKey(object sender, RoutedEventArgs e) {
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
			//exponent = 1;
			stopwatch.Start();
			ProgressBar.IsIndeterminate = true;

			//EXPONENT RESETEN und auf 1 setzen
			gmp_lib.mpz_init(exponent);
			gmp_lib.mpz_add_ui(exponent, exponent, 1);
			await Task.Run(() => {
				while (gmp_lib.mpz_cmp(group, exponent) >= 0) {
					Versuche++;
					//exponent++;
					gmp_lib.mpz_add_ui(exponent, exponent, 1);

					gmp_lib.mpz_powm(result, basis, exponent, group);

					if (gmp_lib.mpz_cmp(result, ExchangeKeyAlice) == 0) {
						//exponent wird größer als uint
						gmp_lib.mpz_init_set(secretKeyAlice, exponent);
						i++;
					}
					if (gmp_lib.mpz_cmp(result, ExchangeKeyBob) == 0) {
						gmp_lib.mpz_init_set(secretKeyBob, exponent);
						i++;
					}
					if (i >= 2) {
						break;
					}
				}
			});
			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, secretKeyAlice, group);
			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, secretKeyBob, group);

			ausgabeTopR.Text = secretKeyAlice.ToString();
			ausgabeTop1R.Text = secretKeyBob.ToString();
			ausgabeBottomR.Text = sharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = Versuche.ToString();
			ProgressBar.IsIndeterminate = false;
			stopwatch.Stop();
			ZeitAusgabe.Text = stopwatch.ElapsedMilliseconds.ToString() + " ms";
			stopwatch.Reset();
		}
		private bool SetValues() {
			//bevorzuge immer Wert aus input
			group = publicKeyAinput.Text;
			basis = publicKeyBinput.Text;
			ExchangeKeyAlice = exchangeKeyAinput.Text;
			ExchangeKeyBob = exchangeKeyBinput.Text;
			return true;
		}
		private void BtnCreateKey(object sender, RoutedEventArgs e) {
			//erstelle öffentlichen Handshake
			gmp_lib.mpz_urandomb(group, rnd, BitStandard);
			generatePublicKeyAinput.Text = group.ToString();
			//garantiere prime und gcd == 1
			do {
				gmp_lib.mpz_urandomb(basis, rnd, BitStandardPrime);
			} while (!(CheckInputPrime(basis, group) && gmp_lib.mpz_cmp(basis, group) < 0));
			generatePublicKeyBinput.Text = basis.ToString();

			//erstelle privaten Schlüssel
			gmp_lib.mpz_urandomm(alicePrivate, rnd, group);
			generateAlicePrivate.Text = alicePrivate.ToString();
			gmp_lib.mpz_urandomm(bobPrivate, rnd, group);
			generateBobPrivate.Text = bobPrivate.ToString();

			//erstelle exchange keys
			gmp_lib.mpz_powm(ExchangeKeyAlice, basis, alicePrivate, group);
			generateExchangeKeyAinput.Text = ExchangeKeyAlice.ToString();
			gmp_lib.mpz_powm(ExchangeKeyBob, basis, bobPrivate, group);
			generateExchangeKeyBinput.Text = ExchangeKeyBob.ToString();

			//erstelle die secret shared Schlüssel
			gmp_lib.mpz_powm(sharedSecretKeyAlice, ExchangeKeyBob, alicePrivate, group);
			sharedSecretKeyAliceBox.Text = sharedSecretKeyAlice.ToString();
			gmp_lib.mpz_powm(sharedSecretKeyBob, ExchangeKeyAlice, bobPrivate, group);
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
				ErrorMessageBox("Number is not prime!", "Public key Bob");
				return false;
			}
			else {
				return true;
			}
		}
		private bool CheckInputPrime(mpz_t basis, mpz_t group) {
			gmp_lib.mpz_gcdext(gcd, ext_gcd_s, ext_gcd_t, basis, group);
			return gmp_lib.mpz_probab_prime_p(basis, 25) == 2 && gmp_lib.mpz_divisible_p(basis, group) == 0 && gmp_lib.mpz_cmp_ui(gcd, 1) == 0;
		}
		private void ErrorMessageBox(string errMessage, string errCaption) {
			string messageBoxText = errMessage;
			string caption = errCaption;
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Error;
			MessageBoxResult result;

			result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
		}
		private void InfoMessageBox(string errMessage, string errCaption) {
			string messageBoxText = errMessage;
			string caption = errCaption;
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Information;
			MessageBoxResult result;

			result = MessageBox.Show(messageBoxText, caption, button, icon, MessageBoxResult.Yes);
		}
		private void OptEncProtocol_Click(object sender, RoutedEventArgs e) {
			OptEncryptionProtocol opt = new OptEncryptionProtocol();
			opt.Show();
		}
		private void OptFileOpenWrite_Click(object sender, RoutedEventArgs e) {
			string path = Directory.GetCurrentDirectory();
			if (!Directory.Exists(Path.Combine(path, "Json"))) {
				_ = Directory.CreateDirectory(Path.Combine(path, "Json"));
			}
			path = Path.Combine(path, "Json");

			if (!File.Exists(Path.Combine(path, generateAlicePrivate.Text + ".json"))) {
				FileStream fs = File.Create(Path.Combine(path, generateAlicePrivate.Text + ".json"));

				Key tk = new Key(
					generatePublicKeyAinput.Text,
					generatePublicKeyBinput.Text,
					generateExchangeKeyAinput.Text,
					generateExchangeKeyBinput.Text,
					generateBobPrivate.Text,
					generateAlicePrivate.Text,
					sharedSecretKeyBobBox.Text,
					sharedSecretKeyAliceBox.Text
					);
				string json = JsonConvert.SerializeObject(tk);
				fs.Flush();
				fs.Close();
				File.WriteAllText(Path.Combine(path, generateAlicePrivate.Text + ".json"), json);
				InfoMessageBox("File created", "File creation");
			}
			else {
				ErrorMessageBox("File exists!", "File creation error");
			}
		}
		private void OptFileOpenRead_Click(object sender, RoutedEventArgs e) {
			File_Explorer fe = new File_Explorer();
			string path = Path.Combine(Directory.GetCurrentDirectory(), "Json");
			string[] files = Directory.GetFiles(path);
			for (int i = 0; i < files.Length; i++) {
				string deserialize = File.ReadAllText(files[i]);
				Key item = JsonConvert.DeserializeObject<Key>(deserialize);
				testkeys.Add(item);
			}
			fe.fileNames.ItemsSource = testkeys;
			fe.Show();
		}

	}
}
