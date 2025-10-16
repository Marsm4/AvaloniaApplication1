using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace AvaloniaApplication1
{
    public partial class MoviesPage : UserControl
    {
        private int? _selectedCategoryId = null;
        private bool _isAdmin; //проверка админуки

        public MoviesPage()
        {
            InitializeComponent();

            var currentUser = Data.ContextData.CurrentLoggedInUser;
            _isAdmin = currentUser?.Role == "Admin";
            if (!_isAdmin)
            {
                btnAddMovie.IsEnabled = false;
                ShowMessage("Режим просмотра: доступны только функции добавления в корзину");
            }

            LoadCategories();
            Refresh();
        }

        private void LoadCategories()
        {
            var categories = App.dbContext.Categories.ToList();

            cbCategoryFilter.Items.Clear();
            cbCategoryFilter.Items.Add(new ComboBoxItem { Content = "Все категории" });

            foreach (var category in categories)
            {
                cbCategoryFilter.Items.Add(new ComboBoxItem
                {
                    Content = category.Name,
                    Tag = category.Id
                });
            }

            cbCategoryFilter.SelectedIndex = 0;
        }

        private async void btnAddMovie_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var parent = this.VisualRoot as Window;
            var add = new AddAndChangeMovie();

            var currentFilter = _selectedCategoryId;

            await add.ShowDialog(parent);

            LoadCategories();
            Refresh();

            if (currentFilter.HasValue)
            {
                foreach (ComboBoxItem item in cbCategoryFilter.Items)
                {
                    if (item.Tag is int categoryId && categoryId == currentFilter.Value)
                    {
                        cbCategoryFilter.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private async void btnDeleteMovie_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var movie = (sender as Button).Tag as Movie;
            var box = MessageBoxManager
                .GetMessageBoxStandard("Подтверждение", $"Вы уверены, что хотите удалить фильм '{movie.Title}'?",
                    ButtonEnum.YesNo);

            var result = await box.ShowAsync();

            if (result == ButtonResult.Yes)
            {
                App.dbContext.Movies.Remove(movie);
                App.dbContext.SaveChanges();
                Refresh();
            }
        }

        private void Refresh()
        {
            IQueryable<Movie> query = App.dbContext.Movies;

            if (_selectedCategoryId.HasValue)
            {
                query = query.Where(m => m.CategoryId == _selectedCategoryId.Value);
            }

            var movies = query.ToList();
            dgMovies.ItemsSource = movies.OrderBy(x => x.Id);
        }

        private async void btnEditMovie_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var movie = (sender as Button).Tag as Movie;
            if (movie == null) return;

            var currentFilter = _selectedCategoryId;

            var editWindow = new AddAndChangeMovie(movie);
            var parent = this.VisualRoot as Window;
            await editWindow.ShowDialog(parent);

            LoadCategories();
            Refresh();

            if (currentFilter.HasValue)
            {
                foreach (ComboBoxItem item in cbCategoryFilter.Items)
                {
                    if (item.Tag is int categoryId && categoryId == currentFilter.Value)
                    {
                        cbCategoryFilter.SelectedItem = item;
                        break;
                    }
                }
            }
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

        private void cbCategoryFilter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (cbCategoryFilter.SelectedItem is ComboBoxItem selectedItem)
            {
                if (selectedItem.Content?.ToString() == "Все категории")
                {
                    _selectedCategoryId = null;
                }
                else if (selectedItem.Tag is int categoryId)
                {
                    _selectedCategoryId = categoryId;
                }
            }
            Refresh();
        }

        private void btnResetFilter_Click(object? sender, RoutedEventArgs e)
        {
            _selectedCategoryId = null;
            cbCategoryFilter.SelectedIndex = 0;
            Refresh();
        }

        private void ShowMessage(string message)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Информация", message, ButtonEnum.Ok);
            box.ShowAsync();
        }
    }
}