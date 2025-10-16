using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1
{
    public partial class MoviesPage : UserControl
    {
        public MoviesPage()
        {
            InitializeComponent();
            Refresh();
        }

        private async void btnAddMovie_Click(object? sender, RoutedEventArgs e)
        {
            var parent = this.VisualRoot as Window;
            var add = new AddAndChangeMovie();
            await add.ShowDialog(parent);
            Refresh();
        }

        private async void btnDeleteMovie_Click(object? sender, RoutedEventArgs e)
        {
            var movie = (sender as Button).Tag as Movie;
            var confirmWindow = new ConfirmDialog("Удаление фильма " + movie.Title);

            confirmWindow.Height = 100;
            confirmWindow.Width = 300;

            var parent = this.VisualRoot as Window;
            var result = await confirmWindow.ShowDialog<bool>(parent);

            if (result)
            {
                App.dbContext.Movies.Remove(movie);
                App.dbContext.SaveChanges();
            }
            Refresh();
        }

        private void Refresh()
        {
            dgMovies.ItemsSource = App.dbContext.Movies.ToList().OrderBy(x => x.Id);
        }

        private async void btnEditMovie_Click(object? sender, RoutedEventArgs e)
        {
            var movie = (sender as Button).Tag as Movie;
            if (movie == null) return;

            var editWindow = new AddAndChangeMovie(movie);
            var parent = this.VisualRoot as Window;
            await editWindow.ShowDialog(parent);
            Refresh();
        }

        private void btnAddToBasket_Click(object? sender, RoutedEventArgs e)
        {
            var movie = (sender as Button).Tag as Movie;
            var currentUser = Data.ContextData.CurrentLoggedInUser;

            if (movie == null)
            {
                ShowMessage("Выберите фильм для добавления в корзину");
                return;
            }

            if (currentUser == null)
            {
                ShowMessage("Необходимо авторизоваться");
                return;
            }

            var existingBasketItem = App.dbContext.Baskets
                .FirstOrDefault(b => b.UserId == currentUser.Id && b.MovieId == movie.Id);

            if (existingBasketItem != null)
            {
                existingBasketItem.Quantity++;
                App.dbContext.Baskets.Update(existingBasketItem);
            }
            else
            {
                var newBasketItem = new Basket()
                {
                    UserId = currentUser.Id,
                    MovieId = movie.Id,
                    Quantity = 1
                };
                App.dbContext.Baskets.Add(newBasketItem);
            }

            App.dbContext.SaveChanges();
            ShowMessage($"Фильм '{movie.Title}' добавлен в корзину");
        }

        private void ShowMessage(string message)
        {
            // Реализация показа сообщения
        }
    }
}