using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CustomerApp.Annotations;
using CustomerApp.Services;
using CustomerApp.TagSearch;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;

namespace CustomerApp.ViewModels
{
    public class TagsCategoriesViewModel : INotifyPropertyChanged
    {
        public List<TagsCategories> categories { get; private set; }

        public ICommand ExpandCommand { get; private set; }

        public bool IsExpanded { get; set; }
        public string Message { get; private set; }
        public MultiSelectObservableCollection<Tag> tagsCollection { get; set; }


        public TagsCategoriesViewModel()
        {
            tagsCollection = new MultiSelectObservableCollection<Tag>();
            CreateTagsCollection();
            ExpandCommand = new Command<TagsCategories>(Expand);
            IsExpanded = true;
        }


        private readonly INavigationService _navigationService;

        public TagsCategoriesViewModel([NotNull] INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            tagsCollection = new MultiSelectObservableCollection<Tag>();
            CreateTagsCollection();
            ExpandCommand = new Command<TagsCategories>(Expand);
            IsExpanded = true;
        }


        void Expand(TagsCategories tagsCategory)
        {
            Message = $"{tagsCategory.Name} tapped.";
            OnPropertyChanged("Message");
        }


        void CreateTagsCollection()
        {
            categories = new List<TagsCategories>();

            foreach (string categoryName in TagDict.clothesDict.Keys)
            {
                tagsCollection = new MultiSelectObservableCollection<Tag>();
                foreach (string tagValue in TagDict.clothesDict[categoryName])
                {
                    Tag tagObject = new Tag();
                    tagObject.tagName = tagValue;
                    tagsCollection.Add(tagObject);
                }

                categories.Add(new TagsCategories
                {
                    Name = categoryName,
                    TagsCollection=tagsCollection
                });
            }     
        }


        public void NavigateToSearchResults(HashSet<SelectableItem<Tag>> selectedHash, string source)
        { 
            Tuple<HashSet<SelectableItem<Tag>>, string> tup = new Tuple<HashSet<SelectableItem<Tag>>, string>(selectedHash, source);
            App.NavigationService.NavigateAsyncWithLoad(PageNames.SearchResultsPage, tup, 1000);
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


    }
}
