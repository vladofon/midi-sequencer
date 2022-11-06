using midi_sequencer.service;
using midi_sequencer.view.component.piano_roll;
using midi_sequencer.windows;
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
    internal class Channel : Component
    {
        private Brush brush;

        public int channelNumber;
        private MidiService midiService;

        private PianoRollWindow pianoRollWindow;
        PianoRoll pianoRoll;

        public Channel(int channelNumber, MidiService midiService)
        {
            brush = Brushes.Gray;
            this.channelNumber = channelNumber;
            this.midiService = midiService;
        }

        public Grid Build()
        {
            Grid grid = new();

            Button openPianoRoll = new Button();
            openPianoRoll.Content = "Open piano roll on " + channelNumber + " channel";
            openPianoRoll.Click += OpenPianoRoll_Click;

            grid.Children.Add(openPianoRoll);

            grid.Background = brush;

            return grid;
        }

        private void OpenPianoRoll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            this.pianoRollWindow = new PianoRollWindow();
            pianoRollWindow.Closed += PianoRollWindow_Closed;

            Grid view = new();

            pianoRoll = new PianoRoll(channelNumber);
            Grid pianoRollGrid = pianoRoll.Build();

            view.Children.Add(pianoRollGrid);

            pianoRollWindow.Content = view;

            pianoRollWindow.Show();
        }

        private void PianoRollWindow_Closed(object? sender, EventArgs e)
        {
            MessageBox.Show("piano roll on channel " + channelNumber + " closed");

            MidiEventMapper mapper = new();
            List<MidiEvent> midi = mapper.mapAll(this.pianoRoll.GetNoteButtons());

            

            midiService.collection.AddTrack(midi); //!!!
            //midiService.AppendEndMarker(midiService.collection[17]);

            //midiService.collection.PrepareForExport();
            MidiFile.Export("thisshitfuckinworks.mid", midiService.collection);
        }
    }
}
