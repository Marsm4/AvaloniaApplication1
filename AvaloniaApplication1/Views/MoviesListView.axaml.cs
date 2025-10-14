using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class MoviesListView : UserControl
    {
        public MoviesListView()
        {
            InitializeComponent();
            LoadMovies();
        }

        private void LoadMovies()
        {
            MoviesDataGrid.ItemsSource = App.dbContext.Movies.ToList();
            MoviesDataGrid.Columns.Clear();
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "��������", Binding = new Avalonia.Data.Binding("Title") });
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "����", Binding = new Avalonia.Data.Binding("Genre") });
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "��������", Binding = new Avalonia.Data.Binding("Director") });
        }

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            var movieEditView = new MovieEditView();
            var window = new Window
            {
                Content = movieEditView,
                Title = "���������� ������",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadMovies();
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
            if (selectedMovie != null)
            {
                App.dbContext.Movies.Remove(selectedMovie);
                App.dbContext.SaveChanges();
                LoadMovies();
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
            if (selectedMovie == null) return;

            ContextData.CurrentMovie = selectedMovie;
            var movieEditView = new MovieEditView();
            var window = new Window
            {
                Content = movieEditView,
                Title = "�������������� ������",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadMovies();
        }
    }
}