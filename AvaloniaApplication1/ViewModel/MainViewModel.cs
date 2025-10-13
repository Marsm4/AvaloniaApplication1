using AvaloniaApplication1.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication1.ViewModel
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private object _currentView;

        public MainViewModel()
        {
            // Устанавливаем начальную страницу
            CurrentView = new UsersListView(this);
        }

        [RelayCommand]
        public void ShowUsers()
        {
            CurrentView = new UsersListView(this);
        }

        [RelayCommand]
        public void ShowMovies()
        {
            CurrentView = new MoviesListView(this);
        }
    }
}