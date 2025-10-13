using System.Linq;
using Avalonia.Controls;
using AvaloniaApplication1.Data;
using Microsoft.EntityFrameworkCore;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        private enum CurrentView { Users, Movies }
        private CurrentView _currentView = CurrentView.Users;

        public MainWindow()
        {
            InitializeComponent();
            ShowUsers();
        }

        private void ShowUsers_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowUsers();
        }

        private void ShowMovies_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            ShowMovies();
        }

        private void ShowUsers()
        {
            _currentView = CurrentView.Users;
            MainDataGrid.ItemsSource = App.dbContext.Users.ToList();

            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Full Name", Binding = new Avalonia.Data.Binding("Name") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Password", Binding = new Avalonia.Data.Binding("Password") });
        }

        private void ShowMovies()
        {
            _currentView = CurrentView.Movies;
            MainDataGrid.ItemsSource = App.dbContext.Movies.ToList();


            MainDataGrid.Columns.Clear();
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Title", Binding = new Avalonia.Data.Binding("Title") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Genre", Binding = new Avalonia.Data.Binding("Genre") });
            MainDataGrid.Columns.Add(new DataGridTextColumn { Header = "Director", Binding = new Avalonia.Data.Binding("Director") });
        }

        private async void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_currentView == CurrentView.Users)
            {
                var userWindow = new UserInfo();
                await userWindow.ShowDialog(this);
                ShowUsers();
            }
            else
            {
                var movieWindow = new MovieInfo();
                await movieWindow.ShowDialog(this);
                ShowMovies();
            }
        }

        private void DeleteButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_currentView == CurrentView.Users)
            {
                var selectedUser = MainDataGrid.SelectedItem as User;
                if (selectedUser != null)
                {
                    App.dbContext.Users.Remove(selectedUser);
                    App.dbContext.SaveChanges();
                    ShowUsers();
                }
            }
            else
            {
                var selectedMovie = MainDataGrid.SelectedItem as Movie;
                if (selectedMovie != null)
                {
                    App.dbContext.Movies.Remove(selectedMovie);
                    App.dbContext.SaveChanges();
                    ShowMovies();
                }
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            if (_currentView == CurrentView.Users)
            {
                var selectedUser = MainDataGrid.SelectedItem as User;
                if (selectedUser == null) return;

                ContextData.CurrentUser = selectedUser;
                var profile = new Profile();
                await profile.ShowDialog(this);
                ShowUsers();
            }
            else
            {
                var selectedMovie = MainDataGrid.SelectedItem as Movie;
                if (selectedMovie == null) return;

                ContextData.CurrentMovie = selectedMovie;
                var movieEdit = new MovieEdit();
                await movieEdit.ShowDialog(this);
                ShowMovies();
            }
        }
    }
}