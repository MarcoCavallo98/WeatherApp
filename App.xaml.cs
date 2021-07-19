using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Loading start;
        LoadingViewModel context;
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            
            start = new Loading();
            context = (LoadingViewModel)start.DataContext;
            context.PropertyChanged += Loaded;
            start.DataContext = context;
            start.Show();

        }

        private void Loaded(object sender, EventArgs args) 
        {
            if (context.LoadingString.Equals("Done"))
            {
                MainWindow mw = new MainWindow();
                this.MainWindow = mw;
                mw.DataContext = new MainViewModel();
                
                start.Close();
                mw.Show();
            }
        }
        
    }
}
