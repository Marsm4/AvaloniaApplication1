using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1;

public partial class UserInfo : Window
{
    public UserInfo()
    {
        InitializeComponent();
    }

    private void Button_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var name = tbName.Text;
        var email = tbEmail.Text;
        var password = tbPassword.Text;

        var newUser = new User()
        {
            Name = name,
            Email = email,
            Password = password,
        };

        App.dbContext.Users.Add(newUser);
        App.dbContext.SaveChanges();

        this.Close();
    }
}