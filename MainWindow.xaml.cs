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
using Diffie_Hellman_Cracker;

namespace Diffie_Hellman_Crack {
	/// <summary>
	/// Interaktionslogik für MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {
		public string Crackzeit { get; set; }
		private ulong Versuche = 0;
		public delegate bool CheckInput();
		public static mp_bitcnt_t BitStandard { get; set; } = new mp_bitcnt_t(32);
		public static mp_bitcnt_t BitStandardPrime { get; set; } = new mp_bitcnt_t(8);
		private readonly Stopwatch stopwatch = new Stopwatch();
		readonly List<TextBox> inputBoxes = new List<TextBox>();
		readonly List<TextBox> generatedBoxes = new List<TextBox>();
		readonly List<TextBox> allBoxes = new List<TextBox>();
		Key key = new Key();
		public MainWindow() {
			InitializeComponent();
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
			gmp_lib.mpz_init(key.Exponent);
			gmp_lib.mpz_add_ui(key.Exponent, key.Exponent, 1);
			await Task.Run(() => {
				while (gmp_lib.mpz_cmp(key.Group, key.Exponent) >= 0) {
					Versuche++;
					//exponent++;
					gmp_lib.mpz_add_ui(key.Exponent, key.Exponent, 1);

					gmp_lib.mpz_powm(key.Result, key.Basis, key.Exponent, key.Group);

					if (gmp_lib.mpz_cmp(key.Result, key.ExchangeAlice) == 0) {
						//exponent wird größer als uint
						gmp_lib.mpz_init_set(key.SecretKeyAlice, key.Exponent);
						i++;
					}
					if (gmp_lib.mpz_cmp(key.Result, key.ExchangeBob) == 0) {
						gmp_lib.mpz_init_set(key.SecretKeyBob, key.Exponent);
						i++;
					}
					if (i >= 2) {
						break;
					}
				}
			});
			gmp_lib.mpz_powm(key.SharedSecretKeyAlice, key.ExchangeBob, key.SecretKeyAlice, key.Group);
			gmp_lib.mpz_powm(key.SharedSecretKeyBob, key.ExchangeAlice, key.SecretKeyBob, key.Group);

			ausgabeTopR.Text = key.SecretKeyAlice.ToString();
			ausgabeTop1R.Text = key.SecretKeyBob.ToString();
			ausgabeBottomR.Text = key.SharedSecretKeyAlice.ToString();
			ausgabeBottomR1.Text = Versuche.ToString();
			ProgressBar.IsIndeterminate = false;
			stopwatch.Stop();
			Crackzeit = stopwatch.ElapsedMilliseconds.ToString() + " ms";
			ZeitAusgabe.Text = Crackzeit;
			stopwatch.Reset();
		}
		private bool SetValues() {
			//bevorzuge immer Wert aus input
			key.Group = publicKeyAinput.Text;
			key.Basis = publicKeyBinput.Text;
			key.ExchangeAlice = exchangeKeyAinput.Text;
			key.ExchangeBob = exchangeKeyBinput.Text;
			return true;
		}
		private void BtnCreateKey(object sender, RoutedEventArgs e) {
			//erstelle öffentlichen Handshake
			gmp_lib.mpz_urandomb(key.Group, key.Rnd, BitStandard);
			generatePublicKeyAinput.Text = key.Group.ToString();
			//garantiere prime und gcd == 1
			do {
				gmp_lib.mpz_urandomb(key.Basis, key.Rnd, BitStandardPrime);
			} while (!(CheckInputPrime(key.Basis, key.Group) && gmp_lib.mpz_cmp(key.Basis, key.Group) < 0));
			generatePublicKeyBinput.Text = key.Basis.ToString();

			//erstelle privaten Schlüssel
			gmp_lib.mpz_urandomm(key.AlicePrivate, key.Rnd, key.Group);
			generateAlicePrivate.Text = key.AlicePrivate.ToString();
			gmp_lib.mpz_urandomm(key.BobPrivate, key.Rnd, key.Group);
			generateBobPrivate.Text = key.BobPrivate.ToString();

			//erstelle exchange keys
			gmp_lib.mpz_powm(key.ExchangeAlice, key.Basis, key.AlicePrivate, key.Group);
			generateExchangeKeyAinput.Text = key.ExchangeAlice.ToString();
			gmp_lib.mpz_powm(key.ExchangeBob, key.Basis, key.BobPrivate, key.Group);
			generateExchangeKeyBinput.Text = key.ExchangeBob.ToString();

			//erstelle die secret shared Schlüssel
			gmp_lib.mpz_powm(key.SharedSecretKeyAlice, key.ExchangeBob, key.AlicePrivate, key.Group);
			sharedSecretKeyAliceBox.Text = key.SharedSecretKeyAlice.ToString();
			gmp_lib.mpz_powm(key.SharedSecretKeyAlice, key.ExchangeAlice, key.BobPrivate, key.Group);
			sharedSecretKeyBobBox.Text = key.SharedSecretKeyAlice.ToString();
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
			if (gmp_lib.mpz_probab_prime_p(key.Basis, 25) != 2) {
				inputBoxes[1].Background = Brushes.Red;
				ErrorMessageBox("Number is not prime!", "Public key Bob");
				return false;
			}
			else {
				return true;
			}
		}
		private bool CheckInputPrime(mpz_t Basis, mpz_t Group) {
			gmp_lib.mpz_gcdext(key.Gcd, key.Ext_gcd_s, key.Ext_gcd_t, Basis, Group);
			return gmp_lib.mpz_probab_prime_p(Basis, 25) == 2 && gmp_lib.mpz_divisible_p(Basis, Group) == 0 && gmp_lib.mpz_cmp_ui(key.Gcd, 1) == 0;
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
			fe.Show();

			string path = Path.Combine(Directory.GetCurrentDirectory(), "Json");
			string[] files = Directory.GetFiles(path);

			List<Key> testkeys = new List<Key>();
			for (int i = 0; i < files.Length; i++) {
				string deserialize = File.ReadAllText(files[i]);
				Key item = JsonConvert.DeserializeObject<Key>(deserialize);
				testkeys.Add(item);
			}
			fe.fileNames.ItemsSource = testkeys;
			//InfoMessageBox("Data imported", "Data import");
		}
	}
}
