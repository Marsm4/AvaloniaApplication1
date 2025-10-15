using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
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
                Header = "Name",
                Binding = new Avalonia.Data.Binding("Name")
            });
            CategoriesDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "Description",
                Binding = new Avalonia.Data.Binding("Description")
            });
        }

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            var categoryEditView = new CategoryEditView();
            var window = new Window
            {
                Content = categoryEditView,
                Title = "New Category",
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
                App.dbContext.Categories.Remove(selectedCategory);
                App.dbContext.SaveChanges();
                LoadCategories();
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, TappedEventArgs e)
        {
            var selectedCategory = CategoriesDataGrid.SelectedItem as Category;
            if (selectedCategory == null) return;

            ContextData.CurrentCategory = selectedCategory;
            var categoryEditView = new CategoryEditView();
            var window = new Window
            {
                Content = categoryEditView,
                Title = "Edit Category",
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

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}