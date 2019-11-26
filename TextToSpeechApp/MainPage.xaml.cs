using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace TextToSpeechApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public VoiceInformation SelectedVoice { get; set; }
        public string TextInput { get; set; }
        private Stream stream = null;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (stream != null)
                {
                    var filePicker = new FileSavePicker()
                    {
                        CommitButtonText = "Save",
                    };
                    filePicker.FileTypeChoices.Add("wav", new List<string>() { ".wav" });
                    var file = await filePicker.PickSaveFileAsync();
                    if (file != null)
                    {
                        var _stream = await file.OpenStreamForWriteAsync();
                        await stream.CopyToAsync(_stream);
                        await _stream.FlushAsync();
                        var dlg = new MessageDialog("File saved.", Package.Current.DisplayName);
                        var cmd = await dlg.ShowAsync();
                    }
                }
            }
            catch (Exception exception)
            {
                var dlg = new MessageDialog(exception.Message, Package.Current.DisplayName);
                var cmd = await dlg.ShowAsync();
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (var synthesizer = new SpeechSynthesizer())
                {
                    synthesizer.Voice = SelectedVoice ?? SpeechSynthesizer.DefaultVoice;

                    var synStream = await synthesizer.SynthesizeTextToStreamAsync(TextInput);

                    mPlayerElement.Source = MediaSource.CreateFromStream(synStream, synStream.ContentType);

                    stream = synStream.AsStream();
                    stream.Position = 0;

                    var dlg = new MessageDialog("Conversion succeeded.", Package.Current.DisplayName);
                    var cmd = await dlg.ShowAsync();
                }
            }
            catch (Exception exception)
            {
                var dlg = new MessageDialog(exception.Message, Package.Current.DisplayName);
                var cmd = await dlg.ShowAsync();
            }
        }
    }
}
