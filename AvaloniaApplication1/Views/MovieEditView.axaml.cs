using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1.Views
{
    public partial class MovieEditView : UserControl
    {
        private Movie _movie;
        private bool _isEditMode;

        public MovieEditView()
        {
            InitializeComponent();

            if (ContextData.CurrentMovie != null)
            {
                _movie = ContextData.CurrentMovie;
                _isEditMode = true;
                tbTitle.Text = _movie.Title;
                tbGenre.Text = _movie.Genre;
                tbDirector.Text = _movie.Director;
            }
            else
            {
                _isEditMode = false;
            }
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_isEditMode && _movie != null)
            {
                _movie.Title = tbTitle.Text;
                _movie.Genre = tbGenre.Text;
                _movie.Director = tbDirector.Text;
                App.dbContext.Movies.Update(_movie);
            }
            else
            {
                var newMovie = new Movie()
                {
                    Title = tbTitle.Text,
                    Genre = tbGenre.Text,
                    Director = tbDirector.Text,
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