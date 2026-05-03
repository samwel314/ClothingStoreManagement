using ClothingStoreManagement.Application.Mapping;
using ClothingStoreManagement.Application.Services;
using ClothingStoreManagement.Data;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Data.Repository.implementation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Media.Imaging;

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
            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "app.db");

            serviceCollection.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });
            serviceCollection.AddScoped<IUnitOfWork , UnitOfWork>();
            serviceCollection.AddScoped<ColorService , ColorService>(); 
            serviceCollection.AddScoped<SizeService , SizeService>();
            serviceCollection.AddScoped<CategoryService , CategoryService>();
            serviceCollection.AddScoped<ProductService , ProductService>(); 
            serviceCollection.AddScoped<InvoiceService , InvoiceService>(); 
            //-*****************************************
            var serviceProvider = serviceCollection.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Database.Migrate();
            }

            Resources.Add("services", serviceProvider);
        }
    }
}