using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using MsBox.Avalonia.Enums;
using MsBox.Avalonia;

namespace AvaloniaApplication1
{
    public partial class UsersPage : UserControl
    {
        private bool _isAdmin; //проверка админуки
        public UsersPage()
        {
            InitializeComponent();

            var currentUser = Data.ContextData.CurrentLoggedInUser;
            _isAdmin = currentUser?.Role == "Admin";
            if (!_isAdmin)
            {
                btnAddUser.IsEnabled = false;
                ShowMessage("Режим просмотра: доступны только функции добавления в корзину");
            }


            Refresh();
        }
        private void ShowMessage(string message)
        {
            var box = MessageBoxManager
                .GetMessageBoxStandard("Информация", message, ButtonEnum.Ok);
            box.ShowAsync();
        }
        private async void btnAddUser_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var parent = this.VisualRoot as Window;
            var add = new AddAndChangeUser();
            await add.ShowDialog(parent);
            Refresh();
        }

        private async void btnDeleteUser_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var user = (sender as Button).Tag as User;
            var confirmWindow = new ConfirmDialog("Удаление пользователя " + user.Name);

            confirmWindow.Height = 100;
            confirmWindow.Width = 300;  

            var parent = this.VisualRoot as Window;
            var result = await confirmWindow.ShowDialog<bool>(parent);

            if (result)
            {
                App.dbContext.Users.Remove(user);
                App.dbContext.SaveChanges();
            }
            Refresh();
        }

        private void Refresh()
        {
            dgUsers.ItemsSource = App.dbContext.Users.ToList().OrderBy(x => x.Id);
        }

        private async void btnEditUser_Click(object? sender, RoutedEventArgs e)
        {
            if (!_isAdmin) return;
            var user = (sender as Button).Tag as User;
            if (user == null) return;

            var editWindow = new AddAndChangeUser(user);
            var parent = this.VisualRoot as Window;
            await editWindow.ShowDialog(parent);
            Refresh();
        }
    }
}