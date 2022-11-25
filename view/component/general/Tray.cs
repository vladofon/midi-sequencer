using Microsoft.Win32;
using midi_sequencer.service;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace midi_sequencer.view.component.general
{
    internal class Tray : Component
    {
        private Brush brush;

        public Tray()
        {
            brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#36495a");
        }

        public Grid Build()
        {
            ToolBarTray tbt = new();
            tbt.Height = 30;
            tbt.VerticalAlignment = VerticalAlignment.Center;

            ToolBar tb = new();
            tb.Height = tbt.Height;
            Button openFile = new();
            openFile.Content = "Open file";
            openFile.Click += openFile_Click;
            tb.Items.Add(openFile);

            tbt.ToolBars.Add(tb);

            Grid grid = new();
            grid.Background = brush;

            grid.Children.Add(tbt);

            return grid;
        }

        private void openFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();

            if (openFileDialog.ShowDialog() == true)
            {
                MidiService.GetInstance().ImportFileToCollection(openFileDialog.FileName);
            }

        }
    }
}
