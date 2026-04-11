using System.Windows;
using ChildernOfTheBible.Data;
using ChildernOfTheBible.Services;
using ChildernOfTheBible.ViewModels;
using ChildernOfTheBible.Views;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;

namespace ChildernOfTheBible
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }
        public static IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            //Build Configuration
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

           Configuration = builder.Build();

            //Register Services
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = ServiceProvider.CreateScope())
            {
                // 4. Resolve MainViewModel and MainWindow
                var mainWindow = scope.ServiceProvider.GetRequiredService<MainWindow>();

                // 5. Link them and Show
                mainWindow.Show();
            }
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Register DbContext with connection string from appsettings.json
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Register Services
            services.AddTransient<DatabaseService>();
            services.AddTransient<BarcodeService>();
            services.AddTransient<ReportService>();

            // Register WPF viewmodels
            services.AddTransient<MainViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<AttendanceViewModel>();
            services.AddTransient<ReportingViewModel>();

            // Register WPF windows views
            services.AddTransient<MainWindow>();
            services.AddTransient<UserManagement>();
            services.AddTransient<AttendanceView>();
            services.AddTransient<ReportingView>();
        }

    }

}
