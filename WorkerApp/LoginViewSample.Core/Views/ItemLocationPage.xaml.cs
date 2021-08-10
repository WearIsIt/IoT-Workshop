using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LoginViewSample.Core.ViewModels;
using LoginViewSample.Core.Services;
using LoginViewSample.Core.Settings;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net.Http;
using Nancy.Json;

namespace LoginViewSample.Core.Views
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
            _isImagePlaceholderVisible = !UploadToBlob.CheckIfExistsInBlob(itemName);
            if (!_isImagePlaceholderVisible) // Image placeholder is not visiable - image exists in blob
            {
                imagePlaceHolder.Source = "offWhite_placeholder.jpeg"; // Image place holder is white
                updateImageSource();
            }

            else // There is no image on blob - set placeholder image
            {
                imagePlaceHolder.Source = "image_preview_placeholder.png";
            }

            getEntitiesByPartitionKey();
        }


        private void getEntitiesByPartitionKey()
        {
            string urlGetEntities = "https://updateprojecttablefunctionapp.azurewebsites.net/api/search-by-partitionkey/" + itemName;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetEntities).Result;
            var resultFromHttp = response.Content.ReadAsStringAsync().Result;

            JavaScriptSerializer js = new JavaScriptSerializer();
            ItemEntity[] entities = js.Deserialize<ItemEntity[]>(resultFromHttp);
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
                currLocation = entity.Location;
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

            standCell.Detail = updateSpecificLocationInXaml("Stand", locationsDict, sizeIndices);
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
                    if (detailsToUpdate!="")
                    {
                        detailsToUpdate += "  |  " + sizeIndices[i] + ": " + locationsDict[locationName][i];
                    }
                    else 
                    {
                        detailsToUpdate += sizeIndices[i] + ": " + locationsDict[locationName][i];
                    }                 
                }
            }

            if (detailsToUpdate == "")
            {
                detailsToUpdate = "There are no items here";
            }
            return detailsToUpdate;
        }


        private void updateImageSource()
        {
            string imageFullName = UploadToBlob.imageNameSuffixIncluded;
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


        [Obsolete]
        void OnClick(object sender, EventArgs e) //back to main button
        {
            ViewModel.BackToMainPageAsync();
        }


        public ItemLocationModel ViewModel { get; set; }
        string itemName { set; get; }


    }
}