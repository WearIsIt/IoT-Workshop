using System;
using System.Collections.Generic;
using System.Text;
using Acr.UserDialogs;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Plugin.Media.Abstractions;
using Plugin.Permissions.Abstractions;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using LoginViewSample.Core.Services;
using LoginViewSample.Core.Settings;
using System.Windows.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;

// based on https://github.com/mecvillarina/XFObjectDetection

namespace LoginViewSample.Core.ViewModels.AddImageViewModel
{
    public class AddImageMain : AddImageBase
	{
		private IMedia _mediaPlugin;
		private IPermissions _permissionsPlugin;
		private IUserDialogs _userDialogs;
		string[] arrTmp;
		string itemName;
		MediaFile fileImageToUpload;
		private string source;

		private bool _EditButton;
		public bool EditButton
		{
			get { return _EditButton; }
			set { _EditButton = value; OnPropertyChanged(); }
		}

		public AddImageMain(INavigationService navigationService, IMedia mediaPlugin, IPermissions permissionsPlugin, IUserDialogs userDialogs, Tuple<string, string> tup)
			: base(navigationService)
		{
			_mediaPlugin = mediaPlugin;
			_permissionsPlugin = permissionsPlugin;
			_userDialogs = userDialogs;
			itemName = tup.Item1;
			source = tup.Item2;

			EditButton = false;
			this.TakePictureCommand = new Command(async () => await OnTakePictureAsync());
			this.PickPictureCommand = new Command(async () => await OnPickPictureAsync());
		}


		private ImageSource _imageSource;
		public ImageSource ImageSource
		{
			get => _imageSource;
			set
			{
				SetPropertyNew(ref _imageSource, value);
				this.IsImagePlaceholderVisible = value == null;
			}
		}


		private string _resultText = string.Empty;
		public string ResultText
		{
			get => _resultText;
			set => SetPropertyNew(ref _resultText, value);
		}
	

		private bool _isImagePlaceholderVisible = true;
		public bool IsImagePlaceholderVisible
		{
			get => _isImagePlaceholderVisible;
			set => SetPropertyNew(ref _isImagePlaceholderVisible, value);
		}


		public static IDictionary<string, HashSet<string>> clothesDict = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

		public static void AddToClothesDict()
		{
			if (clothesDict.Count != 0) return;

			clothesDict.Add("Colors", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "white", "yellow", "blue", "red", "green",
				"beige", "black", "brown", "silver", "purple", "navy", "navy blue", "gray", "grey",
				"orange", "coral", "lime", "pink", "olive", "magenta", "plum", "gold", "bourdeaux",
				"turquoise", "light blue", "violet", "lavender", "peach", "mint", "colorful",
				"ivory", "multicolor"});

			clothesDict.Add("Type", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "backpack", "bag", "handbag", "bikini", "bikinis",
				"top", "blouse", "blazer", "cardigan", "hat", "cap", "coat", "crop top", "dress", "flip flops",
				"flip flop", "hoodies", "jacket", "jogger", "jumper", "jumpsuit", "leggings", "kimonos",
				"lingerie sets", "lingerie", "loungewear sets", "nighties",
				"nightwear set", "nightwear", "playsuits", "polo shirt", "purse", "pyjama bottoms",
				"pyjamas", "pyjama top", "sandals", "shapewear", "shell top", "shirt", "t-shirts",
				"overall", "overall dress", "sweater dress", "tank dress", "t-shirt dress", "wrap dress",
				"shoes", "shorts", "skirt", "sliders", "slippers", "slips", "socks", "shirts",
				"sports bras", "suit trousers", "sweatshirts", "sweatshirt", "swim briefs", "swimsuit", "tankinis",
				"tank top", "tights", "tracksuits", "trainers", "trousers", "tunic",
				"t-shirt", "underwear set" ,"vest", "suit", "suits", "tartan", "scarf",
				"board shorts", "boilersuit", "boxer", "brief", "chinos", "compression tights",
				"co-ords", "dressing gown", "dungarees", "fleece", "gilets", "jeans", "jeans jacket",
				"loungewear set", "onesies","running tights", "suit jacket", "swim shorts",
				"trunk", "waistcoat", "boots", "pants"});

			clothesDict.Add("Gender", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "woman", "women", "man", "men", "girl", "girls", "boy",
				"boys", "kid", "kids", "baby", "toddler", "teen", "teenager", "teenagers"});

			clothesDict.Add("Pattern", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "checkered", "striped", "spotted", "plaid",
				"animal print", "geometric", "floral", "camouflage", "tie dye", "gingham", "houndstooth",
				"herringbone", "paisley", "patchwork", "polka dot", "batik", "colorblock", "fair isle",
				"chevron", "check", "argyle", "diamond", "solid"});

			clothesDict.Add("Length", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "short", "knee", "knee length", "midi",
				"long", "maxi", "mini", "low", "sleeve", "sleeve length", "sleeveless", "half sleeve",
				"3/4 sleeve", "long sleeve", "neckline", "boat neck", "collar", "collared", "cowl neck", "crew neck",
				"halter", "high neck", "mock neck", "off the shoulder", "one shoulder", "round neck",
				"scoop neck", "square neck", "v-neck", "turtleneck"});

			clothesDict.Add("Occasion", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "business", "casual", "formal", "party",
				"travel", "wedding", "work", "workwear"});

			clothesDict.Add("Season", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "fall", "spring", "summer", "winter", "autumn" });

			clothesDict.Add("Brand", new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "Nike", "Adidas", "Chanel", "Microsoft", "Prada", "Dior", 
				"Puma", "Michael Kors", "Under Armour", "Burberry", "Calvin Klein", "Lacoste", "Tommy Hilfiger",
				"Ray Ban", "Louis Vuitton", "Kappa", "Columbia", "Guess", "Reebok", "Vans", "Havaianas",
				"Hugo Boss", "Armani", "The North Face", "Skechers", "Hollister", "Timberland", "Fila",
				"Quicksilver", "Lee Cooper"});
		}


		public static IDictionary<string, string> mappedTagsDict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		
		public static void InitSmallDict()
		{
			if (mappedTagsDict.Count != 0) return;

			mappedTagsDict.Add("grey", "gray");
			mappedTagsDict.Add("navy", "blue");
			mappedTagsDict.Add("navy blue", "blue");
			mappedTagsDict.Add("coral", "orange");
			mappedTagsDict.Add("multicolor", "colorful");
			mappedTagsDict.Add("ivory", "white");
			mappedTagsDict.Add("magenta", "pink");
			mappedTagsDict.Add("lavender", "purple");
			mappedTagsDict.Add("plum", "purple");
			mappedTagsDict.Add("olive", "green");
			mappedTagsDict.Add("mint", "green");

			mappedTagsDict.Add("bikinis", "bikini");
			mappedTagsDict.Add("flip flops", "flip flop");
			mappedTagsDict.Add("playsuits", "playsuit");
			mappedTagsDict.Add("tracksuits", "tracksuit");
			mappedTagsDict.Add("sweatshirts", "sweatshirt");

			mappedTagsDict.Add("t-shirts", "t-shirt");
			mappedTagsDict.Add("shirts", "shirt");
			mappedTagsDict.Add("suits", "suit");
			mappedTagsDict.Add("running tights", "tights");
			mappedTagsDict.Add("compression tights", "tights");

			mappedTagsDict.Add("woman", "women");
			mappedTagsDict.Add("man", "men");
			mappedTagsDict.Add("girl", "women");
			mappedTagsDict.Add("girls", "women");
			mappedTagsDict.Add("boy", "men");
			mappedTagsDict.Add("boys", "men");
			mappedTagsDict.Add("kid", "kids");
			mappedTagsDict.Add("toddler", "baby");
			mappedTagsDict.Add("teen", "teenager");
			mappedTagsDict.Add("teenagers", "teenager");

			mappedTagsDict.Add("fall", "autumn");
			mappedTagsDict.Add("work", "workwear");

			mappedTagsDict.Add("tank dress", "dress");
			mappedTagsDict.Add("overall dress", "dress");
			mappedTagsDict.Add("sweater dress", "dress");
			mappedTagsDict.Add("t-shirt dress", "dress");
			mappedTagsDict.Add("wrap dress", "dress");
			mappedTagsDict.Add("dungarees", "overall");
			mappedTagsDict.Add("lingerie sets", "lingerie");

			mappedTagsDict.Add("pyjama bottoms", "pyjamas");
			mappedTagsDict.Add("pyjama top", "pyjamas");
			mappedTagsDict.Add("slips", "pyjamas");
			mappedTagsDict.Add("nighties", "pyjamas");
			mappedTagsDict.Add("nightwear set", "pyjamas");
			mappedTagsDict.Add("nightwear", "pyjamas");
			mappedTagsDict.Add("swim briefs", "swimsuit");
			mappedTagsDict.Add("swim shorts", "swimsuit");
			mappedTagsDict.Add("jeans jacket", "jacket");
			mappedTagsDict.Add("suit jacket", "jacket");

			mappedTagsDict.Add("backpack", "bag");
			mappedTagsDict.Add("handbag", "bag");
			mappedTagsDict.Add("purse", "bag");
			mappedTagsDict.Add("cap", "hat");
			mappedTagsDict.Add("collar", "collared");
		}


		public ICommand TakePictureCommand { get; private set; }
		public ICommand PickPictureCommand { get; private set; }
		public ICommand EditTagsCommand { get; private set; }


		public async Task EditTagsAsync()
		{
			UploadToBlob controller = new UploadToBlob();
			await controller.UploadToAzureAsync(fileImageToUpload, itemName);

			string[] itemNameAndSource = { itemName, source };
			Tuple<string[], string[]> tup = new Tuple<string[], string[]>(itemNameAndSource, arrTmp);	
			await App.NavigationService.NavigateAsync(PageNames.TagsPage, tup);
		}


		private async Task OnTakePictureAsync()
		{
			var cameraStatus = await _permissionsPlugin.CheckPermissionStatusAsync(Permission.Camera);
			var storageStatus = await _permissionsPlugin.CheckPermissionStatusAsync(Permission.Storage);

			if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
			{
				var statusDict = await _permissionsPlugin.RequestPermissionsAsync(new Permission[] { Permission.Camera, Permission.Storage });
				cameraStatus = statusDict[Permission.Camera];
				storageStatus = statusDict[Permission.Storage];
			}

			if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
			{
				var file = await _mediaPlugin.TakePhotoAsync(new StoreCameraMediaOptions
				{
					PhotoSize = PhotoSize.Medium
				});

				if (file != null)
				{
					fileImageToUpload = file;
					_userDialogs.ShowLoading();

					this.ImageSource = ImageSource.FromStream(() =>
					{
						var stream = file.GetStream();
						return stream;
					});

					var imageAnalysis = await MakeAnalysisRequest(Constants.ComputerVisionUriBase, Constants.ComputerVisionSubscriptionKey1, file.GetStream());

					if (imageAnalysis != null)
					{
						var strBuilder = new StringBuilder();
						ShowTags(strBuilder, imageAnalysis);
						ResultText = strBuilder.ToString();
						this.arrTmp = ResultText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
						ResultText = string.Join(Environment.NewLine, arrTmp.Distinct());
					}
					else
					{
						ResultText = string.Empty;
					}

					_userDialogs.HideLoading();
					EditButton = true;
				}
			}
			else
			{
				await _userDialogs.AlertAsync("Unable to take photos", "Permissions Denied", "Ok");
			}
		}


		private async Task OnPickPictureAsync()
		{
			var cameraStatus = await _permissionsPlugin.CheckPermissionStatusAsync(Permission.Camera);
			var storageStatus = await _permissionsPlugin.CheckPermissionStatusAsync(Permission.Storage);

			if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
			{
				var statusDict = await _permissionsPlugin.RequestPermissionsAsync(new Permission[] { Permission.Camera, Permission.Storage });
				cameraStatus = statusDict[Permission.Camera];
				storageStatus = statusDict[Permission.Storage];
			}

			if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
			{
				var file = await _mediaPlugin.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
				{
					PhotoSize = PhotoSize.Medium
				});

				if (file != null)
				{
					fileImageToUpload = file;
					_userDialogs.ShowLoading();

					this.ImageSource = ImageSource.FromStream(() =>
					{
						var stream = file.GetStream();
						return stream;
					});

					var imageAnalysis = await MakeAnalysisRequest(Constants.ComputerVisionUriBase, Constants.ComputerVisionSubscriptionKey1, file.GetStream());

					if (imageAnalysis != null)
					{
						var strBuilder = new StringBuilder();
						ShowTags(strBuilder, imageAnalysis);
						ResultText = strBuilder.ToString();

						this.arrTmp = ResultText.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
						ResultText = string.Join(Environment.NewLine, arrTmp.Distinct());
					}
					else
					{
						ResultText = string.Empty;
					}
					_userDialogs.HideLoading();
					EditButton = true;
				}				
			}
			else
			{
				await _userDialogs.AlertAsync("Unable to pick photos", "Permissions Denied", "Ok");
			}
		}


		public void ShowTags(StringBuilder strBuilder, ImageAnalysis imageAnalysis)
		{
			strBuilder.AppendLine("Tags:");
			string currTag = "";

			if (imageAnalysis.Description.Captions.Count > 0)
			{
				string[] captionarr = imageAnalysis.Description.Captions[0].Text.Split(' ');
				for (int i = 0; i < captionarr.Length; i++)
				{
					currTag = captionarr[i].ToLower();
					if (CheckIfTagInDict(currTag))
					{
						InsertTagToStringBuilder(currTag, strBuilder);
					}
				}
			}
			
			string tmp = strBuilder.ToString();
			for (int i = 0; i < imageAnalysis.Color.DominantColors.Count; i++)
			{
				currTag = imageAnalysis.Color.DominantColors[i].ToLower();
				if (currTag == imageAnalysis.Color.DominantColorBackground.ToLower())
				{
					continue;
				}
				if (clothesDict["Colors"].Contains(currTag))
				{
					InsertTagToStringBuilder(currTag, strBuilder);
				}
			}

			strBuilder.AppendLine(string.Join(Environment.NewLine, imageAnalysis.Color.DominantColorForeground.ToLower()));

			for (int i = 0; i < imageAnalysis.Description.Tags.Count; i++)
			{
				currTag = imageAnalysis.Description.Tags[i].ToLower();
				if (CheckIfTagInDict(currTag))
				{
					InsertTagToStringBuilder(currTag, strBuilder);
				}
			}

			for (int i = 0; i < imageAnalysis.Tags.Count; i++)
			{
				if (imageAnalysis.Tags[i].Confidence < 0.95)
				{
					continue;
				}
				currTag = imageAnalysis.Tags[i].Name.ToLower();
				if (CheckIfTagInDict(currTag))
				{
					InsertTagToStringBuilder(currTag, strBuilder);
				}
			}

			if (imageAnalysis.Brands.Count != 0)
			{
				currTag = imageAnalysis.Brands[0].Name;
				if (CheckIfTagInDict(currTag))
				{
					strBuilder.AppendLine(string.Join(Environment.NewLine, currTag));
				}			
			}
		}


		public void InsertTagToStringBuilder(string currTag, StringBuilder strBuilder)
		{
			if (mappedTagsDict.ContainsKey(currTag))
			{
				strBuilder.AppendLine(string.Join(Environment.NewLine, mappedTagsDict[currTag]));
			}
			else
			{
				strBuilder.AppendLine(string.Join(Environment.NewLine, currTag));
			}
		}


		public Boolean CheckIfTagInDict(string currTag)
		{
			foreach (var key in clothesDict.Keys)
			{
				if (clothesDict[key].Contains(currTag))
				{
					return true;
				}
			}
			return false;
		}


		public async Task<ImageAnalysis> MakeAnalysisRequest(string uriBase, string subscriptionKey, Stream imageStream)
		{
			try
			{
				var computerVision = new ComputerVisionClient(
					new ApiKeyServiceClientCredentials(subscriptionKey),
					new System.Net.Http.DelegatingHandler[] { });

				computerVision.Endpoint = "https://projectcomputervision0.cognitiveservices.azure.com/";

				var features = new List<VisualFeatureTypes>()
					{
						VisualFeatureTypes.Categories, VisualFeatureTypes.Description,
						VisualFeatureTypes.Faces, VisualFeatureTypes.ImageType,
						VisualFeatureTypes.Tags, VisualFeatureTypes.Color, VisualFeatureTypes.Brands
					};

				ImageAnalysis analysis = await computerVision.AnalyzeImageInStreamAsync(imageStream, features);
				return analysis;
			}
			catch (Exception ex)
			{
				await _userDialogs.AlertAsync(ex.Message);
			}

			return null;

		}
	}
}

