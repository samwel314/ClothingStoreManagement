using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using ClothingStoreManagement.Data;
using ClothingStoreManagement.Application.Mapping;
using ClothingStoreManagement.Data.Repository;
using ClothingStoreManagement.Data.Repository.implementation;
using ClothingStoreManagement.Application.Services;

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
            var serviceCollection = new ServiceCollection();
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