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
            int octavePart = 11;
            for (int keyRow = 0; keyRow < countOfNotes; keyRow++)
            {
                Button key = BuildPianoKey(keyRow + 1);

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

            string content = "";
            int note = countOfNotes - row;
            switch ((note) % 12)
            {
                case 0: { content += "C"; break; }
                case 1: { content += "C#"; break; }
                case 2: { content += "D"; break; }
                case 3: { content += "D#"; break; }
                case 4: { content += "E"; break; }
                case 5: { content += "F"; break; }
                case 6: { content += "F#"; break; }
                case 7: { content += "G"; break; }
                case 8: { content += "G#"; break; }
                case 9: { content += "A"; break; }
                case 10: { content += "A#"; break; }
                case 11: { content += "B"; break; }
            }
            switch ((note) / 12)
            {
                case 0: { content += "-1"; break; }
                case 1: { content += "0"; break; }
                case 2: { content += "1"; break; }
                case 3: { content += "2"; break; }
                case 4: { content += "3"; break; }
                case 5: { content += "4"; break; }
                case 6: { content += "5"; break; }
                case 7: { content += "6"; break; }
                case 8: { content += "7"; break; }
                case 9: { content += "8"; break; }
                case 10: { content += "9"; break; }
            }
            key.Content = /*(note).ToString() + " | " + */content;
            key.HorizontalContentAlignment = HorizontalAlignment.Right;
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
