using midi_sequencer.model;
using midi_sequencer.service;
using midi_sequencer.view.component;
using midi_sequencer.view.component.general;
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

            window.Show();
        }

        public void PlaybackWindow(Window window)
        {
            window = new PlaybackDEBUG();
            window.Show();
        }

        public void GeneralWindow(Window window)
        {
            MidiService midiService = new MidiService(); //!!!

            //midiService.collection = PlaybackService.OpenFile("C:\\Users\\kosty\\source\\repos\\midi-sequencer\\Test MIDI files\\d_dead\\d_dead.mid");

            Grid view = new();

            ColumnDefinition firstCol = new();
            RowDefinition firstRow = new(); // Tray Red
            RowDefinition secondRow = new(); // Playback Blue
            RowDefinition thirdRow = new(); // PlayerScrollBar Green
            RowDefinition fourthRow = new(); // Channels Yellow
            RowDefinition fifthRow = new(); // StatusBar Purple

            firstRow.Height = new GridLength(30);
            secondRow.Height = new GridLength(60);
            thirdRow.Height = new GridLength(30);
            fifthRow.Height = new GridLength(30);

            view.ColumnDefinitions.Add(firstCol);
            view.RowDefinitions.Add(firstRow);
            view.RowDefinitions.Add(secondRow);
            view.RowDefinitions.Add(thirdRow);
            view.RowDefinitions.Add(fourthRow);
            view.RowDefinitions.Add(fifthRow);

            Grid tray = new Tray().Build();
            tray.SetValue(Grid.RowProperty, 0);
            tray.SetValue(Grid.ColumnProperty, 0);

            Grid playback = new Playback(midiService).Build();
            playback.SetValue(Grid.RowProperty, 1);
            playback.SetValue(Grid.ColumnProperty, 0);

            Grid playerScrollBar = new PlayerScrollBar().Build();
            playerScrollBar.SetValue(Grid.RowProperty, 2);
            playerScrollBar.SetValue(Grid.ColumnProperty, 0);

            Grid channels = new Channels(midiService).Build();
            channels.SetValue(Grid.RowProperty, 3);
            channels.SetValue(Grid.ColumnProperty, 0);

            Grid statusBar = new StatusBar().Build();
            statusBar.SetValue(Grid.RowProperty, 4);
            statusBar.SetValue(Grid.ColumnProperty, 0);

            view.Children.Add(tray);
            view.Children.Add(playback);
            view.Children.Add(playerScrollBar);
            view.Children.Add(channels);
            view.Children.Add(statusBar);

            window.Content = view;

            window.Show();
        }
    }
}
