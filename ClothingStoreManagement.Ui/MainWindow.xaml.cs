using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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