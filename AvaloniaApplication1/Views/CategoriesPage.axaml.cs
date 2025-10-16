using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1
{
    public partial class CategoriesPage : UserControl
    {
        public CategoriesPage()
        {
            InitializeComponent();
            Refresh();
        }

        private async void btnAddCategory_Click(object? sender, RoutedEventArgs e)
        {
            var parent = this.VisualRoot as Window;
            var add = new AddAndChangeCategory();
            await add.ShowDialog(parent);
            Refresh();
        }

        private async void btnDeleteCategory_Click(object? sender, RoutedEventArgs e)
        {
            var category = (sender as Button).Tag as Category;
            var confirmWindow = new ConfirmDialog("Удаление категории " + category.Name);

            confirmWindow.Height = 100;
            confirmWindow.Width = 300;

            var parent = this.VisualRoot as Window;
            var result = await confirmWindow.ShowDialog<bool>(parent);

            if (result)
            {
                App.dbContext.Categories.Remove(category);
                App.dbContext.SaveChanges();
            }
            Refresh();
        }

        private void Refresh()
        {
            dgCategories.ItemsSource = App.dbContext.Categories.ToList().OrderBy(x => x.Id);
        }

        private async void btnEditCategory_Click(object? sender, RoutedEventArgs e)
        {
            var category = (sender as Button).Tag as Category;
            if (category == null) return;

            var editWindow = new AddAndChangeCategory(category);
            var parent = this.VisualRoot as Window;
            await editWindow.ShowDialog(parent);
            Refresh();
        }
    }
}