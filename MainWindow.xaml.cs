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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace midi_sequencer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void pianoRollButton_Click(object sender, RoutedEventArgs e)
        {
            PianoRollWindow pianoRollWindow = new PianoRollWindow();
            //pianoRollWindow.Show();

            DesignManager dm = new();
            dm.DrawPianoRoll(pianoRollWindow);
        }

        //________________________

        Playback? playback;
        MidiOut midiOut = new MidiOut(0);

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                //Thread playbackThread = new Thread(() => Playback.PlayFile(openFileDialog.FileName));
                //playbackThread.Start();
                //Playback.PlayFile(openFileDialog.FileName);

                playback = new Playback(midiOut, Playback.OpenFile(openFileDialog.FileName));

                timer.Stop();
                //durationLabel.Content = "Opening file: " + timer.Elapsed;

                DEBUGListBox.Items.Add("Opening file: " + timer.Elapsed);
                DEBUGListBox.Items.Add("maxAbsoluteTime" + playback.maxAbsoluteTime);
                DEBUGListBox.Items.Add("MIDI Events:");

                for (int i = 0; i < playback.bigEventList.Count; i++)
                {
                    DEBUGListBox.Items.Add(playback.bigEventList[i]);
                }

                DEBUGListBox.Items.Add("");
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            //Stopwatch timer = new Stopwatch();
            //timer.Start();

            if (playback != null)
            {
                playback.ThreadPlay();
                
            }


            Thread timeThread = new Thread(() =>
            {
                ;
            });
            timeThread.Start();


            //timer.Stop();
            //durationLabel.Content = "Playing collection: " + timer.Elapsed;
        }
    }
}
