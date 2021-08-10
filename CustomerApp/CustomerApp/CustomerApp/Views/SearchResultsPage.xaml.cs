using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using CustomerApp.ImagesSearchResults;
using CustomerApp.TagSearch;
using CustomerApp.ViewModels;
using Nancy.Json;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchResultsPage : ContentPage
    {

        public HashSet<SelectableItem<Tag>> selectedHash;
        private string source;


        public SearchResultsPage(Tuple<HashSet<SelectableItem<Tag>>, string> tup)
        {
            selectedHash = tup.Item1;
            source = tup.Item2;

            string[] itemsAfterUnion = initResults();
            InitializeComponent();
            ViewModel = new DisplayImageViewModel(App.NavigationService, itemsAfterUnion);
            BindingContext = ViewModel;
            ScannerPage.popup = false;            
        }


        public DisplayImageViewModel ViewModel { get; }


        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currItemName = (e.CurrentSelection.FirstOrDefault() as ItemToShow)?.Name;
            ViewModel.NavigateToItemPageAsync(currItemName, source);

        }


        public string[] initResults()
        {
            string tagsString = "";
            foreach (var hashItem in selectedHash)
            {
                if (tagsString != "")
                {
                    tagsString += "_" + hashItem.Data.tagName;
                }

                else
                {
                    tagsString += hashItem.Data.tagName;
                }
            }

            return getUnionItemsByTags(tagsString);
        }


        public string[] getUnionItemsByTags(string tagsString)
        {
            string urlGetItems = "https://uploadtagstostroage.azurewebsites.net/api/getItemsAccordingTags/" + tagsString;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetItems).Result;
            var resultFromHttp = response.Content.ReadAsStringAsync().Result;

            JavaScriptSerializer js = new JavaScriptSerializer();
            string[] entities = js.Deserialize<string[]>(resultFromHttp);
            return entities;
        }


        [Obsolete]
        void OnClick_AddToFav(object sender, EventArgs e) // Back to main button
        {
            if (App.IsUserLoggedIn == false)
            {
                DisplayAlert("Oops..", "You must be logged in to save your items. Please " +
                    "go to home page to login.", "OK");
                return;
            }

            Button button = (Button)sender;
            var gridParent = (Grid)button.Parent;
            var labelName = (Label)gridParent.Children[1];
            var itemName = labelName.Text;

            if (!LoginViewModel.favoriteItems.Contains(itemName)) // user added item to favourites
            {
                button.ImageSource = "heartFilled.png";

                // Add item to user favorites
                LoginViewModel.favoriteItems.Add(itemName);
            }

            else // If LoginViewModel.favoriteItems.Contains(itemName), user removed item from favourites
            {
                button.ImageSource = "heartNotFilled.png";

                // Remove item from user favorites
                LoginViewModel.favoriteItems.Remove(itemName);
            }
        }


    }
}