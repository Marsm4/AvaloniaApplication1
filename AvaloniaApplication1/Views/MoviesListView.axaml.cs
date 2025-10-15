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
            try
            {
                if (MoviesDataGrid == null) return;

                var movies = App.dbContext.Movies.ToList();
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
         
                Console.WriteLine("������");
            
        }
    }
}
