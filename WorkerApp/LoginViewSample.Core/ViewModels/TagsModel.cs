using LoginViewSample.Core.Annotations;
using LoginViewSample.Core.Services;
using LoginViewSample.Core.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;
using Xamvvm;
using System.Threading.Tasks;
using LoginViewSample.Core.ViewModels.AddImageViewModel;

// based on https://github.com/daniel-luberda/DLToolkit.Forms.Controls/tree/master/TagEntryView

namespace LoginViewSample.Core.ViewModels
{
	public class TagsModel : BasePageModel
	{
		private string nameOfItem;
		private readonly INavigationService _navigationService;

		public TagsModel()
		{
			RemoveTagCommand = new BaseCommand<TagItem>((arg) => RemoveTag(arg));
		}


		public TagsModel([NotNull] INavigationService navigationService, Tuple<string, string[]> tup)
		{
			_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			nameOfItem = tup.Item1;
			RemoveTagCommand = new BaseCommand<TagItem>((arg) => RemoveTag(arg));
			ReloadTags(tup.Item2);
		}


		private string source;

		public TagsModel([NotNull] INavigationService navigationService, Tuple<string[], string[]> tup)
		{
			_navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
			nameOfItem = tup.Item1[0];
			RemoveTagCommand = new BaseCommand<TagItem>((arg) => RemoveTag(arg));
			ReloadTags(tup.Item2);
			source = tup.Item1[1];
		}


		public void ReloadTags(string[] words)
		{
			ObservableCollection<TagItem> tags = new ObservableCollection<TagItem>();
			if (words != null && words.Length > 0)
			{
				words.Distinct().ToList().ForEach(i => tags.Add(new TagItem("#" + i)));
				tags.RemoveAt(0);
				if (tags.Count > 0)
				{
					tags.RemoveAt(tags.Count() - 1);
				}
			}
			Items = tags;
		}


		public void RemoveTag(TagItem tagItem)
		{
			if (tagItem == null)
				return;

			Items.Remove(tagItem);
		}

		public TagItem ValidateAndReturnAsync(string tag)
		{
			if (string.IsNullOrWhiteSpace(tag))
				return null;

			// check if tag is in clothesDict
			bool flag = false;
			bool IfBrand = false;

			if (AddImageMain.mappedTagsDict.ContainsKey(tag)) 
            {
				return null;
			}
			
			foreach (var currKey in AddImageMain.clothesDict.Keys)
			{
				if (AddImageMain.clothesDict[currKey].Contains(tag))
				{
					flag = true;
					if (currKey == "Brand")
					{
						IfBrand = true;
					}
					break;
				}
			}

			if (!flag) // tag is not in clothesDict
			{
				return null;
			}
			
            var tagString = tag.StartsWith("#") ? tag : "#" + tag;

			if (Items.Any(v => v.Name.Equals(tagString, StringComparison.OrdinalIgnoreCase)))
				return null;

			if (IfBrand) // If tag is a name of a brand- don't convert to lower case
            {
				tagString = tagString.Substring(0,1) + tagString.Substring(1,1).ToUpper() + tagString.Substring(2).ToLower();
				return new TagItem(tagString);
			}

			return new TagItem(tagString.ToLower());
		}


		public IBaseCommand RemoveTagCommand
		{
			get { return GetField<IBaseCommand>(); }
			set { SetField(value); }
		}

		public ObservableCollection<TagItem> Items
		{
			get { return GetField<ObservableCollection<TagItem>>(); }
			set { SetField(value); }
		}

		public async Task navigateToMainAsync()
		{
			if (source=="Add" || source=="Change" )
            {
				await startNavigateToMainPageAsync(4);
            }

			else if (source == "Image")
			{
				await startNavigateToMainPageAsync(3);
			}
		}


		public async Task startNavigateToMainPageAsync(int num)
        {
			for (int i=0;i<num;i++)
            {
				await _navigationService.GoBack();
			}
        }


		public void uploadTagToTagsTable()
        {
			string ItemsString="";
			string tmp;
			for (int i=0; i<Items.Count ;i++)
            {
				tmp = Items[i].Name;
				tmp=tmp.Substring(1, tmp.Length - 1);

				if (i==0)
                {
					ItemsString += tmp;
				}
				else
                {
					ItemsString += "_"+tmp;
				}
			}

			// Call function app
			string url = "https://uploadtagstostroage.azurewebsites.net/api/updateTags/" + nameOfItem + "/" + ItemsString;
			HttpClient client = new HttpClient();
			var response = client.GetAsync(url).Result;
		}


		public class TagItem : BaseModel
		{
			public TagItem(String s)
			{
				name = s;
			}

			string name;
			public string Name
			{
				get { return name; }
				set { SetField(ref name, value); }
			}
		}
	

	}
}
