using midi_sequencer.view.component.piano_roll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace midi_sequencer.view.component
{
    internal class PianoRoll : Component
    {
        private const int noteButtonWidth = 20;
        private const int noteButtonHeight = 20;
        private const int countOfNotes = 127;

        private Grid pianoRoll;

        private Component pianoKeysBuilder;
        private Component notesGridBuilder;

        public PianoRoll()
        {
            this.pianoKeysBuilder = new PianoKeys(countOfNotes, noteButtonHeight);
            this.notesGridBuilder = new NotesGrid(countOfNotes, noteButtonHeight, noteButtonWidth);
            this.pianoRoll = new Grid();
        }

        public Grid Build()
        {
            ColumnDefinition firstColumn = new ColumnDefinition();
            firstColumn.Width = new GridLength(100);

            ColumnDefinition secondColumn = new ColumnDefinition();
            RowDefinition secondRow = new RowDefinition();

            Grid pianoKeys = pianoKeysBuilder.Build();
            pianoKeys.SetValue(Grid.ColumnProperty, 0);

            Grid notesGrid = notesGridBuilder.Build();
            notesGrid.SetValue(Grid.ColumnProperty, 1);

            this.pianoRoll.ColumnDefinitions.Add(firstColumn);
            this.pianoRoll.ColumnDefinitions.Add(secondColumn);
            this.pianoRoll.RowDefinitions.Add(secondRow);
            this.pianoRoll.Children.Add(notesGrid);
            this.pianoRoll.Children.Add(pianoKeys);

            return pianoRoll;
        }
    }
}
