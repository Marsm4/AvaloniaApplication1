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
            InitializeControls();
        }

        private void InitializeControls()
        {
            // Инициализация элементов управления
            LoadCategories();
            LoadMovies();
        }

        private void LoadCategories()
        {
            try
            {
                // Проверяем, что элемент управления существует
                if (cbCategoryFilter == null)
                {
                    Console.WriteLine("cbCategoryFilter is null");
                    return;
                }

                var categories = App.dbContext.Categories.ToList();

                var allCategories = new Category { Id = 0, Name = "Все категории" };
                var categoriesList = categories.Prepend(allCategories).ToList();

                cbCategoryFilter.ItemsSource = categoriesList;
                cbCategoryFilter.SelectedItem = allCategories;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading categories: {ex.Message}");
            }
        }

        private void LoadMovies()
        {
            try
            {
                // Проверяем, что элементы управления существуют
                if (cbCategoryFilter == null || cbSortBy == null || MoviesDataGrid == null)
                {
                    Console.WriteLine("One or more controls are null");
                    return;
                }

                var movies = App.dbContext.Movies.ToList();

                // Применяем фильтрацию по категории
                var selectedCategory = cbCategoryFilter.SelectedItem as Category;
                if (selectedCategory != null && selectedCategory.Id != 0)
                {
                    movies = movies.Where(m => m.CategoryId == selectedCategory.Id).ToList();
                }

                // Применяем сортировку
                var sortBy = cbSortBy.SelectedIndex;
                movies = sortBy switch
                {
                    0 => movies.OrderBy(m => m.Title).ToList(),
                    1 => movies.OrderByDescending(m => m.Title).ToList(),
                    2 => movies.OrderBy(m => m.Genre).ThenBy(m => m.Title).ToList(),
                    3 => movies.OrderBy(m => m.Director).ThenBy(m => m.Title).ToList(),
                    4 => movies.OrderBy(m => m.Category != null ? m.Category.Name : "").ThenBy(m => m.Title).ToList(),
                    _ => movies.OrderBy(m => m.Title).ToList()
                };

                MoviesDataGrid.ItemsSource = movies;
                MoviesDataGrid.Columns.Clear();
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Название",
                    Binding = new Avalonia.Data.Binding("Title"),
                
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Жанр",
                    Binding = new Avalonia.Data.Binding("Genre"),
                
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Режиссер",
                    Binding = new Avalonia.Data.Binding("Director"),
              
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "Категория",
                    Binding = new Avalonia.Data.Binding("Category.Name"),
               
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading movies: {ex.Message}");
            }
        }

        private void CategoryFilter_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (cbCategoryFilter != null)
            {
                LoadMovies();
            }
        }

        private void SortBy_SelectionChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (cbSortBy != null)
            {
                LoadMovies();
            }
        }

        private void ResetFilters_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (cbCategoryFilter == null || cbSortBy == null) return;

                var allCategories = cbCategoryFilter.ItemsSource?.OfType<Category>().FirstOrDefault(c => c.Id == 0);
                if (allCategories != null)
                {
                    cbCategoryFilter.SelectedItem = allCategories;
                }
                cbSortBy.SelectedIndex = 0;
                LoadMovies();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error resetting filters: {ex.Message}");
            }
        }

        private void AddToBasket_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (MoviesDataGrid == null) return;

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

                using var context = new AppDbContext();
                var existingBasketItem = context.Baskets
                    .FirstOrDefault(b => b.UserId == currentUser.Id && b.MovieId == selectedMovie.Id);

                if (existingBasketItem != null)
                {
                    existingBasketItem.Quantity++;
                    context.Baskets.Update(existingBasketItem);
                }
                else
                {
                    var newBasketItem = new Basket()
                    {
                        UserId = currentUser.Id,
                        MovieId = selectedMovie.Id,
                        Quantity = 1
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

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                var movieEditView = new MovieEditView();
                var window = new Window
                {
                    Content = movieEditView,
                    Title = "Добавление фильма",
                    Width = 400,
                    Height = 350,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await window.ShowDialog((Window)this.VisualRoot);
                LoadMovies();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при открытии формы добавления: {ex.Message}");
            }
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            try
            {
                if (MoviesDataGrid == null) return;

                var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
                if (selectedMovie != null)
                {
                    App.dbContext.Movies.Remove(selectedMovie);
                    App.dbContext.SaveChanges();
                    LoadMovies();
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при удалении фильма: {ex.Message}");
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            try
            {
                if (MoviesDataGrid == null) return;

                var selectedMovie = MoviesDataGrid.SelectedItem as Movie;
                if (selectedMovie == null) return;

                ContextData.CurrentMovie = selectedMovie;
                var movieEditView = new MovieEditView();
                var window = new Window
                {
                    Content = movieEditView,
                    Title = "Редактирование фильма",
                    Width = 400,
                    Height = 350,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await window.ShowDialog((Window)this.VisualRoot);
                LoadMovies();
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка при редактировании фильма: {ex.Message}");
            }
        }

        private void ShowMessage(string message)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"Error showing message: {ex.Message}");
            }
        }
    }
}