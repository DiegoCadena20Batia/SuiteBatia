using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BatiaSuite.ViewModel
{
   public  class BaseViewModel : INotifyPropertyChanged
    {
     
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public async Task DisplayAlert(string title, string message, string cancel)
            {
                await Application.Current.MainPage.DisplayAlert(title, message, cancel);
            }

            public async Task<bool> DisplayAlert(string title, string message, string accept, string cancel)
            {
                return await Application.Current.MainPage.DisplayAlert(title, message, accept, cancel);
            }

            private bool _isBusy;
            public bool IsBusy
            {
                get { return _isBusy; }
                set
                {
                    SetProperty(ref _isBusy, value);
                }
            }

            protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
            {
                if (EqualityComparer<T>.Default.Equals(field, value))
                {
                    return false;
                }

                field = value;
                OnPropertyChanged(propertyName);

                return true;
            }

        }
    }

