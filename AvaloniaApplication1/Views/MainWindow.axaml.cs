using Avalonia.Controls;

namespace AvaloniaApplication1
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainControl.Content = new MainPage();
        }
    }
}