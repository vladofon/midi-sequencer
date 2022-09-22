using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace midi_sequencer.view.component
{
    internal class PianoRoll : Component
    {
        private Button currentPressedNote = new Button();
        private Point mouseDown = new(0, 0);

        private const int noteBtnWidth = 20;
        private const int noteBtnHeight = 12;

        private const int countOfNotes = 127;
        private const int pianoWidth = 100;

        private Grid pianoGrid = new();

        public Grid Draw()
        {
            pianoGrid.Background = Brushes.Gray;
            pianoGrid.MouseDown += Piano_MouseDown;
            pianoGrid.MouseUp += Piano_MouseUp;

            return this.pianoGrid;
        }

        private void Piano_MouseDown(object sender, MouseButtonEventArgs e)
        {

            Point p = e.GetPosition(pianoGrid);
            double x = p.X;
            double y = p.Y;

            int left = (int)x - (int)x % noteBtnWidth;
            int top = (int)y - (int)y % noteBtnHeight;

            this.mouseDown.X = left;
            this.mouseDown.Y = top;

            currentPressedNote = new Button();
            currentPressedNote.Margin = new Thickness(left, top, 0, 0);
            currentPressedNote.Width = noteBtnWidth;
            currentPressedNote.Height = noteBtnHeight;
            currentPressedNote.HorizontalAlignment = HorizontalAlignment.Left;
            currentPressedNote.VerticalAlignment = VerticalAlignment.Top;

            currentPressedNote.PreviewMouseDown += note_MouseDown;
            currentPressedNote.PreviewMouseUp += note_MouseUp;

            pianoGrid.Children.Add(currentPressedNote);
        }

        private void Piano_MouseUp(object sender, MouseButtonEventArgs e)
        {
            buildNoteCell(e.GetPosition(pianoGrid).X);
        }

        void buildNoteCell(double mouseUp_X)
        {
            if (mouseUp_X <= this.mouseDown.X) return;

            int left = (int)mouseUp_X - (int)mouseUp_X % noteBtnWidth;

            if (currentPressedNote.Width == noteBtnWidth)
            {
                currentPressedNote.Width += left - (int)this.mouseDown.X;
            }
            else
            {
                currentPressedNote.Width += left - (int)this.mouseDown.X - currentPressedNote.Width + noteBtnWidth;
            }
        }

        void note_MouseUp(object sender, MouseButtonEventArgs e)
        {
            buildNoteCell(e.GetPosition(pianoGrid).X);
        }

        void note_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.currentPressedNote = (Button)sender;
            this.mouseDown.X = (int)currentPressedNote.Margin.Left;
            this.mouseDown.Y = (int)currentPressedNote.Margin.Top;
        }
    }
}
