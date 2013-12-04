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
using System.Collections.ObjectModel;

namespace Requireris
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MainUserControl                 _main;
        AddAccountUserControl           _account;

        public MainWindow()
        {
            _main = new MainUserControl();
            _account = new AddAccountUserControl(_main);
            _main.LoadAccount(_account);
            InitializeComponent();
            ContentControl.Content = _main;
        }

        private void AddAcount_Click(object sender, RoutedEventArgs e)
        {
            UserControl current = ContentControl.Content as UserControl;

            if (current is MainUserControl)
            {
                ImageButton.Source = new BitmapImage(new Uri("back.png", UriKind.Relative));
                ContentControl.Content = _account;
            }
            else
            {
                ImageButton.Source = new BitmapImage(new Uri("add_account.png", UriKind.Relative));
                ContentControl.Content = _main;
            }
        }
    }
}
