using System;
using System.Collections.Generic;
using Xamvvm;
using LoginViewSample.Core.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LoginViewSample.Core.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TagsPage : ContentPage, IBasePage<TagsModel>
	{
		public TagsPage(Tuple<string, string[]> tags)
		{
			ViewModel= new TagsModel(App.NavigationService, tags);
			BindingContext = ViewModel;
			ScannerPage.popup = false;
			Resources = new ResourceDictionary();
			Resources.Add("TagValidatorFactory", new Func<string, object>(
				(arg) => (BindingContext as TagsModel)?.ValidateAndReturnAsync(arg)));

			InitializeComponent();
		}


		public TagsPage(Tuple<string[], string[]> tags)
		{
			ViewModel = new TagsModel(App.NavigationService, tags);
			BindingContext = ViewModel;
			ScannerPage.popup = false;
			Resources = new ResourceDictionary();
			Resources.Add("TagValidatorFactory", new Func<string, object>(
				(arg) => (BindingContext as TagsModel)?.ValidateAndReturnAsync(arg)));

			InitializeComponent();
		}


		void RefButtonClicked(object sender, EventArgs e)
        {
			// Upload tags to tags table
			ViewModel.uploadTagToTagsTable();
			ViewModel.navigateToMainAsync();
        }

		public TagsModel ViewModel { get; }


	}
}