using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
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

namespace Requireris
{
    /// <summary>
    /// Interaction logic for MainUserControl.xaml
    /// </summary>
    public partial class MainUserControl : UserControl
    {
        AddAccountUserControl _addAccount;

        public MainUserControl()
        {
            InitializeComponent();
        }

        private void Generate_Click(object sender, RoutedEventArgs e)
        {
            ((ListBoxItem)MyListView.ContainerFromElement((Button)sender)).IsSelected = true;
            string secret = _addAccount.GetSecret(MyListView.SelectedItem as string);
            ((ListBoxItem)MyListView.ContainerFromElement((Button)sender)).IsSelected = false;

            Console.WriteLine("SECRET=" + secret);
            byte[] key;
            try
            {
                // Google send you base32 string you need to decode it before using it.
                key = Base32.Base32Encoder.Decode(secret);
            }
            catch (Exception)
            {
                return;
            }

            // T = (Current Unix time - T0) / X (RFC 6238)
            // Here we get the current time, then we substract the unix epoch (1st January 1970) and we divide by the step
            // T0 is unix's epoch it's the default value
            // X is the step here 30 seconds. Like before we use the default value
            byte[] message = BitConverter.GetBytes((Int64)Math.Floor((DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds / 30.0));
            // Why ? Why is it in big endian !!! See 5.2 Description RFC 4226
            message = message.Reverse().ToArray();

            // We use HMAC-SHA-1 algorithm because HOTP is based on this algorithm
            HMACSHA1 hmac = new HMACSHA1(key);
            byte[] hash = hmac.ComputeHash(message);

            // We extract the nibble (quartet) of the last char 
            int nibble = (hash[hash.Length - 1] & 0xf);

            // We extract 4 bytes from byte array and we need to set the first bit to 0
            int truncatedHash = (hash[nibble] & 0x7f) << 24 | (hash[nibble + 1] & 0xff) << 16 | (hash[nibble + 2] & 0xff) << 8 | (hash[nibble + 3] & 0xff);
            
            // We get a 6 digit code
            string code = (truncatedHash % 1000000).ToString();

            // If needed we add '0' if the code doesn't have enough digit
            while (code.Length < 6)
                code = "0" + code;
            Code.Text = code.ToString();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string mail = MyListView.SelectedItem as string;

            if (mail != null)
                _addAccount.DeleteAccount(mail);
        }

        public void LoadAccount(AddAccountUserControl addAccount)
        {
            _addAccount = addAccount;
        }

        private void Modify_Click(object sender, RoutedEventArgs e)
        {
            SecretModify win = new SecretModify();
            string mail = MyListView.SelectedItem as string;

            if (mail != null)
            {
                win.Secret.Text = _addAccount.GetSecret(mail);
                if (win.ShowDialog() == true)
                {
                    if (win.Secret.Text.Length == 32)
                    {
                        _addAccount.SetSecret(mail, win.Secret.Text);
                    }
                }
            }
        }
    }
}
