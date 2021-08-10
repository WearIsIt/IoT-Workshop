using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using CustomerApp.Annotations;
using CustomerApp.ImagesSearchResults;
using CustomerApp.Services;
using Xamarin.Forms;
using System.Threading.Tasks;

namespace CustomerApp.ViewModels
{
    public class DisplayImageViewModel : INotifyPropertyChanged
    {
        readonly IList<ItemToShow> source;
        public ObservableCollection<ItemToShow> ItemsToShow { get; private set; }

        private readonly INavigationService _navigationService;


        public DisplayImageViewModel([NotNull] INavigationService navigationService, string[] itemsAfterUnion)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            source = new List<ItemToShow>();
            if (itemsAfterUnion.Length == 0) // There are no items to display
            {
                NoItems = "no items to display";
                BackgroundImage = "hangersOnRack.jpg";
                MarginNoItems = new Thickness(60, 70, 50, 50);
            }

            else // There are items to display
            {
                MarginNoItems = new Thickness(60, -100, 50, 50);
                BackgroundImage = "backgroundPinkBlue2.jpg";
            }
            CreateItemsCollection(itemsAfterUnion);
        }


        void CreateItemsCollection(string[] itemsAfterUnion)
        {
            foreach (string item in itemsAfterUnion)
            {
                string imageFullName = BlobServices.getImageFullname(item);
                string urlImage;
                if (imageFullName == null) // Should not happen (just in case)- means we have item linked to tag, but item doesn't have image on blob storage
                {
                    urlImage = "image_preview_placeholder.png";
                }

                else // The common scenario
                {
                    urlImage = "https://projectstorageaccount0.blob.core.windows.net/images/" + imageFullName;
                }

                bool forIsFavorite;
                if (App.IsUserLoggedIn && LoginViewModel.favoriteItems.Contains(item))
                {
                    forIsFavorite = true;
                }

                else
                {
                    forIsFavorite = false;
                }

                source.Add(new ItemToShow
                {
                    Name = item,
                    ImageUrl = urlImage,
                    IsFavorite = forIsFavorite
                });
            }

            ItemsToShow = new ObservableCollection<ItemToShow>(source);
        }


        private string _HeartImage;
        public string HeartImage
        {
            get => _HeartImage;
            set
            {
                if (value == _HeartImage) return;
                _HeartImage = value;
                OnPropertyChanged(nameof(HeartImage));
            }
        }


        public async Task NavigateToItemPageAsync(string currItemName, string source)
        {
            Tuple<string, string> tup = new Tuple<string, string>(currItemName, source);
            await App.NavigationService.NavigateAsync(PageNames.ItemLocationPage, tup);
        }


        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        public string NoItems { get; set; }
        public string BackgroundImage { get; set; }
        public Thickness MarginNoItems { get; set; }


    }
}
