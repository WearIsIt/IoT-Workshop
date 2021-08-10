using Microsoft.Azure.Devices.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoginViewSample.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LoginViewSample.Core.Settings;

namespace LoginViewSample.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SelectLocationPage : ContentPage
    {
        public string messageFromQR { get; set; }
     

        public SelectLocationPage()
        {
            ScannerPage.popup = false;
            InitializeComponent();
        }


        public SelectLocationPage(string m)
        {
            ScannerPage.popup = false;          
            InitializeComponent();
            messageFromQR = m;

            string itemName = messageFromQR.Split('_')[0];
            if (!UploadToBlob.CheckIfExistsInBlob(itemName)) // There is no image 
            {
                isImageExists = false;
            }

            else
            {
                isImageExists = true;
            }
        }

        private bool isImageExists { get; set; }

        private string source;

        public SelectLocationPage(Tuple<string, string> tup)
        {
            ScannerPage.popup = false;
            InitializeComponent();
            messageFromQR = tup.Item1;
            source = tup.Item2;
            
            if (source=="Add") // If item was added for the first time- don't show "Sold" option
            {
                PickerLocation.Items.Remove("Sold");
            }

            string itemName = messageFromQR.Split('_')[0];
            if (!UploadToBlob.CheckIfExistsInBlob(itemName)) // There is no image 
            {
                isImageExists = false;               
            }

            else
            {
                isImageExists = true;
            }
        }


        public SelectLocationViewModel ViewModel { get; set; }

        void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            int selectedIndex = picker.SelectedIndex;

            if (selectedIndex != -1)
            {
                if (source!="Add")
                {         
                    if (selectedIndex != 3)
                    {
                        LocationNameLabel.Text = "Your item will be moved to " + picker.Items[selectedIndex];
                    }

                    else
                    {
                        LocationNameLabel.Text = "";
                    }
                }

                else  // source == "Add"
                {
                    LocationNameLabel.Text = "Your item will be added to " + picker.Items[selectedIndex];
                }
            }

            string mWithLocation = messageFromQR + "_" + picker.Items[selectedIndex];
            Tuple<string, string> tup = new Tuple<string, string>(mWithLocation, source);
            ViewModel = new SelectLocationViewModel(App.NavigationService, tup);
            BindingContext = ViewModel;
            DoneChoosingButton.IsEnabled = true;

            string itemName = messageFromQR.Split('_')[0];
            if (!isImageExists) // There is no image 
            {
                AddImageButton.IsEnabled = true;
            }
        }

        private void DoneChoosingButton_Clicked(object sender, EventArgs e) // done choosing button 
        {
            ViewModel.DoneChoosing();
        }
  
        private void AddImageButton_Clicked(object sender, EventArgs e) // add image button 
        {
            ViewModel.AddImage();
        }


    }
}