﻿using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using TesseractOcrMaui;
using TesseractOcrMaui.Extensions;

namespace Text2Txt;

public partial class MainPage : ContentPage
{
    public const int LEXILE_EASY = 200;
    public const int LEXILE_MED = 500;
    public const int LEXILE_HARD = 800;
    public int LexileLevel = 0;
    public int InputMethod = 0;

	public MainPage(ITesseract tesseract)
	{
		InitializeComponent();
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
		string prompt = $"Simplify the reading level of this text to a lexile score of {lexileLevel} -> {ocrBucket}";
        AI ai = new AI();
        string aiBucket = await AI.APICall(prompt, ai.apiKey);
        SimplifiedText.Text = aiBucket;
		Status.Text = "Not Running";
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
        Status.Text = $"Confidence: {result.Confidence}";
        if (result.NotSuccess())
        {
            return $"Recognizion failed: {result.Status}";
        }
        return result.RecognisedText;
    }

    private void CreateButton_Clicked(object sender, EventArgs e)
    {
        PageFlipper(2);
    }

    private void ViewButton_Clicked(object sender, EventArgs e)
    {
        PageFlipper(3);
    }

    private void SettingButton_Clicked(object sender, EventArgs e)
    {
        PageFlipper(4);
    }
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
                    break;
                case 3:
                    ViewPage.IsVisible = true;
                    break;
                case 4:
                    SettingsPage.IsVisible = true;
                    break;
        }
    }
    private void CloseAllPages()
    {
        PrototypePage.IsVisible = false;
        MenuPage.IsVisible = false;
        CreatePage.IsVisible = false;
        ViewPage.IsVisible = false;
        SettingsPage.IsVisible = false;
    }
    private void CloseCreatePages()
    {
        LexileMenu.IsVisible = false;
        InputMenu.IsVisible = false;
        OCRMenu.IsVisible = false;
        FileMenu.IsVisible = false;
        PasteMenu.IsVisible = false;
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
}

