using LoginViewSample.Core.Annotations;
using LoginViewSample.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace LoginViewSample.Core.ViewModels
{
    public class ItemLocationModel
    {
        private readonly INavigationService _navigationService;
        private string source;

        public ItemLocationModel([NotNull] INavigationService navigationService, Tuple<string, string> tup)
        {
            _navigationService = navigationService ?? throw new ArgumentNullException(nameof(navigationService));
            source = tup.Item2;
        }


        public async Task BackToMainPageAsync()
        {
            if (source == "Show")
            {
                await startNavigateToMainPageAsync(2);
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
