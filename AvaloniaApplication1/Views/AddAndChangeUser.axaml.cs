using System.Xml.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;

namespace AvaloniaApplication1
{
    public partial class AddAndChangeUser : Window
    {
        User user;

        public AddAndChangeUser()
        {
            InitializeComponent();
            user = new User();
        }

        public AddAndChangeUser(User user)
        {
            InitializeComponent();
            this.user = user;
            tbName.Text = user.Name;
            tbEmail.Text = user.Email;
            tbPassword.Text = user.Password;

            // Устанавливаем выбранную роль
            foreach (ComboBoxItem item in cbRole.Items)
            {
                if (item.Content?.ToString() == user.Role)
                {
                    cbRole.SelectedItem = item;
                    break;
                }
            }
        }

        private void btnSave_Click(object? sender, RoutedEventArgs e)
        {
            user.Name = tbName.Text;
            user.Email = tbEmail.Text;
            user.Password = tbPassword.Text;
            user.Role = (cbRole.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "User";

            if (user.Id == 0)
            {
                App.dbContext.Users.Add(user);
            }
            else
            {
                App.dbContext.Update(user);
            }
            App.dbContext.SaveChanges();
            this.Close();
        }
    }
}