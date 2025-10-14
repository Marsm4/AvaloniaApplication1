// Views/MoviesListView.xaml.cs - обновленная версия
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System;
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
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Название", Binding = new Avalonia.Data.Binding("Title") });
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Жанр", Binding = new Avalonia.Data.Binding("Genre") });
            MoviesDataGrid.Columns.Add(new DataGridTextColumn { Header = "Режиссер", Binding = new Avalonia.Data.Binding("Director") });
        }

        private void AddToBasket_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
                var currentUser = ContextData.CurrentLoggedInUser;

                if (selectedMovie == null)
                {
                    ShowMessage("Выберите фильм для добавления в корзину");
                    return;
                }

                if (currentUser == null)
                {
                    ShowMessage("Необходимо авторизоваться");
                    return;
                }

                // Создаем новый контекст для избежания проблем с отслеживанием
                using var context = new AppDbContext();

                // Проверяем, есть ли уже этот фильм в корзине пользователя
                var existingBasketItem = context.Baskets
                    .FirstOrDefault(b => b.UserId == currentUser.Id && b.MovieId == selectedMovie.Id);

                if (existingBasketItem != null)
                {
                    // Увеличиваем количество, если уже есть в корзине
                    existingBasketItem.Quantity++;
                    context.Baskets.Update(existingBasketItem);
                }
                else
                {
                    // Добавляем новый элемент в корзину
                    var newBasketItem = new Basket()
                    {
                        UserId = currentUser.Id,
                        MovieId = selectedMovie.Id,
                        Quantity = 1,
                        AddedDate = DateTime.UtcNow // Явно указываем UTC время
                    };
                    context.Baskets.Add(newBasketItem);
                }

                context.SaveChanges();
                ShowMessage($"Фильм '{selectedMovie.Title}' добавлен в корзину");
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при добавлении в корзину: {ex.Message}");
            }
        }

        // Остальные методы остаются без изменений...
        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            var movieEditView = new MovieEditView();
            var window = new Window
            {
                Content = movieEditView,
                Title = "Добавление фильма",
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
                Title = "Редактирование фильма",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadMovies();
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Title = "Информация",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            messageBox.ShowDialog((Window)this.VisualRoot);
        }
    }
}