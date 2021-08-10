using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CustomerApp.ViewModels;
using CustomerApp.Services;
using System.Net.Http;
using Nancy.Json;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Xamarin.CommunityToolkit.UI.Views.Options;
using Xamarin.CommunityToolkit.Extensions;
using Acr.UserDialogs;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LeaderBoardPage : ContentPage
    {
        public ObservableCollection<TopEntity> topEntitiesToShow { get; set; }

        public LeaderBoardPage()
        {
            InitializeComponent();
            initPage();

            connectToSignalROnScanItem();
        }


        private void initPage()
        {
            List<TopEntity> topFiveEntitiesList = getTopFive();

            // first place affection
            if (topFiveEntitiesList[0].image != null)
            {
                firstPlaceImage.Source = topFiveEntitiesList[0].image;
            }
            firstPlaceLabel.Text = topFiveEntitiesList[0].itemName;

            // second place affection 
            if (topFiveEntitiesList[1].image != null)
            {
                secondPlaceImage.Source = topFiveEntitiesList[1].image;
            }
            secondPlaceLabel.Text = topFiveEntitiesList[1].itemName;

            // third place affection
            if (topFiveEntitiesList[2].image != null)
            {
                thirdPlaceImage.Source = topFiveEntitiesList[2].image;
            }
            thirdPlaceLabel.Text = topFiveEntitiesList[2].itemName;

            topEntitiesToShow = new ObservableCollection<TopEntity>(topFiveEntitiesList);
            MyListView.ItemsSource = topEntitiesToShow;
        }


        public List<TopEntity> getTopFive()
        {
            string urlGetTopFive = "https://scancountfunctionapp.azurewebsites.net/api/retrieve-top-five";
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlGetTopFive).Result;
            var resultFromHttp = response.Content.ReadAsStringAsync().Result;

            JavaScriptSerializer js = new JavaScriptSerializer();
            TopEntity[] TopEntityList = JsonConvert.DeserializeObject<TopEntity[]>(resultFromHttp);

            // Add image (if exists) to each TopEntity
            foreach (TopEntity currTopEntity in TopEntityList)
            {
                string imageFullName = BlobServices.getImageFullName(currTopEntity.itemName);
                if (imageFullName != null) // If itemName has an image
                {
                    imageFullName = "https://projectstorageaccount0.blob.core.windows.net/images/" + imageFullName;
                    currTopEntity.image = imageFullName;
                }

                else // itemName has no image;
                {
                    currTopEntity.image = "placeholderLeaderBoard.jpeg";
                }
            }

            return TopEntityList.ToList<TopEntity>();
        }


        private void MyListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            int currIndex = e.SelectedItemIndex;
            ListView listView = (ListView)sender;

            var currEntity = e.SelectedItem as TopEntity;
            Tuple<string, string> tup = new Tuple<string, string>(currEntity.itemName, "TopFive");
            App.NavigationService.NavigateAsync(PageNames.ItemLocationPage, tup);
        }


        private HubConnection connectionSignalRScanItem = null;


        private void connectToSignalROnScanItem()
        {
            string urlNegotiate = "https://scancountfunctionapp.azurewebsites.net/api/negotiate";

            // call negotiate
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlNegotiate).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            SignalRConnectionInfo signalR = JsonConvert.DeserializeObject<SignalRConnectionInfo>(result);

            connectionSignalRScanItem = new HubConnectionBuilder()
                 .WithUrl(signalR.Url, option =>
                 {
                     option.AccessTokenProvider = () =>
                     {
                         return Task.FromResult(signalR.AccessToken);
                     };
                 }).Build();

            connectionSignalRScanItem.On<string>("ScanUpdate", (ScannedItemName) =>
            {
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
                            Message = "The list has been changed"
                        },
                        BackgroundColor = Color.FromHex("#63615E"),
                        Duration = TimeSpan.FromSeconds(10),
                        Actions = new[] { actions }
                    };
                    this.DisplaySnackBarAsync(options);

/*                    Task.Run(async () =>
                    {
                        await this.DisplaySnackBarAsync(options);
                    });*/
                }
            });

            connectionSignalRScanItem.StartAsync();
        }


        private async Task refreshAsync()
        {
            IUserDialogs userDialogs = UserDialogs.Instance;
            using (userDialogs.Loading("Loading...", null, null, true, MaskType.Black))
            {
                await Task.Delay(3);
                initPage();
            }
        }


    }
}