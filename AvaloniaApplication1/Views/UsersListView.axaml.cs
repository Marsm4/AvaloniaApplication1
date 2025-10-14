using Avalonia.Controls;
using Avalonia.Interactivity;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1.Views
{
    public partial class UsersListView : UserControl
    {
        public UsersListView()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            UsersDataGrid.ItemsSource = App.dbContext.Users.ToList();
            UsersDataGrid.Columns.Clear();
            UsersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Имя", Binding = new Avalonia.Data.Binding("Name") });
            UsersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Email", Binding = new Avalonia.Data.Binding("Email") });
            UsersDataGrid.Columns.Add(new DataGridTextColumn { Header = "Роль", Binding = new Avalonia.Data.Binding("Role") });
        }

        private async void AddButton_Click(object? sender, RoutedEventArgs e)
        {
            var userEditView = new UserEditView();
            var window = new Window
            {
                Content = userEditView,
                Title = "Добавление пользователя",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadUsers();
        }

        private void DeleteButton_Click(object? sender, RoutedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as User;
            if (selectedUser != null)
            {
                App.dbContext.Users.Remove(selectedUser);
                App.dbContext.SaveChanges();
                LoadUsers();
            }
        }

        private async void DataGrid_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
        {
            var selectedUser = UsersDataGrid.SelectedItem as User;
            if (selectedUser == null) return;

            ContextData.CurrentUser = selectedUser;
            var userEditView = new UserEditView();
            var window = new Window
            {
                Content = userEditView,
                Title = "Редактирование пользователя",
                Width = 400,
                Height = 300,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            await window.ShowDialog((Window)this.VisualRoot);
            LoadUsers();
        }
    }
}