using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
namespace PhotoUploader;


public partial class MainPage : ContentPage
{
    public FileResult photoPub;
	public MainPage()
	{
		InitializeComponent();
	}
     
    
	private async void navigateTo(object sender, EventArgs e)
	{
       string destination = ((Button)sender).BindingContext.ToString();
		await Shell.Current.GoToAsync("//"+destination);
    }
}

