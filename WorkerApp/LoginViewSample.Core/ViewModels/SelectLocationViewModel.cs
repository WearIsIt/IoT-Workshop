using LoginViewSample.Core.Services;
using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Net.Http;

namespace LoginViewSample.Core.ViewModels
{
    public class SelectLocationViewModel
    {
        private readonly INavigationService _navigationService;
        const string DeviceConnectionString = "HostName=ProjectIotHub0.azure-devices.net;DeviceId=BarcodeDevice0;SharedAccessKey=Fhs61+LLPmQn+ebqSk7mbnkCbNxRyoDfk7sD0OfG2/A=";
        static readonly DeviceClient Client = DeviceClient.CreateFromConnectionString(DeviceConnectionString);
        public string messageFromQRModel { get; set; }
       
        public SelectLocationViewModel(INavigationService navigationService)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
        }


        public SelectLocationViewModel(INavigationService navigationService, string m)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            messageFromQRModel = m;
        }


        private string source;

        public SelectLocationViewModel(INavigationService navigationService, Tuple<string, string> tup)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            messageFromQRModel = tup.Item1;
            source = tup.Item2;
        }


        public void DoneChoosing()
        {
            Message message = new Message(Encoding.ASCII.GetBytes(messageFromQRModel)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            Client.SendEventAsync(message).Wait();

            // call signalr
            string itemName = messageFromQRModel.Split('_')[0];
            CallSignalRAfterLocationUpdated(itemName);
        
            _navigationService.GoBack();
            _navigationService.GoBack();
        }


        public void AddImage()
        {
            Message message = new Message(Encoding.ASCII.GetBytes(messageFromQRModel)) { ContentType = "application/json", ContentEncoding = "utf-8" };
            Client.SendEventAsync(message).Wait();

            // Prepare Item name staring
            string itemName = messageFromQRModel.Split('_')[0];

            // Call signalR
            CallSignalRAfterLocationUpdated(itemName);

            Tuple<string, string> tup = new Tuple<string, string>(itemName, source);
            _navigationService.NavigateAsync(PageNames.AddImagePage, tup);
        }

        private void CallSignalRAfterLocationUpdated(string itemName)
        {           
            string urlCallSignalR = "https://locationchangedsignalr.azurewebsites.net/api/location-changed-signalR/" + itemName;
            HttpClient client = new HttpClient();
            var response = client.GetAsync(urlCallSignalR).Result;
        }


    }
}
