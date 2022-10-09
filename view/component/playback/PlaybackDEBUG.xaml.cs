using Microsoft.Win32;
using midi_sequencer.service;
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

namespace midi_sequencer.view.component.playback
{
    /// <summary>
    /// Логика взаимодействия для PlaybackWindow.xaml
    /// </summary>
    public partial class PlaybackDEBUG : Window
    {
        private PlaybackService playback = new PlaybackService();

        public PlaybackDEBUG()
        {
            InitializeComponent();

            //GlobalVars.dbgInt = 20;
            GlobalVars.midiOut = new(0);
            GlobalVars.midiEventCollection = PlaybackService.OpenFile("C:\\Users\\kosty\\source\\repos\\midi-sequencer\\Test MIDI files\\d_dead\\d_dead.mid");

            playback.StartNewThread();
        }

        // Тестовая палата для методов воспроизведения

        private void fileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                playback.Stop();
                playback.UpdateMidiCollection(PlaybackService.OpenFile(openFileDialog.FileName));

                //currentStateLabel.Content = "Current state: " + playback.playbackState;

                //DEBUGListBox.Items.Add("maxAbsoluteTime: " + playback.maxAbsoluteTime);
                //DEBUGListBox.Items.Add("MIDI Events:");
                //for (int i = 0; i < playback.bigEventList.Count; i++)
                //{
                //    DEBUGListBox.Items.Add(playback.bigEventList[i]);
                //}
                //DEBUGListBox.Items.Add("");
            }
            //else
            //{
            //    playback.Close();

            //    currentStateLabel.Content = "Current state: " + playback.playbackState;
            //    fileButton.Content = "Open file";

            //    playback = null;
            //}
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            playback.Close();
            //playback = null;

            //this.playbackService.Close();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            playback.Play();

            currentStateLabel.Content = "Current state: " + playback.playbackState;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            playback.Pause();

            currentStateLabel.Content = "Current state: " + playback.playbackState;
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            playback.Stop();

            currentStateLabel.Content = "Current state: " + playback.playbackState;
        }

        private void dbg0_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("dbgInt = " + GlobalVars.dbgInt);
        }

        private void dbg1_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("");

        }
    }
}
