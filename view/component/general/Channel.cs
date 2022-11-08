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

        //private PianoRollWindow pianoRollWindow;
        //private PianoRoll pianoRoll;

        private ComboBox patchSelect;
        private Button openPianoRoll;

        //public delegate void DPianoRollWindow(int channelNumber);
        private DesignManager.DPianoRollWindow dpianoRollWindow;

        public Channel(int channelNumber, DesignManager.DPianoRollWindow del)
        {
            dpianoRollWindow = del;

            brush = Brushes.Gray;
            this.channelNumber = channelNumber;
        }

        public Grid Build()
        {
            Grid grid = new();

            patchSelect = new ComboBox();
            for (int i = 0; i < 128; i++)
            {
                patchSelect.Items.Add(NAudio.Midi.PatchChangeEvent.GetPatchName(i));
            }
            patchSelect.SelectedIndex = 0;
            patchSelect.Width = 100;
            patchSelect.HorizontalAlignment = HorizontalAlignment.Left;
            patchSelect.SelectionChanged += PatchSelect_SelectionChanged;

            openPianoRoll = new Button();
            openPianoRoll.Content = "Open piano roll on " + channelNumber + " channel";
            openPianoRoll.Width = 150;
            openPianoRoll.HorizontalAlignment = HorizontalAlignment.Right;
            openPianoRoll.Click += OpenPianoRoll_Click;

            grid.Children.Add(patchSelect);
            grid.Children.Add(openPianoRoll);

            grid.Background = brush;

            return grid;
        }

        private void PatchSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MidiService.GetInstance().ChangeInstrumentOnTrack(patchSelect.SelectedIndex, channelNumber);
            MessageBox.Show("patch changed");
        }

        private void OpenPianoRoll_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            //this.pianoRollWindow = new PianoRollWindow();
            //pianoRollWindow.Closed += PianoRollWindow_Closed;

            //Grid view = new();

            //pianoRoll = new PianoRoll(channelNumber);
            //Grid pianoRollGrid = pianoRoll.Build();

            //view.Children.Add(pianoRollGrid);

            //pianoRollWindow.Content = view;

            //pianoRollWindow.Show();

            dpianoRollWindow(channelNumber);
        }

        //private void PianoRollWindow_Closed(object? sender, EventArgs e)
        //{
        //    MessageBox.Show("piano roll on channel " + channelNumber + " closed");

        //    MidiEventMapper mapper = new();
        //    List<MidiEvent> midi = mapper.mapAll(this.pianoRoll.GetNoteButtons());

        //    MidiService.GetInstance().WriteInTrack(midi, channelNumber);

        //    MidiFile.Export("thisshitfuckinworks.mid", MidiService.GetInstance().collection); // СДЕЛАТЬ ОБРАБОТКУ ИСКЛЮЧЕНИЯ КОГДА ФАЙЛ ЗАНЯТ ДРУГОЙ ПРОГРАММОЙ!!!
        //}
    }
}
