using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class RegistrationView : UserControl
    {
        public RegistrationView()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object? sender, RoutedEventArgs e)
        {
            string name = tbName.Text ?? string.Empty;
            string email = tbEmail.Text ?? string.Empty;
            string password = tbPassword.Text ?? string.Empty;
            string confirmPassword = tbConfirmPassword.Text ?? string.Empty;

            // Проверка паролей
            if (password != confirmPassword)
            {
                ShowError("Пароли не совпадают");
                return;
            }

            // Проверка, существует ли пользователь с таким email
            var existingUser = App.dbContext.Users.FirstOrDefault(u => u.Email == email);
            if (existingUser != null)
            {
                ShowError("Пользователь с таким email уже существует");
                return;
            }

            // Создание нового пользователя
            var newUser = new User()
            {
                Name = name,
                Email = email,
                Password = password,
                Role = "User" // По умолчанию обычный пользователь
            };

            App.dbContext.Users.Add(newUser);
            App.dbContext.SaveChanges();

            // Закрываем окно регистрации
            CloseWindow();
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            CloseWindow();
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
            messageBox.ShowDialog((Window)this.VisualRoot);
        }

        private void CloseWindow()
        {
            var window = (Window)this.VisualRoot;
            window?.Close();
        }
    }
}