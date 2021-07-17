using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows.Input;
using System.Diagnostics;

namespace WeatherApp
{
    class MainViewModel: INotifyPropertyChanged
    {
        #region fields

        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;
        private ICommand _changePageCommand;

        #endregion

        #region properties
        public IPageViewModel CurrentViewModel 
        {
            get { return _currentPageViewModel; }
            set {
                if (_currentPageViewModel != value)
                    _currentPageViewModel = value;

                OnPropertyChanged("CurrentViewModel");
            }
        }

        public List<IPageViewModel> PageViewModels 
        {
            get 
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels; 
            }
            
        }

        public ICommand ChangePageCommand 
        {
            get 
            {
                if (_changePageCommand == null)
                    _changePageCommand = new ChangeCommand(this);

                return _changePageCommand;
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChanged;

        public MainViewModel() 
        {
            PageViewModels.Add(new HomeViewModel());
            PageViewModels.Add(new LocationsViewModel());

            CurrentViewModel = PageViewModels[0];
        }

        #region methods

        public void OnPropertyChanged(string propertyName) 
        {
            PropertyChangedEventHandler copy = PropertyChanged;
            if (copy != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private class ChangeCommand : ICommand
        {
            public event EventHandler CanExecuteChanged;

            private MainViewModel MainModel;

            public ChangeCommand(MainViewModel vm) 
            {
                MainModel = vm;
            }

            public bool CanExecute(object parameter)
            {
                return true;
            }

            public void Execute(object parameter)
            {
                if (MainModel.PageViewModels != null)
                {
                    IPageViewModel pageType = (IPageViewModel)parameter;
                    foreach (IPageViewModel p in MainModel.PageViewModels)
                        if (pageType.PageName == p.PageName)
                            MainModel.CurrentViewModel = p;
                }
            }
        }
    }
}
