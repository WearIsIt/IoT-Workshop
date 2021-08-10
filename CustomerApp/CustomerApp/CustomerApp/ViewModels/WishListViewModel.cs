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
    public class WishListViewModel : INotifyPropertyChanged
    {
        private readonly INavigationService _navigationService;

        public WishListViewModel([NotNull] INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            sourceCollection = new List<ItemToShow>();
            CreateItemsCollection();

            if (ItemsToShow.Count == 0) // There are no items to display
            {
                EmptyWishList = "Your Wishlist is empty";
                WishListBackgroundImage = "hangersOnRack.jpg";
                MarginEmptyWishList = new Thickness(60, 70, 50, 50);
            }

            else // There are items to display
            {
                MarginEmptyWishList = new Thickness(60, -100, 50, 50);
                WishListBackgroundImage = "background18.jpg";
            }
        }


        readonly IList<ItemToShow> sourceCollection;
        public ObservableCollection<ItemToShow> ItemsToShow { get; private set; }


        void CreateItemsCollection()
        {
            foreach (string item in LoginViewModel.favoriteItems)
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

                sourceCollection.Add(new ItemToShow
                {
                    Name = item,
                    ImageUrl = urlImage
                });
            }

            ItemsToShow = new ObservableCollection<ItemToShow>(sourceCollection);
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

        public string EmptyWishList { get; set; }
        public string WishListBackgroundImage { get; set; }
        public Thickness MarginEmptyWishList { get; set; }
        

    }
}
