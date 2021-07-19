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
using System.Windows.Shapes;

namespace WeatherApp
{
    /// <summary>
    /// Logica di interazione per Loading.xaml
    /// </summary>
    public partial class Loading : Window
    {
        public Loading()
        {
            InitializeComponent();
            LoadingViewModel context = new LoadingViewModel();
            context.ErrorHandler += ErrorHandler;
            this.DataContext = context;
        }

        public void ErrorHandler(object sender, string args) 
        {
            MessageBoxResult msgRes = MessageBox.Show(args, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Application.Current.Shutdown();
        }
    }
}
