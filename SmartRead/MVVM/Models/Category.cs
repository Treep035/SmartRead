using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartRead.MVVM.Models
{
    public class Category
    {
        public string Name { get; set; }
        public ObservableCollection<Book> Books { get; set; }

        public Category(string name, ObservableCollection<Book> books)
        {
            Name = name;
            Books = books;
        }
    }
}
