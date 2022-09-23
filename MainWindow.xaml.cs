using Microsoft.Win32;
using midi_sequencer.playback;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

        private void openFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                Stopwatch timer = new Stopwatch();
                timer.Start();
                Playback.PlayFile(openFileDialog.FileName);
                timer.Stop();
                durationLabel.Content = timer.Elapsed;
            }
        }
    }
}
