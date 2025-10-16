using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class AddAndChangeMovie : Window
    {
        Movie movie;

        public AddAndChangeMovie()
        {
            InitializeComponent();
            movie = new Movie();
            LoadCategories();
        }

        public AddAndChangeMovie(Movie movie)
        {
            InitializeComponent();
            this.movie = movie;
            tbTitle.Text = movie.Title;
            tbGenre.Text = movie.Genre;
            tbDirector.Text = movie.Director;
            LoadCategories();

            if (movie.CategoryId.HasValue)
            {
                var selectedCategory = App.dbContext.Categories
                    .FirstOrDefault(c => c.Id == movie.CategoryId.Value);
                if (selectedCategory != null)
                {
                    cbCategory.SelectedItem = selectedCategory;
                }
            }
        }

        private void LoadCategories()
        {
            var categories = App.dbContext.Categories.ToList();
            cbCategory.ItemsSource = categories;
        }

        private void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            movie.Title = tbTitle.Text;
            movie.Genre = tbGenre.Text;
            movie.Director = tbDirector.Text;
            movie.CategoryId = (cbCategory.SelectedItem as Category)?.Id;

            if (movie.Id == 0)
            {
                App.dbContext.Movies.Add(movie);
            }
            else
            {
                App.dbContext.Update(movie);
            }
            App.dbContext.SaveChanges();
            this.Close();
        }
    }
}