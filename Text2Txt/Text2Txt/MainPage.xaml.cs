namespace Text2Txt;

public partial class MainPage : ContentPage
{
	

	public MainPage()
	{
		InitializeComponent();
	}

    private void Scan_Photo_Clicked(object sender, EventArgs e)
    {
		Handle();
    }
	private async Task Handle()
	{
		int lexileLevel = 500;
		OCR ocr = new OCR();
        string ocrBucket = await ocr.OCRAction();
		OutputText.Text = ocrBucket;
		string prompt = $"Simplify this text to a lexile score of {lexileLevel} {ocrBucket}";
        AI ai = new AI();
        string aiBucket = await AI.APICall(prompt, ai.apiKey);
        SimplifiedText.Text = aiBucket;
	}
}

