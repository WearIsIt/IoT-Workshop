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
    public partial class WishListPage : ContentPage
    {
        public WishListPage(string sourceParam)
        {
            source = sourceParam;
            ScannerPage.popup = false;
            InitializeComponent();
        }


        public WishListViewModel ViewModel { get; set; }

        private string source;


        void OnCollectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string currItemName = (e.CurrentSelection.FirstOrDefault() as ItemToShow)?.Name;
            ViewModel.NavigateToItemPageAsync(currItemName, source);
        }


        protected override void OnAppearing()
        {
            ViewModel = new WishListViewModel(App.NavigationService);
            BindingContext = ViewModel;
        }


    }
}