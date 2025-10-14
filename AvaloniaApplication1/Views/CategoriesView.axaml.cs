using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class CategoriesView : UserControl
    {
        public CategoriesView()
        {
            InitializeComponent();
            LoadCategories();
        }

        private void LoadCategories()
        {
            CategoriesDataGrid.ItemsSource = App.dbContext.Categories.ToList();
            CategoriesDataGrid.Columns.Clear();
            CategoriesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Название",
                Binding = new Avalonia.Data.Binding("Name"),
          
            });
            CategoriesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Описание",
                Binding = new Avalonia.Data.Binding("Description"),
                
            });
            CategoriesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Кол-во фильмов",
                Binding = new Avalonia.Data.Binding("Movies.Count"),
              
            });
        }

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            ContextData.CurrentCategory = null; // Сбрасываем для создания новой категории
            var categoryEditView = new CategoryEditView();
            var window = new Window
            {
                Content = categoryEditView,
                Title = "Добавление категории",
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadCategories();
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedCategory = CategoriesDataGrid.SelectedItem as Category;
            if (selectedCategory != null)
            {
                // Не позволяем удалить дефолтную категорию
                if (selectedCategory.Name == "Без категории")
                {
                    ShowMessage("Нельзя удалить дефолтную категорию");
                    return;
                }

                // Переносим фильмы в дефолтную категорию
                var defaultCategory = App.dbContext.Categories.FirstOrDefault(c => c.Name == "Без категории");
                if (defaultCategory != null)
                {
                    var moviesInCategory = App.dbContext.Movies.Where(m => m.CategoryId == selectedCategory.Id).ToList();
                    foreach (var movie in moviesInCategory)
                    {
                        movie.CategoryId = defaultCategory.Id;
                    }
                    App.dbContext.SaveChanges();
                }

                App.dbContext.Categories.Remove(selectedCategory);
                App.dbContext.SaveChanges();
                LoadCategories();
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedCategory = CategoriesDataGrid.SelectedItem as Category;
            if (selectedCategory == null) return;

            ContextData.CurrentCategory = selectedCategory;
            var categoryEditView = new CategoryEditView();
            var window = new Window
            {
                Content = categoryEditView,
                Title = "Редактирование категории",
                Width = 400,
                Height = 250,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadCategories();
        }

        private void CloseButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
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

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}