using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
namespace PhotoUploader;


public partial class Placeholder2 : ContentPage
{
    public FileResult photoPub;
	public Placeholder2()
	{
		InitializeComponent();
	}
     
    public async void sendFile(FileResult photo)
    {
        HttpContent stringContent = new StringContent(field0.Text);
        HttpContent stringContent2 = new StringContent(field1.Text);

        FileStream fileStream = File.Open(photo.FullPath, FileMode.Open);
        HttpContent fileStreamContent = new StreamContent(fileStream);


    
        fileStreamContent.Headers.Add("Content-Type", photo.ContentType);


        using (var client = new HttpClient())
        using (var formData = new MultipartFormDataContent())
        {
            // Add the HttpContent objects to the form data
            formData.Add(stringContent, "field0");
            formData.Add(stringContent2, "field1");
            formData.Add(fileStreamContent, "photo", photo.FileName);
   
            var response = await client.PostAsync("http://localhost:3000/", formData);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Failed", "Failed to send to server", "OK");
            }         
        }
    }

    private async void chooseImage(object sender, EventArgs e)
	{
		Debug.WriteLine("choosing Image");
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
        test.Source = photo.FullPath;
        photoPub = photo;
	}
    private void submit(object sender, EventArgs e)
    {
        sendFile(photoPub);
    }
	private async void takePicture(object sender, EventArgs e)
	{
        Debug.WriteLine("Taking Image");
        if (MediaPicker.Default.IsCaptureSupported)
        {
            try
            {
                FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    using Stream sourceStream = await photo.OpenReadAsync();
                    using FileStream localFileStream = File.OpenWrite(localFilePath);

                    await sourceStream.CopyToAsync(localFileStream);
                    test.Source = photo.FullPath;
                    photoPub = photo;

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine("Failed to take picture");
            }
        }
    }
}

