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
            services.AddSingleton<IConfiguration>(Configuration);

            // ✅ CHANGED: AddDbContextFactory instead of AddDbContext
            // This allows services to create fresh contexts on demand
            // rather than sharing one scoped instance that gets disposed
            services.AddDbContextFactory<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Core Services
            services.AddSingleton<EncryptionService>();
            services.AddTransient<MemberService>();
            services.AddTransient<BarcodeService>();
            services.AddTransient<AttendanceService>();
            services.AddTransient<ReportService>();
            services.AddSingleton<WebcamService>();

            // ViewModels
            services.AddTransient<MainViewModel>();
            services.AddTransient<UserManagementViewModel>();
            services.AddTransient<AttendanceViewModel>();
            services.AddTransient<ReportingViewModel>();

            // Views
            services.AddTransient<MainWindow>();
            services.AddTransient<UserManagement>();
            services.AddTransient<AttendanceView>();
            services.AddTransient<ReportingView>();
        }


    }

}
