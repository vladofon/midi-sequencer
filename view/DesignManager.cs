using midi_sequencer.model;
using midi_sequencer.service;
using midi_sequencer.view.component;
using midi_sequencer.view.component.general;
using midi_sequencer.view.component.piano_roll;
using midi_sequencer.view.component.playback;
using midi_sequencer.windows;
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
        public delegate void DPianoRollWindow(int channelNumber);

        public void PianoRollWindow(int channelNumber)
        {
            PianoRollWindow pianoRollWindow = new PianoRollWindow();
            //pianoRollWindow.Closed += PianoRollWindow_Closed;

            Grid view = new();

            PianoRoll pianoRoll = new PianoRoll(channelNumber);
            Grid pianoRollGrid = pianoRoll.Build();

            view.Children.Add(pianoRollGrid);

            pianoRollWindow.Content = view;

            pianoRollWindow.Show();
        }

        //private void PianoRollWindow_Closed(object? sender, EventArgs e)
        //{
        //    PianoRollWindow pianoRollWindow = (PianoRollWindow)sender;

        //    MessageBox.Show("piano roll on channel " + channelNumber + " closed");

        //    MidiEventMapper mapper = new();
        //    List<MidiEvent> midi = mapper.mapAll(pianoRoll.GetNoteButtons());

        //    MidiService.GetInstance().WriteInTrack(midi, channelNumber);

        //    MidiFile.Export("thisshitfuckinworks.mid", MidiService.GetInstance().collection); // СДЕЛАТЬ ОБРАБОТКУ ИСКЛЮЧЕНИЯ КОГДА ФАЙЛ ЗАНЯТ ДРУГОЙ ПРОГРАММОЙ!!!
        //}

        public void PlaybackWindow(Window window)
        {
            window = new PlaybackDEBUG();
            window.Show();
        }

        public void GeneralWindow(Window window)
        {

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

            Grid playback = new Playback().Build();
            playback.SetValue(Grid.RowProperty, 1);
            playback.SetValue(Grid.ColumnProperty, 0);

            Grid playerScrollBar = new PlayerScrollBar().Build();
            playerScrollBar.SetValue(Grid.RowProperty, 2);
            playerScrollBar.SetValue(Grid.ColumnProperty, 0);

            Grid channels = new Channels(PianoRollWindow).Build();
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
