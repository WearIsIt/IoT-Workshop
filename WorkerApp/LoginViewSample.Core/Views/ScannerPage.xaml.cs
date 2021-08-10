using Microsoft.Azure.Cosmos.Table;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using LoginViewSample.Core.Services;
using Xamarin.Forms.Xaml;
using LoginViewSample.Core.ViewModels;
using System.Net.Http;
using LoginViewSample.Core.Settings;

// based on https://blog.verslu.is/xamarin/xamarin-forms-xamarin/scanning-generating-barcodes-zxing/

namespace LoginViewSample.Core.Views
{
 //   [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        const string DeviceConnectionString = "HostName=ProjectIotHub0.azure-devices.net;DeviceId=BarcodeDevice0;SharedAccessKey=Fhs61+LLPmQn+ebqSk7mbnkCbNxRyoDfk7sD0OfG2/A=";
        static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        public string sourceFromMainPage { get; set; }
        public static bool popup = false;


        public ScannerPage()
        {
            InitializeComponent();
            popup = false;
        }

        public ScannerPage(string source)
        {
            InitializeComponent();
            sourceFromMainPage = source;
            popup = false;
        }


        void ZXingScannerView_OnScanResult(ZXing.Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                Page currentPage = Navigation.NavigationStack.LastOrDefault();
                if (currentPage != this || popup) return;
                popup = true;
                scanResultText.Text = result.Text;
                string messageJson = result.Text;

                if (sourceFromMainPage == "Add")
                {
                    bool popUpResult = await DisplayAlert("Scanned result", "The barcode's text is " + result.Text, "OK", "Cancel");
                    if (popUpResult)
                    {
                        Tuple<string, string> tup = new Tuple<string, string>(messageJson, "Add");
                        await App.NavigationService.NavigateAsync(PageNames.SelectLocationPage, tup);
                    }

                    else
                    {
                        popup = false;

                    }
                }

                else if (sourceFromMainPage == "Show")
                {
                    string[] tokens = messageJson.Split('_');
                    string item = tokens[0];
                    string size = tokens[1];

                    bool popUpResult = await DisplayAlert("Scanned result", "The item is: " + item + "\nSize: " + size, "OK", "Cancel");
                    if (popUpResult)
                    {
                        Tuple<string, string> tup = new Tuple<string, string>(item, "Show");
                        await App.NavigationService.NavigateAsync(PageNames.ItemLocationPage, tup);
                    }

                    else
                    {
                        popup = false; // Scan something else
                    }
                }

                else if (sourceFromMainPage == "Image")
                {
                    // Check if item in items Table (project table)
                    string[] tokens = messageJson.Split('_');
                    string id = tokens[0] + "_" + tokens[2];

                    // Check if itemNmae has already image in images blob
                    string itemName = tokens[0];
                    bool exist = UploadToBlob.CheckIfExistsInBlob(itemName);
                    
                    string urlCheckIfItemExists = "https://updateprojecttablefunctionapp.azurewebsites.net/api/check-if-item-exists/" + id;
                    HttpClient client = new HttpClient();
                    var response = client.GetAsync(urlCheckIfItemExists).Result;
                    var resultFromHttp = response.Content.ReadAsStringAsync().Result;

                    if (resultFromHttp == "false") // Item was not in items table
                    {
                        bool popUpResult = await DisplayAlert("Scanned result", "The item does not exist", "Take me back", "OK");
                        if (popUpResult)  // "Take me back" was chosen
                        {
                            await App.NavigationService.GoBack(); // Go back to main page
                        }

                        else // "OK" was chosen
                        {
                            popup = false; // Scan something else
                        }
                    }

                    else if (exist) // Item is in items table but has an image
                    {
                        bool popUpResult = await DisplayAlert("Scanned result", "The item already has an image", "Take me back", "OK");
                        if (popUpResult)  // "Take me back" was chosen
                        {
                            await App.NavigationService.GoBack(); // Go back to main page
                        }

                        else // "OK" was chosen
                        {
                            popup = false; // Scan something else
                        }
                    }

                    else // Item is in items table and does not have an image
                    {
                        bool popUpResult = await DisplayAlert("Scanned result", "The barcode's text is " + result.Text, "OK", "Cancel");
                        if (popUpResult)
                        {
                            Tuple<string, string> tup = new Tuple<string, string>(itemName, "Image");
                            await App.NavigationService.NavigateAsync(PageNames.AddImagePage, tup);
                        }

                        else
                        {
                            popup = false; // Scan something else
                        }
                    }
                }

                else if (sourceFromMainPage == "Change") 
                {
                    // Check If Item exists (messgaeJson)
                    string[] tokens = new string[3];
                    tokens = messageJson.Split('_');

                    string PartitionKey, RowKey;
                    PartitionKey = tokens[0];
                    RowKey = tokens[2];
                    string id = PartitionKey + "_" + RowKey;

                    string urlCheckIfItemExists = "https://updateprojecttablefunctionapp.azurewebsites.net/api/check-if-item-exists/" + id;
                    HttpClient client = new HttpClient();
                    var response = client.GetAsync(urlCheckIfItemExists).Result;
                    var resultFromHttp = response.Content.ReadAsStringAsync().Result;

                    if (resultFromHttp == "true")
                    {
                        bool popUpResult = await DisplayAlert("Scanned result", "The barcode's text is " + result.Text, "OK", "Cancel");
                        if (popUpResult)
                        {
                            Tuple<string, string> tup = new Tuple<string, string>(messageJson, "Change");
                            await App.NavigationService.NavigateAsync(PageNames.SelectLocationPage, tup);
                        }

                        else
                        {
                            popup = false;
                        }                           
                    }

                    else // resultFromHttp = "false"
                    {
                        bool popUpResult= await DisplayAlert("Scanned result", "The item does not exist", "Take me back", "OK");
                        if (popUpResult)  // "Take me back" was chosen
                        {
                            await App.NavigationService.GoBack(); // Go back to main page                                                         
                        }

                        else // "OK" was chosen
                        {
                            popup = false; // Scan something else
                        }                         
                    }
                }
            });
        }


    }
}