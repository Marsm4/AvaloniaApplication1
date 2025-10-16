using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Tmds.DBus.Protocol;

namespace AvaloniaApplication1
{
    public partial class ConfirmDialog : Window
    {
        public ConfirmDialog()
        {
            InitializeComponent();
        }

        public ConfirmDialog(string message)
        {
            InitializeComponent();
            tbMessage.Text = message;
        }

        private void btnYes_Click(object? sender, RoutedEventArgs e)
        {
            Close(true);
        }

        private void btnNo_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}