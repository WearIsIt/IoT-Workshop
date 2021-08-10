using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CustomerApp.ViewModels;
using CustomerApp.Services;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net.Http;
using Nancy.Json;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.Extensions;
using Acr.UserDialogs;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemLocationPage : ContentPage
    {
        public ItemLocationPage(Tuple<string, string> tup)
        {           
            itemName = tup.Item1;
            ViewModel= new ItemLocationModel(App.NavigationService, tup);
            BindingContext = ViewModel;
            ScannerPage.popup = false;
            InitializeComponent();

            textLabel.Text = itemName;

            // Check if image exists ib blob
            _isImagePlaceholderVisible = !BlobServices.CheckIfExistsInBlob(itemName);
            if (!_isImagePlaceholderVisible) // image placeholder is not visiable - image exists in blob
            {
                imagePlaceHolder.Source = "offWhite_placeholder.jpeg"; // image place holder is white
                updateImageSource();
            }

            else // There is no image on blob - set placeholder image
            {
                imagePlaceHolder.Source = "image_preview_placeholder.png";
            }

            getEntitiesByPartitionKey();

            connectToSignalROnLocationChanged();

            // Init heart icon
            if (App.IsUserLoggedIn && LoginViewModel.favoriteItems.Contains(itemName)) // Item is already in user favorites
            {
                AddToFavourites.IconImageSource = "heartFilled.png";
            }

            else // Item is not in user favorites
            {
                AddToFavourites.IconImageSource = "heartNotFilled.png";
            }
        }


        private HubConnection connectionToLocationHub = null;


        private void connectToSignalROnLocationChanged()
        {
            string urlNegotiate = "https://locationchangedsignalr.azurewebsites.net/api/negotiate";

            // call negotiate
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlNegotiate).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            SignalRConnectionInfo signalR = JsonConvert.DeserializeObject<SignalRConnectionInfo>(result);

            connectionToLocationHub = new HubConnectionBuilder()
                 .WithUrl(signalR.Url, option =>
                 {
                     option.AccessTokenProvider = () =>
                     {
                         return Task.FromResult(signalR.AccessToken);
                     };
                 }).Build();

            connectionToLocationHub.On<string>("LocationUpdate", (itemId) =>
            {

                if (itemId == itemName)
                {
                    var actions = new SnackBarActionOptions
                    {
                        Action = () => refreshAsync(),
                        Text = "Update!"
                    };

                    var options = new SnackBarOptions
                    {
                        MessageOptions = new MessageOptions
                        {
                            Foreground = Color.White,
                            Message = "There are updates for " + itemName + " item"
                        },
                        BackgroundColor = Color.FromHex("#63615E"),
                        Duration = TimeSpan.FromSeconds(10),
                        Actions = new[] { actions }
                    };

                    Task.Run(async () =>
                    {
                        await this.DisplaySnackBarAsync(options);
                    });
                }
            });

            connectionToLocationHub.StartAsync();
        }


/*        protected override async void OnAppearing()
        {         
            if (connectionToLocationHub!=null)
            {
                await connectionToLocationHub.StartAsync();
            }
            
        }

        protected override async void OnDisappearing()
        {
            await connectionToLocationHub.StopAsync();
        }*/


        private async Task refreshAsync()
        {
            getEntitiesByPartitionKey();
        }


        private void getEntitiesByPartitionKey()
        {
            string urlGetEntities = "https://updateprojecttablefunctionapp.azurewebsites.net/api/search-by-partitionkey/" + itemName;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetEntities).Result;
            var resultFromHttp = response.Content.ReadAsStringAsync().Result;
          
            JavaScriptSerializer js = new JavaScriptSerializer();
            ItemEntity[] entities= js.Deserialize<ItemEntity[]>(resultFromHttp);
            GetItemDescription(entities);
        }


        private void GetItemDescription(ItemEntity[] entities)
        {
            IDictionary<string, int> sizeIndices = new Dictionary<string, int>();
            sizeIndices.Add("XS", 0);
            sizeIndices.Add("Small", 1);
            sizeIndices.Add("Medium", 2);
            sizeIndices.Add("Large", 3);
            sizeIndices.Add("XL", 4);

            IDictionary<string, int[]> locationsDict = new Dictionary<string, int[]>();
            locationsDict.Add("Stand", new int[5]);
            locationsDict.Add("Fitting Room", new int[5]);
            locationsDict.Add("Storeroom", new int[5]);

            string currLocation, currSize;
            int currSizeIndex;

            foreach (ItemEntity entity in entities)
            {
                currLocation=entity.Location;
                currSize = entity.Size;
                currSizeIndex = sizeIndices[currSize];
                locationsDict[currLocation][currSizeIndex]++;
            }

            UpdateDetailsInXaml(locationsDict);
        }


        private void UpdateDetailsInXaml(IDictionary<string, int[]> locationsDict)
        {
            IDictionary<int, string> sizeIndices = new Dictionary<int, string>();
            sizeIndices.Add(0, "XS");
            sizeIndices.Add(1, "Small");
            sizeIndices.Add(2, "Medium");
            sizeIndices.Add(3, "Large");
            sizeIndices.Add(4, "XL");

            standCell.Detail=updateSpecificLocationInXaml("Stand", locationsDict, sizeIndices);
            fittingCell.Detail = updateSpecificLocationInXaml("Fitting Room", locationsDict, sizeIndices);
            storeroomCell.Detail = updateSpecificLocationInXaml("Storeroom", locationsDict, sizeIndices);         
        }


        private string updateSpecificLocationInXaml(string locationName, IDictionary<string, int[]> locationsDict,
            IDictionary<int, string> sizeIndices)
        {
            string detailsToUpdate = "";
            int[] locationSizes = locationsDict[locationName];
            for (int i = 0; i < 5; i++)
            {
                if (locationsDict[locationName][i] != 0)
                {
                    if (detailsToUpdate=="")
                    {
                        detailsToUpdate += sizeIndices[i] + ": " + locationsDict[locationName][i];
                    }
                    else
                    {
                        detailsToUpdate += "  |  " + sizeIndices[i] + ": " + locationsDict[locationName][i];
                    }
                }
            }

            if (detailsToUpdate=="")
            {
                detailsToUpdate = "There are no items here";
            }
            return detailsToUpdate;
        }


        private void updateImageSource()
        {
            string imageFullName = BlobServices.imageNameSuffixIncluded;
            string urlImage = "https://projectstorageaccount0.blob.core.windows.net/images/" + imageFullName;
            ImageToShow.Source = urlImage;
        }


        private bool _isImagePlaceholderVisible = false;
        public bool IsImagePlaceholderVisible
        {
            get => _isImagePlaceholderVisible;
            set => SetPropertyNew(ref _isImagePlaceholderVisible, value);
        }


        public bool SetPropertyNew<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        string itemName { set; get; }


        [Obsolete]
        void OnClick_GoToMain(object sender, EventArgs e) // back to main button
        {
            ViewModel.BackToMainPageAsync();
        }


        public ItemLocationModel ViewModel { get; set; }


        [Obsolete]
        void OnClick_AddToFav(object sender, EventArgs e) // back to main button
        {
            if (App.IsUserLoggedIn == false)
            {
                DisplayAlert("Oops..", "You must be logged in to save your items. Please " +
                    "go to home page to login.", "OK");
                return;
            }
           
            if (!LoginViewModel.favoriteItems.Contains(itemName)) // user added item to favourites
            {
                AddToFavourites.IconImageSource = "heartFilled.png";

                // Add item to user favorites
                LoginViewModel.favoriteItems.Add(itemName);
            }
            else // If LoginViewModel.favoriteItems.Contains(itemName), user removed item from favourites
            {
                AddToFavourites.IconImageSource = "heartNotFilled.png";

                // Remove item from user favorites
                LoginViewModel.favoriteItems.Remove(itemName);
            }
        }


    }
}