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
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace midi_sequencer.view.component.general
{
    internal class Channel : Component
    {
        private Brush brush;

        public int channelNumber;

        private PianoRollWindow pianoRollWindow;
        private PianoRoll pianoRoll;

        private ComboBox patchSelect;
        private Button openPianoRoll;

        public Channel(int channelNumber)
        {
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
            //patchSelect.Width = 100;
            //patchSelect.HorizontalAlignment = HorizontalAlignment.Center;
            patchSelect.Margin = new Thickness(5, 5, 5, 5);
            patchSelect.VerticalAlignment = VerticalAlignment.Top;
            patchSelect.Height = 25;
            patchSelect.SelectionChanged += PatchSelect_SelectionChanged;

            patchSelect.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#f8634d");
            patchSelect.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#dcdcc8");
            patchSelect.BorderBrush = (SolidColorBrush)new BrushConverter().ConvertFromString("#f8634d");
            patchSelect.Loaded += combobox_Loaded;




            openPianoRoll = new Button();
            openPianoRoll.Content = "Open piano roll on " + channelNumber + " channel";
            //openPianoRoll.Width = 150;
            //openPianoRoll.HorizontalAlignment = HorizontalAlignment.Right;
            openPianoRoll.Margin = new Thickness(5, patchSelect.Height + 10, 5, 5);
            openPianoRoll.Click += OpenPianoRoll_Click;
            openPianoRoll.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#122029");
            openPianoRoll.Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#728395");

            grid.Children.Add(patchSelect);
            grid.Children.Add(openPianoRoll);

            grid.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#2c3d4d");

            return grid;
        }

        private void combobox_Loaded(object sender, RoutedEventArgs e)
        {
            Popup popup = FindVisualChildByName<Popup>((sender as DependencyObject), "PART_Popup");
            Border border = FindVisualChildByName<Border>(popup.Child, "DropDownBorder");
            border.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#f8634d");
        }

        private T FindVisualChildByName<T>(DependencyObject parent, string name) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                string controlName = child.GetValue(Control.NameProperty) as string;
                if (controlName == name)
                {
                    return child as T;
                }
                else
                {
                    T result = FindVisualChildByName<T>(child, name);
                    if (result != null)
                        return result;
                }
            }
            return null;
        }

        private void Combo_OnLoaded(object sender, RoutedEventArgs e)
        {
            var popup = (Popup)patchSelect.Template.FindName("PART_Popup", patchSelect);
            if (popup != null)
            {
                var parent = (Border)popup.Parent;
                parent.Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#ff0000");
            }
        }

        private void PatchSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MidiService.GetInstance().ChangeInstrumentOnTrack(patchSelect.SelectedIndex, channelNumber);
            MessageBox.Show("patch changed");
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

            MidiService.GetInstance().WriteInTrack(midi, channelNumber);

            MidiFile.Export("thisshitfuckinworks.mid", MidiService.GetInstance().collection); // СДЕЛАТЬ ОБРАБОТКУ ИСКЛЮЧЕНИЯ КОГДА ФАЙЛ ЗАНЯТ ДРУГОЙ ПРОГРАММОЙ!!!
        }
    }
}
