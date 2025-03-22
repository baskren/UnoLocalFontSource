using Windows.Networking;

namespace SideloadedFont;

public sealed partial class MainPage : Page
{
    HttpClient HttpClient = new();

    public MainPage()
    {
        this.InitializeComponent();
    }

    private async Task<string?> DownloadFont()
    {
        var sourceUri = new Uri("https://github.com/google/fonts/raw/refs/heads/main/ofl/monsieurladoulaise/MonsieurLaDoulaise-Regular.ttf");
        var bytes = await HttpClient.GetByteArrayAsync(sourceUri);

        var localPath = Path.Combine("FONTS", "MonsieurLaDoulaise-Regular.ttf");
        var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, localPath); 
        var folder = Path.GetDirectoryName(path);
        Directory.CreateDirectory(folder);

        await File.WriteAllBytesAsync(path, bytes);
        return localPath;
    }

    private void ApplyFont(string localPath)
    {
        var path = Path.Combine(ApplicationData.Current.LocalFolder.Path, localPath);
        var familyName = TrueTypeFontInfo.FontFamily(path);
        var fontPath = "ms-appdata:///local/" + localPath.Replace('\\', '/') + (string.IsNullOrWhiteSpace(familyName) ? null : "#" + familyName);
        TextBlock.TextWrapping = TextWrapping.Wrap;
        TextBlock.Text = $"FAMILY: {familyName}\nPATH:{fontPath}";
        TextBlock.FontFamily = new FontFamily(fontPath);
    }

    private async void Button_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            if (await DownloadFont() is not { } path)
                return; 

            ApplyFont(path);
        }
        catch (Exception ex) 
        {
            TextBlock.Text = ex.Message;
        }
    }

}
