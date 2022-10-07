using midi_sequencer.model;
using midi_sequencer.service;
using midi_sequencer.view.component;
using midi_sequencer.view.component.piano_roll;
using midi_sequencer.view.component.playback;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace midi_sequencer.view
{
    internal class DesignManager
    {


        public void PianoRollWindow(Window window)
        {
            window.Show();

            Grid view = new();

            ColumnDefinition firstCol = new();
            RowDefinition firstRow = new();
            RowDefinition secondRow = new();

            firstRow.Height = new GridLength(30);

            view.ColumnDefinitions.Add(firstCol);
            view.RowDefinitions.Add(firstRow);
            view.RowDefinitions.Add(secondRow);

            PianoRoll pianoRoll = new PianoRoll(1);
            Grid pianoRollGrid = pianoRoll.Build();
            pianoRollGrid.VerticalAlignment = VerticalAlignment.Bottom;
            pianoRollGrid.SetValue(Grid.RowProperty, 1);
            pianoRollGrid.SetValue(Grid.ColumnProperty, 0);

            Grid pianoRollTray = new PianoRollTray(pianoRoll).Build();
            pianoRollTray.SetValue(Grid.RowProperty, 0);
            pianoRollTray.SetValue(Grid.ColumnProperty, 0);

            view.Children.Add(pianoRollTray);
            view.Children.Add(pianoRollGrid);

            window.Content = view;
        }

        public void PlaybackWindow(Window window)
        {
            window = new Playback();
            window.Show();
        }
    }
}
