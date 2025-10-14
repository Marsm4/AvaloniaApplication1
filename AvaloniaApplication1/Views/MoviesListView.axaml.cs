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
            // ������������� ��������� ����������
            LoadCategories();
            LoadMovies();
        }

        private void LoadCategories()
        {
            try
            {
                // ���������, ��� ������� ���������� ����������
                if (cbCategoryFilter == null)
                {
                    Console.WriteLine("cbCategoryFilter is null");
                    return;
                }

                var categories = App.dbContext.Categories.ToList();

                var allCategories = new Category { Id = 0, Name = "��� ���������" };
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
                // ���������, ��� �������� ���������� ����������
                if (cbCategoryFilter == null || cbSortBy == null || MoviesDataGrid == null)
                {
                    Console.WriteLine("One or more controls are null");
                    return;
                }

                var movies = App.dbContext.Movies.ToList();

                // ��������� ���������� �� ���������
                var selectedCategory = cbCategoryFilter.SelectedItem as Category;
                if (selectedCategory != null && selectedCategory.Id != 0)
                {
                    movies = movies.Where(m => m.CategoryId == selectedCategory.Id).ToList();
                }

                // ��������� ����������
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
                    Header = "��������",
                    Binding = new Avalonia.Data.Binding("Title"),
                
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "����",
                    Binding = new Avalonia.Data.Binding("Genre"),
                
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "��������",
                    Binding = new Avalonia.Data.Binding("Director"),
              
                });
                MoviesDataGrid.Columns.Add(new DataGridTextColumn
                {
                    Header = "���������",
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
                    ShowMessage("�������� ����� ��� ���������� � �������");
                    return;
                }

                if (currentUser == null)
                {
                    ShowMessage("���������� ��������������");
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
                ShowMessage($"����� '{selectedMovie.Title}' �������� � �������");
            }
            catch (Exception ex)
            {
                ShowMessage($"������ ��� ���������� � �������: {ex.Message}");
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
                    Title = "���������� ������",
                    Width = 400,
                    Height = 350,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await window.ShowDialog((Window)this.VisualRoot);
                LoadMovies();
            }
            catch (Exception ex)
            {
                ShowMessage($"������ ��� �������� ����� ����������: {ex.Message}");
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
                ShowMessage($"������ ��� �������� ������: {ex.Message}");
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
                    Title = "�������������� ������",
                    Width = 400,
                    Height = 350,
                    WindowStartupLocation = WindowStartupLocation.CenterOwner
                };

                await window.ShowDialog((Window)this.VisualRoot);
                LoadMovies();
            }
            catch (Exception ex)
            {
                ShowMessage($"������ ��� �������������� ������: {ex.Message}");
            }
        }

        private void ShowMessage(string message)
        {
            try
            {
                var messageBox = new Window
                {
                    Title = "����������",
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