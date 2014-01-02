using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using System.Xml;
using System.Xml.Serialization;

namespace Requireris
{
    /// <summary>
    /// Interaction logic for AddAccountUserControl.xaml
    /// </summary>
    public partial class AddAccountUserControl : UserControl
    {
        static readonly string FILE = "otp.requireris";
        List<MyKeyValuePair<string, string>> _dic;
        ObservableCollection<string> _listAccounts;

        public AddAccountUserControl(MainUserControl main)
        {
            _dic = new List<MyKeyValuePair<string, string>>();
            _listAccounts = new ObservableCollection<string>();
            main.MyListView.ItemsSource = _listAccounts;
            InitializeComponent();

            try
            {
                using (FileStream stream = new FileStream(FILE, FileMode.OpenOrCreate))
                {
                    TextReader reader = new StreamReader(stream);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));

                    _dic = serializer.Deserialize(reader) as List<MyKeyValuePair<string, string>>;
                    foreach (MyKeyValuePair<string, string> cur in _dic)
                    {
                        Console.WriteLine("Email=" + cur.Key + " Secret=" + cur.Value);
                        _listAccounts.Add(cur.Key);
                    }
                }
            }
            catch (Exception)
            {
                File.Delete(FILE);
            }
        }
        
        private void AddAcountEvent(object sender, RoutedEventArgs e)
        {
            string secret = SecretTextBox.Text.Replace(" ", "");

            if (!Regex.IsMatch(MailTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$") || _dic.Exists(a => a.Key == MailTextBox.Text))
            {
                InvalidMail.Visibility = System.Windows.Visibility.Visible;
                if (secret.Length != 32)
                {
                    InvalidSecret.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else if (secret.Length != 32)
            {
                InvalidSecret.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                string encrypt = Base32.Base32Encoder.Encode(MyCrypt.Protect(Encoding.ASCII.GetBytes(secret)));

                _dic.Add(new MyKeyValuePair<string, string>(MailTextBox.Text, encrypt));
                _listAccounts.Add(MailTextBox.Text);
                MailTextBox.Text = "";
                SecretTextBox.Text = "";
                if (!File.Exists(FILE))
                {
                    using (FileStream stream = File.Create(FILE))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));

                        serializer.Serialize(stream, _dic);
                    }
                }
                else
                {
                    using (FileStream stream = new FileStream(FILE, FileMode.Truncate))
                    {
                        XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));
                        
                        serializer.Serialize(stream, _dic);
                    };
                }
            }
        }

        private void Form_GotFocus(object sender, RoutedEventArgs e)
        {
            InvalidMail.Visibility = System.Windows.Visibility.Collapsed;
            InvalidSecret.Visibility = System.Windows.Visibility.Collapsed;
        }

        public void DeleteAccount(string mail)
        {
            MyKeyValuePair<string, string> at = _dic.Where(d => d.Key == mail).FirstOrDefault();

            _dic.Remove(at);
            _listAccounts.Remove(mail);
            using (FileStream stream = new FileStream(FILE, FileMode.Truncate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));

                serializer.Serialize(stream, _dic);
            };
        }

        public string GetSecret(string mail)
        {
            Console.WriteLine("MAIL=" + mail);
            try
            {
                return Encoding.Default.GetString(MyCrypt.UnProtect(Base32.Base32Encoder.Decode(_dic.Where(d => d.Key == mail).FirstOrDefault().Value)));
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void SetSecret(string mail, string secret)
        {
            string encrypt = Base32.Base32Encoder.Encode(MyCrypt.Protect(Encoding.ASCII.GetBytes(secret)));

            _dic.Where(a => a.Key == mail).FirstOrDefault().Value = encrypt;
            using (FileStream stream = new FileStream(FILE, FileMode.Truncate))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));

                serializer.Serialize(stream, _dic);
            };
        }
    }
}
