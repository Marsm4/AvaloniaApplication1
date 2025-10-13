using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1;

public partial class MovieEdit : Window
{
    private Movie _movie;

    public MovieEdit()
    {
        InitializeComponent();

        if (ContextData.CurrentMovie != null)
        {
            _movie = ContextData.CurrentMovie;
            tbTitle.Text = _movie.Title;
            tbGenre.Text = _movie.Genre;
            tbDirector.Text = _movie.Director;
        }
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_movie != null)
        {
            _movie.Title = tbTitle.Text;
            _movie.Genre = tbGenre.Text;
            _movie.Director = tbDirector.Text;

            App.dbContext.Movies.Update(_movie);
            App.dbContext.SaveChanges();

            this.Close();
        }
    }
}