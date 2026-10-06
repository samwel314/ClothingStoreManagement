using ClothingStoreManagement.Application.Mapping;
using ClothingStoreManagement.Application.Services;
using ClothingStoreManagement.Data;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Data.Repository.implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace ClothingStoreManagement.Ui
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WindowState = WindowState.Maximized;
            //  Icon = new BitmapImage(new Uri("Assets/app.ico", UriKind.Relative));
            Title = $"🧥 Clothing Store System | Developed by Eng. Samuel Marzouk © {DateTime.Now.Year}"; var serviceCollection = new ServiceCollection();
            serviceCollection.AddWpfBlazorWebView();
            var dataDirectory = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                         "ClothingStoreManagement",
                         "Data");

            Directory.CreateDirectory(dataDirectory);

            var dbPath = Path.Combine(dataDirectory, "ClothingStoreManagement.db");

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            }); 
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            serviceCollection.AddScoped<IUnitOfWork, UnitOfWork>();
            serviceCollection.AddScoped<ColorService, ColorService>();
            serviceCollection.AddScoped<SizeService, SizeService>();
            serviceCollection.AddScoped<CategoryService, CategoryService>();
            serviceCollection.AddScoped<ProductService, ProductService>();
            serviceCollection.AddScoped<InvoiceService, InvoiceService>();
            serviceCollection.AddScoped<UserService, UserService    >();
            serviceCollection.AddScoped<AppState, AppState>();  
            serviceCollection.AddScoped<ShiftService, ShiftService>();
            serviceCollection.AddScoped<PaymentSourceService, PaymentSourceService>();
            serviceCollection.AddScoped<MainTreasuryService, MainTreasuryService>();
            serviceCollection.AddScoped<EmployeeService, EmployeeService>();
            serviceCollection.AddSingleton<IReceiptPrinterService, WindowsReceiptPrinterService>();
            serviceCollection.AddScoped<IBarcodeService, BarcodeService>();
            serviceCollection.AddScoped<IWindowsBarcodePrinterService, WindowsBarcodePrinterService>();
            //-*****************************************
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            Resources.Add("services", serviceProvider);
        }
        private static string GetSettingsDirectory()
        {
            var settingsDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ClothingStoreManagement",
                "Settings");

            Directory.CreateDirectory(settingsDirectory);

            return settingsDirectory;
        }
        private static string? SelectBackupFolder()
        {
            var dialog = new OpenFolderDialog
            {
                Title = "Select Backup Folder",
                Multiselect = false
            };

            return dialog.ShowDialog() == true
                ? dialog.FolderName
                : null;
        }
        private static string GetSettingsPath()
        {
            return Path.Combine(
                GetSettingsDirectory(),
                "settings.json");
        }
    }
    public sealed class AppSettings
    {
        public string? BackupFolder { get; set; }
    }
}