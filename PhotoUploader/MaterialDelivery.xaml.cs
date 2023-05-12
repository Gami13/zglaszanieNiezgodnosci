using System;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace PhotoUploader;


public partial class MaterialDelivery : ContentPage
{

    public ObservableCollection<Element> elementss = new ObservableCollection<Element>();
    private int tempId = 0;

    public async void updatePlaces()
    {

        try
        {
            var response = await AppState.client.GetAsync("http://localhost:3000/place");

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");

            }

            var result = JsonConvert.DeserializeObject<AppState.Places>(await response.Content.ReadAsStringAsync());
            foreach (var a in result.places)
            {

                AppState.places.Add(a);

            }
            placeUI.ItemsSource = AppState.places;
        }
        catch
        {
            await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");

        }







    }

    public async void updateCategories()
    {
        try
        {
            var response = await AppState.client.GetAsync("http://localhost:3000/category");

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");

            }

            var result = JsonConvert.DeserializeObject<AppState.Categories>(await response.Content.ReadAsStringAsync());

            foreach (var a in result.categories)
            {

                AppState.categories.Add(a);

            }
            categoriesUI.ItemsSource = AppState.categories;
        }
        catch
        {
            await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");

        }

    }
    public async void updateSubcategories(object sender, EventArgs e)
    {
        try
        {
            sender = ((Picker)sender).SelectedItem.ToString();
            var URL = "http://localhost:3000/subcategory?category=" + sender;
            var response = await AppState.client.GetAsync(URL);

            if (!response.IsSuccessStatusCode)
            {
                await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");

            }

            var result = JsonConvert.DeserializeObject<AppState.Subcategories>(await response.Content.ReadAsStringAsync());
            AppState.subcategories.Clear();

            foreach (var a in result.subcategories)
            {

                AppState.subcategories.Add(a);

            }
            subcategoriesUI.ItemsSource = AppState.subcategories;
            subcategoriesUI.IsEnabled = true;
        }
        catch
        {
            await DisplayAlert("Błąd", "Nie można nawiązać polączenia z serwerem", "OK");
        }

    }
    public class Element
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public string Subcategory { get; set; }
        public int Amount { get; set; }





    }

    public MaterialDelivery()
    {
        InitializeComponent();
        updatePlaces();
        updateCategories();

        elements.ItemsSource = elementss;
        photosUI.ItemsSource = AppState.photos;


    }
    public void removePhoto(object sender, EventArgs e)
    {
        string photoPath = ((Button)sender).BindingContext.ToString();
        AppState.photos.Remove(AppState.photos.Where(x => x.FullPath == photoPath).FirstOrDefault());
    }

    public async void sendPhoto(object sender, EventArgs e)
    {
        List<Element> elementsFin = new List<Element>();
        foreach (var x in elementss)
        {
            var elem = new Element();
            elem.Amount = x.Amount;
            elem.Id = x.Id;
            elem.Category = x.Category;
            elem.Subcategory = x.Subcategory;
            elementsFin.Add(elem);
        }
        var num = projectNumber.Text;
        var place = placeUI.SelectedItem;
        var desc = descAndComment.Text;
        var elementsFinFin = JsonConvert.SerializeObject(elementsFin);
        var response = JsonConvert.SerializeObject($"{{projectNumber: {num}, elements:{elementsFinFin}, place: {place}, description:{desc} }} ");

        HttpContent dataJSON = new StringContent(response);








        var client = new HttpClient();
        var formData = new MultipartFormDataContent();


        formData.Add(dataJSON, "data");

        foreach (var x in AppState.photos)
        {
            FileStream fileStream = File.Open(x.FullPath, FileMode.Open);
            HttpContent fileStreamContent = new StreamContent(fileStream);
            fileStreamContent.Headers.Add("Content-Type", x.ContentType);


            formData.Add(fileStreamContent, "photo", x.FileName);

        }


        var res = await client.PostAsync("http://localhost:3000/", formData);

        if (!res.IsSuccessStatusCode)
        {
            await DisplayAlert("Failed", "Failed to send to server", "OK");

        }
    }

    private void addElement(object sender, EventArgs e)
    {
        tempId++;
        Element newElem = new Element();
        if (elementAmount.Text == null || categoriesUI.SelectedItem.ToString() == null || subcategoriesUI.SelectedItem.ToString() == null)
        {
            DisplayAlert("Błąd", "Prosze uzupełnić wszystkie pola", "OK");
        }
        newElem.Amount = int.Parse(elementAmount.Text);
        newElem.Category = categoriesUI.SelectedItem.ToString();
        newElem.Subcategory = subcategoriesUI.SelectedItem.ToString();
        newElem.Id = tempId;
        elementss.Add(newElem);
    }


    private void removeElement(object sender, EventArgs e)
    {
        string itemId = ((Button)sender).BindingContext.ToString();
        elementss.Remove(elementss.Where(x => x.Id == int.Parse(itemId)).FirstOrDefault());
        elements.ItemsSource = null;
        elements.ItemsSource = elementss;
    }


    private async void chooseImage(object sender, EventArgs e)
    {
        FileResult photo = await MediaPicker.Default.PickPhotoAsync();
        if (photo != null)
        {
            AppState.photos.Add(photo);

        }


    }

    private async void takePicture(object sender, EventArgs e)
    {
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
                    //  test.Source = photo.FullPath;
                    AppState.photos.Add(photo);

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                Debug.WriteLine("Failed to take picture");
                await DisplayAlert("Błąd", "Nie udało się zrobić zdjęcia", "OK");
            }
        }
    }
}

