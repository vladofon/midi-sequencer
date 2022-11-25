using midi_sequencer.model;
using midi_sequencer.service;
using midi_sequencer.view;
using midi_sequencer.windows;
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

namespace midi_sequencer.windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DesignManager designManager;

        public MainWindow()
        {
            InitializeComponent();
            //this.designManager = new();
        }

        private void pianoRollButton_Click(object sender, RoutedEventArgs e)
        {
            PianoRollWindow pianoRollWindow = new();

            designManager.PianoRollWindow(pianoRollWindow);
        }

        private void playbackButton_Click(object sender, RoutedEventArgs e)
        {
            PlaybackWindow playbackWindow = new();

            designManager.PlaybackWindow(playbackWindow);
        }

        private void generalButton_Click(object sender, RoutedEventArgs e)
        {
            GeneralWindow generalWindow = new();

            //designManager.GeneralWindow(generalWindow);
        }

        //________________________


    }
}
