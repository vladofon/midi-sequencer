using Microsoft.Win32;
using midi_sequencer.playback;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace midi_sequencer
{
    /// <summary>
    /// Логика взаимодействия для PlaybackWindow.xaml
    /// </summary>
    public partial class PlaybackWindow : Window
    {
        public PlaybackWindow()
        {
            InitializeComponent();
        }

        // Тестовая палата для методов воспроизведения

        Playback? playback;
        MidiOut midiOut = new MidiOut(0);

        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
            if (playback == null)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();

                if (openFileDialog.ShowDialog() == true)
                {
                    playback = new Playback(midiOut, Playback.OpenFile(openFileDialog.FileName));

                    currentStateLabel.Content = "Current state: " + playback.playbackState;
                    fileButton.Content = "Close file";

                    DEBUGListBox.Items.Add("maxAbsoluteTime: " + playback.maxAbsoluteTime);
                    DEBUGListBox.Items.Add("MIDI Events:");
                    for (int i = 0; i < playback.bigEventList.Count; i++)
                    {
                        DEBUGListBox.Items.Add(playback.bigEventList[i]);
                    }
                    DEBUGListBox.Items.Add("");
                }
            }
            else
            {
                playback.Close();

                currentStateLabel.Content = "Current state: " + playback.playbackState;
                fileButton.Content = "Open file";

                playback = null;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (playback != null)
            {
                playback.Close();
                playback = null;
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (playback != null)
            {
                playback.Play();

                currentStateLabel.Content = "Current state: " + playback.playbackState;
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (playback != null)
            {
                playback.Pause();

                currentStateLabel.Content = "Current state: " + playback.playbackState;
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (playback != null)
            {
                playback.Stop();

                currentStateLabel.Content = "Current state: " + playback.playbackState;
            }
        }
    }
}
