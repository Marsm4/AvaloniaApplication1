using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1
{
    public partial class AddAndChangeCategory : Window
    {
        Category category;

        public AddAndChangeCategory()
        {
            InitializeComponent();
            category = new Category();
        }

        public AddAndChangeCategory(Category category)
        {
            InitializeComponent();
            this.category = category;
            tbName.Text = category.Name;
            tbDescription.Text = category.Description;
        }

        private void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            category.Name = tbName.Text;
            category.Description = tbDescription.Text;

            if (category.Id == 0)
            {
                App.dbContext.Categories.Add(category);
            }
            else
            {
                App.dbContext.Update(category);
            }
            App.dbContext.SaveChanges();
            this.Close();
        }
    }
}