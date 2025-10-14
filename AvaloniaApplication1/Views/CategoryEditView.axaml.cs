using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Xml.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class CategoryEditView : UserControl
    {
        private Category _category;
        private bool _isEditMode;

        public CategoryEditView()
        {
            InitializeComponent();

            if (ContextData.CurrentCategory != null)
            {
                _category = ContextData.CurrentCategory;
                _isEditMode = true;
                tbName.Text = _category.Name;
                tbDescription.Text = _category.Description ?? string.Empty;
            }
            else
            {
                _isEditMode = false;
            }

        }

  
        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tbName.Text))
            {
                ShowMessage("Название категории не может быть пустым");
                return;
            }

            if (_isEditMode && _category != null)
            {
                _category.Name = tbName.Text;
                _category.Description = tbDescription.Text;
                App.dbContext.Categories.Update(_category);
            }
            else
            {
                var newCategory = new Category()
                {
                    Name = tbName.Text,
                    Description = tbDescription.Text
                };
                App.dbContext.Categories.Add(newCategory);
            }

            App.dbContext.SaveChanges();
            CloseWindow();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
        }

        private void ShowMessage(string message)
        {
            var messageBox = new Window
            {
                Title = "Ошибка",
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