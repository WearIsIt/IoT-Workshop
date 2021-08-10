using CustomerApp.Services;
using CustomerApp.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CustomerApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        public SignUpPage()
        {
            InitializeComponent();
        }

        async void OnRegisterButtonClicked(object sender, EventArgs e)
        {
            Preferences.Clear();
            Preferences.Set("password", Password.Text);
            Preferences.Set("email", Email.Text);
            // Sign up logic goes here

            var signUpSucceeded = AreDetailsValid(Email.Text.Trim(), Password.Text);
            if (signUpSucceeded==1)
            {
                App.IsUserLoggedIn = true;

                //call function app
                string ConnectionInfo = "";
                string urlSignUp = "https://customerlogin.azurewebsites.net/api/signup";
                HttpClient client = new HttpClient();
                var cr = new LoginEntity();
                cr.PartitionKey = "2";
                cr.RowKey = Email.Text.Trim().ToLower();
                cr.Password = Password.Text;

                var json = JsonConvert.SerializeObject(cr);
                using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    var post = client.PostAsync(urlSignUp, content).Result;
                    ConnectionInfo = post.Content.ReadAsStringAsync().Result;
                }

                LoginEntity entity = JsonConvert.DeserializeObject<LoginEntity>(ConnectionInfo);
                if (entity == null) // Email is already in storage
                {
                    await DisplayAlert("Alert", "User already exists", "OK");
                }

                else
                {
                    await App.NavigationService.NavigateAsync(PageNames.LoginPage);
                }
            }

            else if (signUpSucceeded == 2)
            {
                await DisplayAlert("Alert", "SignUp Failed: Email is invalid", "OK");
            }

            else // signUpSucceeded == 3
            {
                await DisplayAlert("Alert", "SignUp Failed: Password is invalid", "OK");
            }
        }


        int AreDetailsValid(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || email.Contains(" "))
            {
                return 2; // 2 indicates email is invaild
            }

            else if (string.IsNullOrWhiteSpace(password) || password.Contains(" "))
            {
                return 3; // 3 indicates password is invaild
            }

            return 1; // 1 indicates email and password are vaild
        }


    }
}
