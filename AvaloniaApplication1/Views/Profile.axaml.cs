using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1;

public partial class Profile : Window
{
    private User _user;

    public Profile()
    {
        InitializeComponent();

        if (ContextData.CurrentUser != null)
        {
            _user = ContextData.CurrentUser;
            tbName.Text = _user.Name;
            tbEmail.Text = _user.Email;
            tbPassword.Text = _user.Password;
        }
    }

    private void SaveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (_user != null)
        {
            _user.Name = tbName.Text;
            _user.Email = tbEmail.Text;
            _user.Password = tbPassword.Text;

            App.dbContext.Users.Update(_user);
            App.dbContext.SaveChanges();

            this.Close();
        }
    }
}