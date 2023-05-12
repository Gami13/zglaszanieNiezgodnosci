namespace PhotoUploader;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
	}
	public async void goBack(object sender, EventArgs e)
	{
		AppState.places.Clear();
        AppState.categories.Clear();
        AppState.subcategories.Clear();
        AppState.photos.Clear();


        await Shell.Current.GoToAsync("//MainPage");

	}
}
