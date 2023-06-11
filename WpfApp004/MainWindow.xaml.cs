using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp004.Models;

namespace WpfApp004
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<string> list { get; set; } = new List<string>() { "Authors", "Themes", "Categories" };

        public ObservableCollection<string> list2 { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        string? selecteditem1;
        string? selecteditem2;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            list2 = new();
            Books = new();

        }


        public DbContextOptions<LibraryContext> GetOptions(string configname)
        {
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            builder.AddJsonFile("AppConfig.json");
            var config = builder.Build();
            string ConnectionString = config.GetConnectionString(configname)!;
            DbContextOptionsBuilder<LibraryContext> optionsbuilder = new DbContextOptionsBuilder<LibraryContext>();
            optionsbuilder.UseSqlServer(ConnectionString);
            return optionsbuilder.Options;
        }

        private void combobox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo=sender as ComboBox;
            if (combo is not null)
            {
                selecteditem1 = combo.SelectedItem as string;
                list2.Clear();
                using (var db = new LibraryContext(GetOptions("Connection1")))
                {
                    if (selecteditem1 == "Authors")
                    {
                        db.Authors.ToList().ForEach(a => list2.Add(a.FirstName + " " + a.LastName));
                    }
                    else if (selecteditem1== "Categories")
                    {
                        db.Categories.ToList().ForEach(a => list2.Add(a.Name));
                    }
                    else if (selecteditem1 == "Themes")
                    {
                        db.Themes.ToList().ForEach(a => list2.Add(a.Name));
                    }
                }
            }
            
        }

        private void combobox2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var combo = sender as ComboBox;
            if (combo is not null)
            {
                selecteditem2 = combo.SelectedItem as string;
                Books.Clear();
                using (var db = new LibraryContext(GetOptions("Connection1")))
                {
                    if (selecteditem1 == "Authors")
                    {
                        var books = db.Books.Join(db.Authors, b => b.IdAuthor, a => a.Id,
                            (b, a) => new
                            {
                                Book = b,
                                Author = a,
                            }).Where(result => result.Author.FirstName + " " + result.Author.LastName == selecteditem2)
                            .Select(result => result.Book)
                            .ToList();

                        books.ForEach(b => Books.Add(b));
                    }
                    else if (selecteditem1 == "Themes")
                    {
                        var books = db.Books.Join(db.Themes, b => b.IdThemes, t => t.Id,
                            (b, t) => new
                            {
                                Book = b,
                                Theme = t,
                            }).Where(result => result.Theme.Name == selecteditem2).Select(result => result.Book).ToList();
                        books.ForEach(b => Books.Add(b));
                    }
                    else if (selecteditem1 == "Categories")
                    {
                        var books = db.Books.Join(db.Categories, b => b.IdCategory, c => c.Id,
                            (b, c) => new
                            {
                                Book = b,
                                Category = c,
                            }).Where(result => result.Category.Name == selecteditem2).Select(result => result.Book).ToList();
                        books.ForEach(book => Books.Add(book));
                    }

                }
            }
        }

    }
}
