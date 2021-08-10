using CustomerApp.Annotations;
using CustomerApp.Services;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Bot.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Drawing.Imaging;
using Xamarin.Forms;
using System.Windows.Media.Imaging;
using System.Threading.Tasks;
using System.Net;
using System.Windows.Input;

namespace CustomerApp.ViewModels
{
    public class ItemLocationModel
    {
        private readonly INavigationService _navigationService;
        private string source;

        public ItemLocationModel([NotNull] INavigationService navigationService, Tuple<string, string> tup)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            source= tup.Item2;
        }


        public async Task BackToMainPageAsync()
        {
            if (source == "Show" || source == "WishList" || source == "TopFive")
            { 
                await startNavigateToMainPageAsync(2);
            }

            else if (source == "TagsFilter")
            {
                await startNavigateToMainPageAsync(3);
            }           
        }


        public async Task startNavigateToMainPageAsync(int num)
        {
            for (int i = 0; i < num; i++)
            {
                await _navigationService.GoBack();
            }
        }


    }
}

