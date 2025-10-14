using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object? sender, RoutedEventArgs e)
        {
            string name = tbName.Text ?? string.Empty;
            string email = tbEmail.Text ?? string.Empty;
            string password = tbPassword.Text ?? string.Empty;
            string confirmPassword = tbConfirmPassword.Text ?? string.Empty;


            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("Все поля должны быть заполнены");
                return;
            }

            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            var existingUser = App.dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ShowError("Пользователь с таким email уже существует");
                return;
            }

            var newUser = new User()
            {
                Name = name,
                Email = email,
                Password = password,
                Role = "User"
            };

            App.dbContext.Users.Add(newUser);
            App.dbContext.SaveChanges();

            BackToAuth();
        }

        private void Back_Click(object? sender, RoutedEventArgs e)
        {
            BackToAuth();
        }

        private void BackToAuth()
        {
            var authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            var messageBox = new Window
            {
                Title = "Ошибка",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            messageBox.ShowDialog(this);
        }
    }
}