using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LoginViewSample.Core.Views
{
    public class SelectLocationSettings : ContentPage
    {
		string messageFromBarcode = "";

		public SelectLocationSettings()
		{
			Title = "Picker ItemsSource";
			IconImageSource = "csharp.png";

			var LocationNameLabel = new Label();
			var picker = new Picker { Title = "Select a Location", TitleColor = Color.Red };
			picker.Items.Add("Storeroom");
			picker.Items.Add("Fitting Room");
			picker.Items.Add("Stand");

			picker.SelectedIndexChanged += (sender, e) =>
			{
				int selectedIndex = picker.SelectedIndex;
				if (selectedIndex != -1)
				{
					LocationNameLabel.Text = picker.Items[selectedIndex];
				}
			};

			Content = new StackLayout
			{
				Margin = new Thickness(20, 35, 20, 20),
				Children = {
					new Label { Text = "Picker - Data in Collection", FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.Center },
					picker,
					new StackLayout
					{
						Orientation = StackOrientation.Horizontal,
						Children = {
							new Label { Text = "You chose:"},
							LocationNameLabel
						}
					}
				}
			};
		}
	}
}