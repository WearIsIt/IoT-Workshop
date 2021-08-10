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
using CustomerApp.Services;
using Xamarin.Forms.Xaml;
using CustomerApp.ViewModels;
using System.Net.Http;
using Acr.UserDialogs;

// based on https://blog.verslu.is/xamarin/xamarin-forms-xamarin/scanning-generating-barcodes-zxing/

namespace CustomerApp.Views
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
                string messageJson = result.Text;

                if (sourceFromMainPage == "Show")
                {
                    string[] tokens = messageJson.Split('_');
                    string item = tokens[0];
                    string size = tokens[1];     
                    bool popUpResult = await DisplayAlert("Scanned result", "The item is: " + item + "\nSize: " + size ,"OK", "Cancel");

                    if (popUpResult)
                    {
                        Tuple<string, string> tup = new Tuple<string, string>(item, "Show");

                        // Update item's scan counter
                        string urlUpdateCounters = "https://scancountfunctionapp.azurewebsites.net/api/update-item-counter/" + item;
                        HttpClient client = new HttpClient();
                        var response = client.GetAsync(urlUpdateCounters).Result;

                        await App.NavigationService.NavigateAsyncWithLoad(PageNames.ItemLocationPage, tup, 1800);
                    }

                    else
                    {
                        popup = false;
                    }
                }
            });
        }


    }
}