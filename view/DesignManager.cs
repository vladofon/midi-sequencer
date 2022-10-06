using midi_sequencer.model;
using midi_sequencer.service;
using midi_sequencer.view.component;
using midi_sequencer.view.component.piano_roll;
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

namespace midi_sequencer
{
    internal class DesignManager
    {
        PianoRoll pianoRoll = new PianoRoll(1);

        public void DrawPianoRoll(Window window)
        {
            window.Show();

            Grid view = new();
            ColumnDefinition firstCol = new();
            RowDefinition firstRow = new RowDefinition();
            firstRow.Height = new GridLength(30);
            RowDefinition secondRow = new RowDefinition();

            view.ColumnDefinitions.Add(firstCol);
            view.RowDefinitions.Add(firstRow);
            view.RowDefinitions.Add(secondRow);

            Grid tbt = new PianoRollTray(pianoRoll).Build();
            tbt.SetValue(Grid.RowProperty, 0);
            tbt.SetValue(Grid.ColumnProperty, 0);

            Grid pr = pianoRoll.Build();
            pr.VerticalAlignment = VerticalAlignment.Bottom;
            pr.SetValue(Grid.RowProperty, 1);
            pr.SetValue(Grid.ColumnProperty, 0);

            view.Children.Add(tbt);
            view.Children.Add(pr);

            window.Content = view;
        }
    }
}
