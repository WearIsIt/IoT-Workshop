using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using LoginViewSample.Core.ViewModels.AddImageViewModel;
using Plugin.Media;
using Plugin.Permissions;
using Acr.UserDialogs;
using LoginViewSample.Core.Services;
using Plugin.Permissions.Abstractions;
using Plugin.Media.Abstractions;

namespace LoginViewSample.Core.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AddImagePage : ContentPage
	{
		public AddImagePage(Tuple<string, string> tup)
		{
			ViewModel= new AddImageMain(App.NavigationService, CrossMedia.Current, CrossPermissions.Current,
			UserDialogs.Instance, tup);
			BindingContext = ViewModel;
			ScannerPage.popup = false;
			InitializeComponent();

			switch (Device.RuntimePlatform)
			{
				case Device.iOS:
					Padding = new Thickness(0, 20, 0, 0);
					break;
				default:
					Padding = new Thickness(0);
					break;
			}

			Visual = VisualMarker.Material;
		}


		private void Button_Clicked(object sender, EventArgs e)
        {
			ViewModel.EditTagsAsync();
		}


		public AddImageMain ViewModel { get; }

		protected override bool OnBackButtonPressed()
		{
			return false;
		}


	}
}