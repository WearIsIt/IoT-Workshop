using CustomerApp.TagSearch;
using CustomerApp.ViewModels;
using CustomerApp.Services;
using System;
using System.Collections.Generic;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.MultiSelectListView;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChooseTagsPage : ContentPage
    {
        public ChooseTagsPage()
        {
            selectedHash = new HashSet<SelectableItem<Tag>>();
            ViewModel = new TagsCategoriesViewModel(App.NavigationService);
            BindingContext = ViewModel;
            ScannerPage.popup = false;
            InitializeComponent();           
        }


        private string source;


        public ChooseTagsPage(string sourceParam)
        {
            selectedHash = new HashSet<SelectableItem<Tag>>();
            ViewModel = new TagsCategoriesViewModel(App.NavigationService);
            BindingContext = ViewModel;
            ScannerPage.popup = false;
            InitializeComponent();
            source = sourceParam;
        }


        public TagsCategoriesViewModel ViewModel { get; }

        void OnLabelTapped(object sender, EventArgs e)
        {
            Label label = sender as Label;
            Expander expander = label.Parent.Parent.Parent as Expander;

            if (label.FontSize == Device.GetNamedSize(NamedSize.Default, label))
            {
                label.FontSize = Device.GetNamedSize(NamedSize.Large, label);
            }

            else
            {
                label.FontSize = Device.GetNamedSize(NamedSize.Default, label);
            }
            expander.ForceUpdateSize();
        }


        private void ProfileDetails_Selected(object sender, ItemTappedEventArgs e)
        {
            SelectableItem<Tag> currItem = (SelectableItem<Tag>)e.Item;
            if (selectedHash.Contains(currItem)) // currItem was removed by the user
            {
                selectedHash.Remove(currItem);
            }

            else // currItem was added by the user
            {
                selectedHash.Add(currItem);
            }
        }


        public static HashSet<SelectableItem<Tag>> selectedHash;


        [Obsolete]
        void OnClick(object sender, EventArgs e)
        {
            ToolbarItem tbi = (ToolbarItem)sender;
            ViewModel.NavigateToSearchResults(selectedHash, source);
        }


    }
}