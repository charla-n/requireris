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

            using (FileStream stream = new FileStream(FILE, FileMode.Open))
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
        
        private void AddAcountEvent(object sender, RoutedEventArgs e)
        {
            if (!Regex.IsMatch(MailTextBox.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                InvalidMail.Visibility = System.Windows.Visibility.Visible;
            }
            else if (SecretTextBox.Text.Length != 16)
            {
                InvalidSecret.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                _dic.Add(new MyKeyValuePair<string, string>(MailTextBox.Text, SecretTextBox.Text));
                _listAccounts.Add(MailTextBox.Text);
                MailTextBox.Text = "";
                SecretTextBox.Text = "";
                using (FileStream stream = new FileStream(FILE, FileMode.OpenOrCreate))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(List<MyKeyValuePair<string, string>>));

                    serializer.Serialize(stream, _dic);
                };
            }
        }

        private void Form_GotFocus(object sender, RoutedEventArgs e)
        {
            InvalidMail.Visibility = System.Windows.Visibility.Collapsed;
            InvalidSecret.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
