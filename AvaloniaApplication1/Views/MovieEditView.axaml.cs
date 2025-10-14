using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class MovieEditView : UserControl
    {
        private Movie _movie;
        private bool _isEditMode;

        public MovieEditView()
        {
            InitializeComponent();
            LoadCategories();

            if (ContextData.CurrentMovie != null)
            {
                _movie = ContextData.CurrentMovie;
                _isEditMode = true;
                tbTitle.Text = _movie.Title;
                tbGenre.Text = _movie.Genre;
                tbDirector.Text = _movie.Director;

                // Устанавливаем выбранную категорию
                if (_movie.CategoryId.HasValue)
                {
                    var categories = App.dbContext.Categories.ToList();
                    var selectedCategory = categories.FirstOrDefault(c => c.Id == _movie.CategoryId.Value);
                    if (selectedCategory != null)
                    {
                        cbCategory.SelectedItem = selectedCategory;
                    }
                }
            }
            else
            {
                _isEditMode = false;
                // Устанавливаем категорию "Без категории" по умолчанию
                var defaultCategory = App.dbContext.Categories.FirstOrDefault(c => c.Name == "Без категории");
                if (defaultCategory != null)
                {
                    cbCategory.SelectedItem = defaultCategory;
                }
            }
        }

        private void LoadCategories()
        {
            var categories = App.dbContext.Categories.ToList();
            cbCategory.ItemsSource = categories;

            if (!categories.Any())
            {
                // Создаем дефолтную категорию если нет категорий
                var defaultCategory = new Category { Name = "Без категории", Description = "Фильмы без категории" };
                App.dbContext.Categories.Add(defaultCategory);
                App.dbContext.SaveChanges();
                cbCategory.ItemsSource = App.dbContext.Categories.ToList();
            }
        }

        private async void ManageCategories_Click(object? sender, RoutedEventArgs e)
        {
            var categoriesView = new CategoriesView();
            var window = new Window
            {
                Content = categoriesView,
                Title = "Управление категориями",
                Width = 500,
                Height = 400,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadCategories(); // Перезагружаем категории после закрытия окна управления
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_isEditMode && _movie != null)
            {
                _movie.Title = tbTitle.Text;
                _movie.Genre = tbGenre.Text;
                _movie.Director = tbDirector.Text;
                _movie.CategoryId = (cbCategory.SelectedItem as Category)?.Id;
                App.dbContext.Movies.Update(_movie);
            }
            else
            {
                var newMovie = new Movie()
                {
                    Title = tbTitle.Text,
                    Genre = tbGenre.Text,
                    Director = tbDirector.Text,
                    CategoryId = (cbCategory.SelectedItem as Category)?.Id
                };
                App.dbContext.Movies.Add(newMovie);
            }

            App.dbContext.SaveChanges();
            CloseWindow();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}