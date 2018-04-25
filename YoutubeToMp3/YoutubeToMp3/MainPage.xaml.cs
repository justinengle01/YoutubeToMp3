using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using VideoLibrary;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
  public sealed partial class MainPage : Page
  {
    public string savePath;
    private const string choosePath = "Please select a path";
    public MainPage()
    {
      this.InitializeComponent();
      ApplicationView.PreferredLaunchViewSize = new Size(1000, 500);
      ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
      txtPath.Text = choosePath;
    }

    private async void btnBrowse_Click(object sender, RoutedEventArgs e)
    {

      var folderPicker = new Windows.Storage.Pickers.FolderPicker();
      folderPicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Desktop;
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
      DataContext = null;
     var youTube = YouTube.Default; // starting point for YouTube actions
      ToggleProgressRing(true);     
      var video = await youTube.GetVideoAsync(txtURL.Text); // gets a Video object with info about the video
      DataContext = video;
      ToggleProgressRing(false);
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
    }

    private void btnDownload_Click(object sender, RoutedEventArgs e)
    {

    }
  }
}
