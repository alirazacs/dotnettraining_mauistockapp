using Azure;
using Azure.AI.Vision.ImageAnalysis;
using CommunityToolkit.Maui.Converters;
using DotnetTrainingStockApp.ViewModels;
using System.Text.RegularExpressions;

namespace DotnetTrainingStockApp.Views;

[QueryProperty(nameof(Photo), nameof(Photo))]
public partial class StockItemDetailsPage : ContentPage
{
	public string? Photo {  get; set; }
	public StockItemDetailsPage(StockItemDetailsViewModel stockItemDetailsViewModel)
	{
		InitializeComponent();
        BindingContext = stockItemDetailsViewModel;
    }

	protected override void OnAppearing()
	{
		base.OnAppearing();

        ((StockItemDetailsViewModel)BindingContext).Path = Photo;
        if (Photo != null)
        {
            AnalyzeImage(File.ReadAllBytes(Photo));
        }
    }

    private static void AnalyzeImage(byte[] data)
    {

        //images used for scanning expiry date:
        //https://lovefoodhatewaste.co.nz/wp-content/uploads/2016/03/Butter-best-before-date-1920-cropped.jpg
        //https://res.cloudinary.com/bunch-media-library/image/upload/w_645,h_378,c_fill,g_faces,q_auto,f_auto,g_auto,fl_lossy/v1605837183/articles/apw7xpihtlrdf5ckdhic.jpg
        //https://www.shutterstock.com/shutterstock/photos/2171820503/display_1500/stock-photo-selective-focus-on-manufacturing-date-and-expiry-date-indicator-aka-mfg-and-exp-dates-in-the-bottle-2171820503.jpg
        //https://pirg.org/edfund/wp-content/uploads/2021/09/Best-ByCan-1-scaled.jpg
        //https://prescriptionhope.com/wp-content/uploads/2020/01/Do-Medications-Expire.jpg

        BinaryData binaryData = new BinaryData(data);

        int maxTag = 5;
        string endpoint = "https://stockvision.cognitiveservices.azure.com/";
        string key = "209f0f88a4fd4522808428efe55be4c3";
        List<string> dates = new List<string>();

        ImageAnalysisClient client = new ImageAnalysisClient(
            new Uri(endpoint),
            new AzureKeyCredential(key));

        ImageAnalysisResult result = client.Analyze(
            binaryData,
            VisualFeatures.Tags | VisualFeatures.Read);

        string pattern = @"\b\d{1,2}/\d{1,2}/\d{2}(?:\d{2})?\b"; //for dates like : 12/01/21  OR 12/01/2021 (date/month/year)

        Console.WriteLine("Detected Tags are :");
        if (result?.Tags?.Values.Count > maxTag)
        {
            for (int i = 0; i < maxTag; i++)
            {
                Console.WriteLine(result.Tags.Values[i].Name);
            }
        }
        else
        {
            foreach (DetectedTag obj in result?.Tags?.Values)
            {
                Console.WriteLine(obj.Name);
            }
        }


        //reading the lines,words to fetch expiry date.

        foreach (DetectedTextBlock block in result.Read.Blocks)
        {
            foreach (DetectedTextLine line in block.Lines)
            {
                foreach (DetectedTextWord word in line.Words)
                {
                    if (word.Text.Contains("/"))
                    {
                        //adding all dates having format with /
                        dates.Add(word.Text);
                    }
                }
            }
        }

        //get the expiry date....usually the expiry date is at last on most products so getting the last date...

        if (dates.Count > 0)
        {
            Match match = Regex.Match(dates[dates.Count - 1], pattern);
            if (match.Success)
            {
                Console.WriteLine("Expiry date is" + dates[dates.Count - 1]);
            }
        }
    }

    //private string analyzeimage(byte[] data)
    //{
    //    console.writeline("adsfasdfasdfadsf");
    //    string endpoint = "https://stockvision.cognitiveservices.azure.com/";
    //    string key = "209f0f88a4fd4522808428efe55be4c3";

    //    binarydata binarydata = new binarydata(data);

    //    imageanalysisclient client = new imageanalysisclient(
    //        new uri(endpoint),
    //        new azurekeycredential(key));

    //    imageanalysisresult result = client.analyze(
    //      binarydata,
    //        visualfeatures.tags);
    //    return result.tags.values[0].name;
    //}

    private async void OnAddToCartBtnClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("..");
	}
}