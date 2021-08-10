using System.Collections.Generic;
using Xamarin.Forms.MultiSelectListView;

namespace CustomerApp.TagSearch
{
    public class TagsCategories
    {
        public string Name { get; set; }
        public MultiSelectObservableCollection<Tag> TagsCollection { get; set; }


    }
}
