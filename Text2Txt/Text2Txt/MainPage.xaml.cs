using TesseractOcrMaui;
using TesseractOcrMaui.Extensions;

namespace Text2Txt;

public partial class MainPage : ContentPage
{
	

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

}

