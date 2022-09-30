using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace midi_sequencer.view.component.piano_roll
{
    internal class PianoKeys : Component
    {
        private int countOfNotes;
        private int noteButtonHeight;
        private Grid pianoKeysGrid;

        private Brush white = Brushes.WhiteSmoke;
        private Brush black = Brushes.Black;

        public PianoKeys(int countOfNotes, int noteButtonHeight)
        {
            this.countOfNotes = countOfNotes;
            this.noteButtonHeight = noteButtonHeight;
            this.pianoKeysGrid = new Grid();
        }

        public Grid Build()
        {
            int octavePart = 12;
            for (int keyRow = 1; keyRow < countOfNotes; keyRow++)
            {
                Button key = BuildPianoKey(keyRow);

                if (octavePart != 6)
                {
                    if (octavePart % 2 == 0)
                    {
                        key.Background = this.black;
                        key.Foreground = this.white;
                    }
                    octavePart++;
                }
                else
                {
                    octavePart += 2;
                }

                if (octavePart == 14) octavePart = 1;

                this.pianoKeysGrid.Children.Add(key);
            }

            return this.pianoKeysGrid;
        }

        private Button BuildPianoKey(int row)
        {
            Button key = new Button();
            key.Content = countOfNotes - row;
            key.Background = white;
            key.Width = this.pianoKeysGrid.Width;
            key.Height = this.noteButtonHeight;
            key.Margin = new Thickness(0, this.noteButtonHeight * (row - 1), 0, 0);
            key.HorizontalAlignment = HorizontalAlignment.Stretch;
            key.VerticalAlignment = VerticalAlignment.Top;

            return key;
        }
    }
}
