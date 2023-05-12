using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoUploader
{
    class AppState 
    {
        public static HttpClient client = new HttpClient();

        public static ObservableCollection<string> places = new ObservableCollection<string>();
        public static ObservableCollection<string> categories = new ObservableCollection<string>();
        public static ObservableCollection<string> subcategories = new ObservableCollection<string>();
        public struct Places
        {
            public List<string> places;
        }
        public struct Categories
        {
            public List<string> categories;
        }
        public struct Subcategories
        {
            public List<string> subcategories;
        }
        public static ObservableCollection<FileResult> photos = new ObservableCollection<FileResult>();


    }
}
