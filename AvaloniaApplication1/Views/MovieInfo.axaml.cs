using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1;

public partial class MovieInfo : Window
{
    public MovieInfo()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var title = tbTitle.Text;
        var genre = tbGenre.Text;
        var director = tbDirector.Text;

        var newMovie = new Movie()
        {
            Title = title,
            Genre = genre,
            Director = director,
        };

        App.dbContext.Movies.Add(newMovie);
        App.dbContext.SaveChanges();

        this.Close();
    }
}