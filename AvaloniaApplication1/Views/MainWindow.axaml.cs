// MainWindow.xaml.cs - ����������� ������
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // ���������� ���������� � ������������
            UpdateUserInfo();

            // ���������� ������ ������������� �� ���������
            ShowUsers();
        }

        private void UpdateUserInfo()
        {
            var currentUser = ContextData.CurrentLoggedInUser;
            if (currentUser != null)
            {
                UserInfoText.Text = $"�� ����� ���: {currentUser.Name} ({currentUser.Role})";
            }
        }

        private void ShowUsers_Click(object? sender, RoutedEventArgs e)
        {
            ShowUsers();
        }

        private void ShowMovies_Click(object? sender, RoutedEventArgs e)
        {
            ShowMovies();
        }

        private void ShowBasket_Click(object? sender, RoutedEventArgs e)
        {
            ShowBasket();
        }

        private void ShowUsers()
        {
            MainContent.Content = new UsersListView();
        }

        private void ShowMovies()
        {
            MainContent.Content = new MoviesListView();
        }

        private void ShowBasket()
        {
            MainContent.Content = new BasketView();
        }

        private void Logout_Click(object? sender, RoutedEventArgs e)
        {
            // ���������� �������� ������������
            ContextData.CurrentLoggedInUser = null;

            // ������������ � �����������
            var authWindow = new AuthWindow();
            authWindow.Show();
            this.Close();
        }
    }
}