using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using AvaloniaApplication1.Data;
using System.Linq;

namespace AvaloniaApplication1
{
    public partial class App : Application
    {
        public static AppDbContext dbContext { get; set; } = new AppDbContext();

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                DisableAvaloniaDataAnnotationValidation();

                // Создаем администратора по умолчанию
                CreateDefaultAdmin();

                // Запускаем окно авторизации
                desktop.MainWindow = new AuthWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }

        private void CreateDefaultAdmin()
        {
            var admin = dbContext.Users.FirstOrDefault(u => u.Role == "Admin");
            if (admin == null)
            {
                var defaultAdmin = new User
                {
                    Name = "Администратор",
                    Email = "admin",
                    Password = "admin",
                    Role = "Admin"
                };
                dbContext.Users.Add(defaultAdmin);
                dbContext.SaveChanges();
            }
        }

        private void DisableAvaloniaDataAnnotationValidation()
        {
            var dataValidationPluginsToRemove =
                BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

            foreach (var plugin in dataValidationPluginsToRemove)
            {
                BindingPlugins.DataValidators.Remove(plugin);
            }
        }
    }
}