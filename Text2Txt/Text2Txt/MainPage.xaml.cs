using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Net;
using System.Threading.Tasks;
using TesseractOcrMaui;
using TesseractOcrMaui.Extensions;
using Text2Txt.Models;

namespace Text2Txt;

public partial class MainPage : ContentPage
{
    public const int LEXILE_EASY = 200;
    public const int LEXILE_MED = 500;
    public const int LEXILE_HARD = 800;
    public int LexileLevel = 0;
    public int InputMethod = 0;
    public string InputText = "";
    public string InputPath = "";
    public string apiKey = "API KEY HERE";

	public MainPage(ITesseract tesseract)
	{
		InitializeComponent();
        ReadAPIKey();
        Title = "Text2Txt";
        Application.Current.UserAppTheme = AppTheme.Light;
        Tesseract = tesseract;
	}
	ITesseract Tesseract { get; }

    private void Scan_Photo_Clicked(object sender, EventArgs e)
    {
		Handle();
        
    }
	private async Task Handle()
	{
        Status.Text = "Running OCR";
		int lexileLevel = 500;
		//WindowsOCR ocr = new WindowsOCR();
        //string ocrBucket = await ocr.OCRAction();
        string ocrBucket = await PerformOCR();
		OutputText.Text = ocrBucket;
		Status.Text = "Running AI";
		string prompt = $"Simplify the reading level of this text to a lexile score of {lexileLevel} without losing information -> {ocrBucket}";
        AI ai = new AI();
        string aiBucket = await AI.APICall(prompt, ai.apiKey);
        SimplifiedText.Text = aiBucket;
		Status.Text = "Not Running";
	}
    private async Task<AIResponse> TextToAI(string text, int lexile, string apiKey)
    {

        string prompt = $"Simplify the reading level of this text to a lexile score of {lexile} without losing information -> {text}";
        return await AI.AICALL(prompt, apiKey);
        
    }
    private void ResetBuckets()
    {
        InputText = "";
        InputPath = "";
        OutputText.Text = "";
        SimplifiedText.Text = "";
        LexileLevel = 0;
        InputMethod = 0;
    }

    // MASTER PAGE ----------------------------------------------------------------------------------------------------------------------------

    // CREATE TEXT PAGE
    private async void CreateButton_Clicked(object sender, EventArgs e)
    {
        bool result = await IsAPIKeyValid();
        if(result)
        PageFlipper(2);
        else
        {
            await DisplayAlert("Error", "Please enter a valid API key in the settings page", "OK");
        }
    }
    // VIEW TEXT PAGE
    private async void ViewButton_Clicked(object sender, EventArgs e)
    {

        string filePath = await FolderPickDocs();
        await OpenDocument(filePath);
    }
    // SETTINGS PAGE
    private void SettingButton_Clicked(object sender, EventArgs e)
    {
        PageFlipper(4);
    }
    // MASTER FUNCTION PAGE FLIPPER
    private void PageFlipper(int pageNumber)
    {
        CloseAllPages();
            switch (pageNumber)
        {
            case 1:
                    MenuPage.IsVisible = true;
                    break;
                case 2:
                    CreatePage.IsVisible = true;
                    LexileMenu.IsVisible = true;
                    break;
                case 3:
                    ViewPage.IsVisible = true;
                    break;
                case 4:
                    SettingsPage.IsVisible = true;
                    break;
        }
    }
    // MASTER FUNCTION PAGE CLOSE
    private void CloseAllPages()
    {
        PrototypePage.IsVisible = false;
        MenuPage.IsVisible = false;
        CreatePage.IsVisible = false;
        ViewPage.IsVisible = false;
        SettingsPage.IsVisible = false;
    } 
    private void BackButton_Clicked(object sender, EventArgs e)
    {
        CloseAllPages();
        CloseCreatePages();
        CloseViewPages();
        CloseSettingsPages();
        PageFlipper(1);
        ResetBuckets();
    }

    // API Storage
    private async Task SaveAPIKey()
    {
        var path = FileSystem.Current.AppDataDirectory;
        var fullPath = Path.Combine(path, "APIKey.txt");
        apiKey = ApiKeyEntry.Text;
        if (apiKey == "" || apiKey == "API KEY HERE")
        {
            await DisplayAlert("Error", "Please enter a valid API Key", "OK");
            return;
        }
        else
        {
            File.WriteAllText(fullPath, apiKey);
            await DisplayAlert("Success", "API Key Saved", "OK");
        }
    }
    private void ReadAPIKey()
    {
        var path = FileSystem.Current.AppDataDirectory;
        var fullPath = Path.Combine(path, "APIKey.txt");
        if (File.Exists(fullPath))
        {
            apiKey = File.ReadAllText(fullPath);
        }
        else
        {
            apiKey = "API KEY HERE";
        }
        ApiKeyEntry.Text = apiKey;
    }
    private async Task<bool> IsAPIKeyValid()
    {
        Title = "Checking API Key";
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri("https://api.openai.com/v1/completions"),
            Content = new StringContent("{\n   \"model\": \"text-davinci-003\", \"prompt\": \"" + $"Check\",\n    \"max_tokens\": 20,\n    \"temperature\": 0.5\n}}")

        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        var response = await client.SendAsync(request);
        Title = "Text2Txt";
        return response.StatusCode == HttpStatusCode.OK;
    }

    // CREATE PAGE --------------------------------------------------------------------------------------------------------------------------

    // CREATE PAGE CLOSE
    private void CloseCreatePages()
    {
        LexileMenu.IsVisible = false;
        InputMenu.IsVisible = false;
        OCRMenu.IsVisible = false;
        FileMenu.IsVisible = false;
        PasteMenu.IsVisible = false;
        CreateLoading.IsVisible = false;
        ResultPage.IsVisible = false;
    }
    private void OnEasyClicked(object sender, EventArgs e)
    {
        EasyButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        MediumButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        HardButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        LexileLevel = LEXILE_EASY;
    }
    private void OnMediumClicked(object sender, EventArgs e)
    {
        EasyButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        MediumButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        HardButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        LexileLevel = LEXILE_MED;
    }
    private void OnHardClicked(object sender, EventArgs e)
    {
        EasyButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        MediumButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        HardButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        LexileLevel = LEXILE_HARD;
    }
    private void LexileScoreNextButton_Clicked(object sender, EventArgs e)
    {
        if(LexileLevel == 0)
        {
            DisplayAlert("Error", "Please select a lexile level", "OK");
        }
        else
        {
            CloseCreatePages();
            InputMenu.IsVisible = true;
        }
    }
    private void CaptureButton_Clicked(object sender, EventArgs e)
    {
        CaptureButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        FromFileButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        PasteButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        InputMethod = 1;
    }
    private void FromFileButton_Clicked(object sender, EventArgs e)
    {
        CaptureButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        FromFileButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        PasteButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        InputMethod = 2;
    }
    private void PasteButton_Clicked(object sender, EventArgs e)
    {
        CaptureButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        FromFileButton.Background = new SolidColorBrush(Color.FromHex("#BBBBBB"));
        PasteButton.Background = new SolidColorBrush(Color.FromHex("#6600FF"));
        InputMethod = 3;
    }
    private void InputMethodNextButton_Clicked(object sender, EventArgs e)
    {
        if (InputMethod == 0)
        { 
            DisplayAlert("Error", "Please select an input method", "OK");
        }
        else
        {
            CloseCreatePages();
            switch(InputMethod)
            {
                case 1:
                    OCRMenu.IsVisible = true;
                    break;
                case 2:
                    FileMenu.IsVisible = true;
                    break;
                case 3:
                    PasteMenu.IsVisible = true;
                    break;
            }
        }
    }
    private async Task<string> PerformOCR()
    {
        // Make user pick file
        #pragma warning disable CA1416 // Validate platform compatibility
        var pickResult = await FilePicker.PickAsync(new PickOptions()
        {
            PickerTitle = "Pick png image",
            // Currently usable image types
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                [DevicePlatform.Android] = new List<string>() { "image/png", "image/jpeg" },
                [DevicePlatform.WinUI] = new List<string>() { ".png", ".jpg", ".jpeg" },
            })
        });
        #pragma warning restore CA1416 // Validate platform compatibility
                              // null if user cancelled the operation
        if (pickResult is null)
        {
            return null;
        }

        // Recognize image
        var result = await Tesseract.RecognizeTextAsync(pickResult.FullPath);

        // Show output
        if (result.NotSuccess())
        {
            return null;
        }
        InputPath = pickResult.FullPath;
        return result.RecognisedText;
    }
    private async void PickFromFolderButton_Clicked(object sender, EventArgs e)
    {
        InputText = await PerformOCR();
        if(InputPath == null || InputPath == "")
        {
            await DisplayAlert("Error", "Please select a valid image", "OK");
            PathLabel.Text = "Path: null";
        }
        else
        {
            PathLabel.Text = "Path: Selected";
        }
    }
    private async void PasteNextButton_Clicked(object sender, EventArgs e)
    {
        InputText = PasteEditor.Text;
        if (InputText == null) { await DisplayAlert("Error", "Please paste valid text", "OK"); }
        else
        {
            CloseCreatePages();
            CreateLoading.IsVisible = true;
            ActivityLabel.Text = "Creating Request...";
            ActivityLabel.Text = "Waiting on OpenAI...";
            AIResponse aI = await TextToAI(InputText, LexileLevel, apiKey);
            CloseCreatePages();
            ResultPage.IsVisible = true;
            ResultLabel.Text = aI.Name;
            ResultTextBox.Text = aI.Text;
            TimeSpan vibrationLength = TimeSpan.FromSeconds(1);
            Vibration.Default.Vibrate(vibrationLength);
        }
    }
    private async void OCRNextButton_Clicked(object sender, EventArgs e)
    {
        if (apiKey != "API KEY HERE")
        {
            if (InputText == null) { await DisplayAlert("Error", "Please select a valid image", "OK"); }
            else
            {
                CloseCreatePages();
                CreateLoading.IsVisible = true;
                ActivityLabel.Text = "Creating Request...";
                ActivityLabel.Text = "Waiting on OpenAI...";
                AIResponse aI = await TextToAI(InputText, LexileLevel, apiKey);
                CloseCreatePages();
                ResultPage.IsVisible = true;
                ResultLabel.Text = aI.Name;
                ResultTextBox.Text = aI.Text;
                TimeSpan vibrationLength = TimeSpan.FromSeconds(1);
                Vibration.Default.Vibrate(vibrationLength);
            }
        }
        else
        {
            await DisplayAlert("Error", "Please enter an API key in settings", "OK");
        }
    }

    // VIEW PAGE ----------------------------------------------------------------------------------------------------------------------------

    // VIEW PAGE CLOSE
    private async Task<string> FolderPickDocs()
    {

        // Make user pick file
#pragma warning disable CA1416 // Validate platform compatibility
        var pickResult = await FilePicker.PickAsync(new PickOptions()
        {
            PickerTitle = "Select Document",
            FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>()
            {
                [DevicePlatform.Android] = new List<string>() { "application/pdf", "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "text/plain" }
            })
        });
#pragma warning restore CA1416 // Validate platform compatibility
        // null if user cancelled the operation
        if (pickResult is null)
        {
            return null;
        }
        else
            return pickResult.FullPath;
    }
    private void CloseViewPages()
    {
    }

    private async Task OpenDocument(string filePath)
    {
        await Launcher.OpenAsync(new OpenFileRequest
        {
            File = new ReadOnlyFile(filePath)
        });
    }



    // SETTINGS PAGE ------------------------------------------------------------------------------------------------------------------------

    // SETTINGS PAGE CLOSE
    private void CloseSettingsPages()
    {
    }

    private async void SaveSettingButton_Clicked(object sender, EventArgs e)
    {
        apiKey = ApiKeyEntry.Text;
        
        bool result = await IsAPIKeyValid();
        if (result) 
        {
            BackButton_Clicked(sender, e); 
            await SaveAPIKey(); 
        }
        else
        {
            await DisplayAlert("Error", "Please enter a valid API key in the settings page", "OK");
        }
    }

    private void OpenAiInfoButton_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://platform.openai.com/account/api-keys");
    }

    private void Text2TxtInfoButton_Clicked(object sender, EventArgs e)
    {
        Launcher.OpenAsync("https://github.com/ayersdecker/Text2Txt-App");
    }
}