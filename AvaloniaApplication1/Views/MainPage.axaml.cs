using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1
{
    public partial class MainPage : UserControl
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void btnUsers_Click(object? sender, RoutedEventArgs e)
        {
            ContentControl.Content = new UsersPage();
        }

        private void btnMovies_Click(object? sender, RoutedEventArgs e)
        {
            ContentControl.Content = new MoviesPage();
        }

        private void btnBasket_Click(object? sender, RoutedEventArgs e)
        {
            ContentControl.Content = new BasketPage();
        }

        private void btnOrders_Click(object? sender, RoutedEventArgs e)
        {
            ContentControl.Content = new OrdersPage();
        }

        private void btnCategories_Click(object? sender, RoutedEventArgs e)
        {
            ContentControl.Content = new CategoriesPage();
        }

        private void btnLogout_Click(object? sender, RoutedEventArgs e)
        {
            Data.ContextData.CurrentLoggedInUser = null;
            var authWindow = new AuthWindow();
            authWindow.Show();
            (this.VisualRoot as Window)?.Close();
        }
    }
}