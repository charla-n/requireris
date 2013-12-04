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
            byte[] key = Base32.Base32Encoder.Decode(secret);
            Console.WriteLine("KEY=" + key);

            string message = Math.Floor(DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalSeconds / 30.0).ToString();
            Console.WriteLine("MESSAGE=" + message);

            HMACSHA1 hmac = new HMACSHA1(key);
            byte[] hash = hmac.ComputeHash(Encoding.ASCII.GetBytes(message.ToCharArray()));
            Console.WriteLine("HASH=" + hash);

            int nibble = (hash[hash.Length - 1] & 0xf);
            Console.WriteLine("NIBBLE=" + nibble);

            byte[] b = new byte[4];

            for (int i = 0; i < 4; i++)
                b[i] = hash[nibble + i];

            int truncatedHash = BitConverter.ToInt32(b, 0);
            //int truncatedHash = (hash[nibble] << 24) + (hash[nibble + 1] << 16) + (hash[nibble + 2] << 8) + hash[nibble + 3];
            Console.WriteLine("TRUNCATED=" + truncatedHash);

            truncatedHash = truncatedHash & 0x7FFFFF;
            Console.WriteLine("TRUNCATED_ZERO_FIRST_BIT=" + truncatedHash);

            string code = (truncatedHash % 1000000).ToString();
            Console.WriteLine("CODE=" + code);

            while (code.Length < 6)
                code = "0" + code;
            Code.Text = code.ToString();
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            string mail = MyListView.SelectedItem as string;

            _addAccount.DeleteAccount(mail);
        }

        public void LoadAccount(AddAccountUserControl addAccount)
        {
            _addAccount = addAccount;
        }
    }
}
