using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using LoginViewSample.Core.Services;


namespace LoginViewSample.Core.ViewModels.AddImageViewModel
{
    public class AddImageBase : INotifyPropertyChanged
	{
		public INavigationService NavigationService { get; private set; }

		private string _title;
		public string Title
		{
			get { return _title; }
			set { SetPropertyNew(ref _title, value); }
		}


		public AddImageBase(INavigationService navigationService)
		{
			NavigationService = navigationService;
		}


		public bool SetPropertyNew<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
		{
			if (Equals(storage, value))
				return false;

			storage = value;
			OnPropertyChanged(propertyName);
			return true;
		}


		public event PropertyChangedEventHandler PropertyChanged;

		public virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


	}
}