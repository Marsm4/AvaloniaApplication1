using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using AvaloniaApplication1.Views;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class AuthWindow : Window
    {
        public AuthWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object? sender, RoutedEventArgs e)
        {
            string email = tbEmail.Text ?? string.Empty;
            string password = tbPassword.Text ?? string.Empty;

            // ��������� �����������
            var user = App.dbContext.Users
                .FirstOrDefault(u => u.Email == email && u.Password == password);

            if (user != null)
            {
                // ��������� �������� ������������
                ContextData.CurrentLoggedInUser = user;

                // ��������� ������� ����
                var mainWindow = new MainWindow();
                mainWindow.Show();

                // ��������� ���� �����������
                this.Close();
            }
            else
            {
                ShowError("�������� email ��� ������");
            }
        }

        private void Register_Click(object? sender, RoutedEventArgs e)
        {
            // ��������� �� �������� �����������
            var registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private void ShowError(string message)
        {
            var messageBox = new Window
            {
                Title = "������",
                Content = new TextBlock { Text = message },
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };
            messageBox.ShowDialog(this);
        }
    }
}