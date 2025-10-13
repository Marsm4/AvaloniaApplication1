using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Xml.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class UserEditView : UserControl
    {
        private User _user;
        private bool _isEditMode;

        public UserEditView()
        {
            InitializeComponent();

            if (ContextData.CurrentUser != null)
            {
                _user = ContextData.CurrentUser;
                _isEditMode = true;
                tbName.Text = _user.Name;
                tbEmail.Text = _user.Email;
                tbPassword.Text = _user.Password;
            }
            else
            {
                _isEditMode = false;
            }
        }

        private void SaveButton_Click(object? sender, RoutedEventArgs e)
        {
            if (_isEditMode && _user != null)
            {
                _user.Name = tbName.Text;
                _user.Email = tbEmail.Text;
                _user.Password = tbPassword.Text;
                App.dbContext.Users.Update(_user);
            }
            else
            {
                var newUser = new User()
                {
                    Name = tbName.Text,
                    Email = tbEmail.Text,
                    Password = tbPassword.Text,
                };
                App.dbContext.Users.Add(newUser);
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