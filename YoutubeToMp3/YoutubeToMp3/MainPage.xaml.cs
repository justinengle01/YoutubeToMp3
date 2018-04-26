using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using VideoLibrary;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using YoutubeExtractor;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace YoutubeToMp3
{
  /// <summary>
  /// An empty page that can be used on its own or navigated to within a Frame.
  /// </summary>
  /// 



   /*
    
     
     CURRENTLY CAN JUST DOWNLOAD VIDEOS     
     
     
     */











  public sealed partial class MainPage : Page
  {
    public string savePath = string.Empty;
    private const string choosePath = "Please select a path";
    public IEnumerable<YouTubeVideo> videos;
    public YouTubeVideo currentVideo;
    Windows.Storage.StorageFolder storageFolder =     Windows.Storage.ApplicationData.Current.LocalFolder;
    public MainPage()
    {
      this.InitializeComponent();
      ApplicationView.PreferredLaunchViewSize = new Size(1000, 450);
      ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
      txtPath.Text = choosePath;
      ToggleProgressBar(false, "");

      showMessageBox("This app only has access to the following folder, so please save there: Documents, Music, Pictures, Videos, Desktop ");
    }

    private async void btnBrowse_Click(object sender, RoutedEventArgs e)
    {

      var folderPicker = new Windows.Storage.Pickers.FolderPicker();
      folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.VideosLibrary;
      folderPicker.FileTypeFilter.Add("*");

      Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();
      if (folder != null)
      {
        // Application now has read/write access to all contents in the picked folder
        // (including other sub-folder contents)
        Windows.Storage.AccessCache.StorageApplicationPermissions.
        FutureAccessList.AddOrReplace("PickedFolderToken", folder);
        txtPath.Text = folder.Path;
        savePath = folder.Path;
      }
      else
      {
        txtPath.Text = choosePath;
      }

    }

    private async void btnDetails_Click(object sender, RoutedEventArgs e)
    {
      ProgressDesc.Text = string.Empty;
      ProgressDesc.Visibility = Visibility.Collapsed;
      if (savePath != string.Empty)
      {
        DownloadArea.Visibility = Visibility.Collapsed;
        DataContext = null;
        var youTube = YouTube.Default; // starting point for YouTube actions
        ToggleProgressRing(true);
        videos = await youTube.GetAllVideosAsync(txtURL.Text); // gets a Video object with info about the video
        var videoViewModels = videos.Select(v => new VideoViewModel(v))
                                           .ToList();
        videoSelection.ItemsSource = videoViewModels.Where(v => !v.Description.Contains("-1"));
        DataContext = videos.ToArray()[0];
        ToggleProgressRing(false);
        DownloadArea.Visibility = Visibility.Visible;
      }
      else
      {
        showMessageBox("Where must I save to? Please select a valid path");
      }
    
    }

    private void ToggleProgressRing(bool active)
    {
      string[] randomText =
      {
        "Wait wait wait...I'm busy",
        "I'm still alive..just thinking",
        "Either I'm slow or your internet sucks",
        "Waiting for Youtube...."
      };
      if (active)
      {
        var rnd = new Random();
        waitingTxt.Text = randomText[rnd.Next(0, randomText.Length - 1)];
      }
      progress.IsActive = active;
      progress.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
      waitingTxt.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
      Title.Visibility = active ? Visibility.Collapsed : Visibility.Visible;
    }

    private async void btnDownload_Click(object sender, RoutedEventArgs e)
    {

      if (videoSelection.SelectedIndex != -1)
      {
        ToggleProgressBar(true, "Downloading...");
        byte[] bytes = await currentVideo.GetBytesAsync();
        ToggleProgressBar(true, "Saving Video...");
        await WriteToDisk(bytes);
        ToggleProgressBar(true, "Converting...");
        ToggleProgressBar(false, string.Empty);
        ProgressDesc.Text = "DONE";
        ProgressDesc.Visibility = Visibility.Visible;
      }
      else
      {
        showMessageBox("You do know YouTube has multiple quality options? Please select one");
      }
     
    }
    private async Task WriteToDisk(byte[] bytes)
    {
      await Task.Run(() =>
      {
        File.WriteAllBytes($"{storageFolder.Path}/{currentVideo.FullName}", bytes);      
      });
    }
    private void ToggleProgressBar(bool active, string description)
    {
      ProgressDesc.Text = description;
      downloadProgress.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
      ProgressDesc.Visibility = active ? Visibility.Visible : Visibility.Collapsed;
    }

    private void videoSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
      if(videoSelection.SelectedIndex > -1)
      {
        currentVideo = videos.ToList()[videoSelection.SelectedIndex];
      }
     

    }

    private void showMessageBox(string msg)
    {
      var msgBox = new MessageDialog(msg);
      msgBox.ShowAsync();
    }
  }
}
