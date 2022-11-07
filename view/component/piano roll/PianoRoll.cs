using midi_sequencer.model;
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

namespace midi_sequencer.view.component.piano_roll
{
    internal class PianoRoll : Component
    {
        private const int noteButtonWidth = 20;
        private const int noteButtonHeight = 20;
        private const int countOfNotes = 127;
        private readonly int channel;

        private Grid pianoRoll;

        private PianoKeys pianoKeysBuilder;
        private NotesGrid notesGridBuilder;

        public PianoRoll(int channel)
        {
            this.channel = channel;

            this.pianoKeysBuilder = new PianoKeys(countOfNotes, noteButtonHeight);
            this.notesGridBuilder = new NotesGrid(countOfNotes, noteButtonHeight, noteButtonWidth, this.channel);
            this.pianoRoll = new Grid();
        }

        public List<NoteButton> GetNoteButtons()
        {
            return this.notesGridBuilder.GetNotes();
        }

        public Grid Build()
        {

            ColumnDefinition firstColumn = new ColumnDefinition();
            firstColumn.Width = new GridLength(100);

            ColumnDefinition secondColumn = new ColumnDefinition();

            RowDefinition firstRow = new RowDefinition();
            RowDefinition secondRow = new RowDefinition();

            Grid pianoKeys = pianoKeysBuilder.Build();
            pianoKeys.SetValue(Grid.ColumnProperty, 0);
            pianoKeys.SetValue(Grid.RowProperty, 1);

            Grid notesGrid = notesGridBuilder.Build();
            notesGrid.SetValue(Grid.ColumnProperty, 1);
            notesGrid.SetValue(Grid.RowProperty, 1);

            this.pianoRoll.ColumnDefinitions.Add(firstColumn);
            this.pianoRoll.ColumnDefinitions.Add(secondColumn);
            this.pianoRoll.RowDefinitions.Add(firstRow);
            this.pianoRoll.RowDefinitions.Add(secondRow);
            this.pianoRoll.Children.Add(notesGrid);
            this.pianoRoll.Children.Add(pianoKeys);

            ScrollViewer scroll = new ScrollViewer();
            //scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;

            scroll.Content = this.pianoRoll;
            Grid window = new Grid();
            window.Children.Add(scroll);

            //this.pianoRoll.Children.Add(tray);

            return window;
        }
    }
}

// TODO SEPARATE SCROLL_VIEW NOTES_GRID INTO NotesGrid CLASS
// TODO SCROLL FROM CODE
