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

namespace LANChat
{
    /// <summary>
    /// Logika interakcji dla klasy Menu.xaml
    /// </summary>
    public partial class Menu : Page
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void GoToServer(object sender, RoutedEventArgs e)
        {
            ServerPage page = new ServerPage();
            this.NavigationService.Navigate(page);
        }

        private void GoToClient(object sender, RoutedEventArgs e)
        {
            ClientPage page = new ClientPage();
            this.NavigationService.Navigate(page);
        }
    }
}
