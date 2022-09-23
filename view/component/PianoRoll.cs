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
        private Button currentPressedNote = new Button();
        private Point mouseDown = new(0, 0);
        private bool isNotePressed = false;

        private Dictionary<int, List<Button>> notes = new();
        private List<Button> noteTrace = new();

        private const int noteBtnWidth = 20;
        private const int noteBtnHeight = 12;

        private const int countOfNotes = 127;
        private const int pianoWidth = 100;

        private Grid pianoGrid = new();

        public PianoRoll()
        {
            InitPianoGrid();
        }

        public Grid Draw()
        {
            pianoGrid.Background = Brushes.Gray;
            pianoGrid.MouseDown += Piano_MouseDown;
            pianoGrid.MouseUp += Piano_MouseUp;
            pianoGrid.MouseMove += Piano_MouseMove;

            return this.pianoGrid;
        }

        private void InitPianoGrid()
        {
            for (int noteRow = 1; noteRow < countOfNotes; noteRow++)
            {
                this.notes.Add(noteRow, new List<Button>());
            }
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

            currentPressedNote.PreviewMouseDown += Note_MouseDown;
            currentPressedNote.PreviewMouseUp += Note_MouseUp;
            currentPressedNote.PreviewMouseMove += Note_MouseMove;

            pianoGrid.Children.Add(currentPressedNote);
            this.notes[(int)this.mouseDown.Y / noteBtnHeight].Add(currentPressedNote);
            //currentPressedNote.Content = "" + (int)this.mouseDown.Y / noteBtnHeight;
            //currentPressedNote.Content = "" + currentPressedNote.Margin.Left;

            this.isNotePressed = true;
        }

        private void Piano_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BuildNoteCell(e.GetPosition(pianoGrid));
            this.isNotePressed = false;
        }

        void BuildNoteCell(Point mousePosition)
        {
            if (mousePosition.X <= this.mouseDown.X) return;

            int left = (int)mousePosition.X - (int)mousePosition.X % noteBtnWidth;

            bool isIntersect = notes[(int)this.currentPressedNote.Margin.Top / noteBtnHeight].Find(
                i => (this.currentPressedNote.Margin.Left <= i.Margin.Left && left >= i.Margin.Left && !i.Equals(currentPressedNote))
            ) != null;

            if (isIntersect) return;

            if (currentPressedNote.Width == noteBtnWidth)
            {
                currentPressedNote.Width += left - (int)this.mouseDown.X;
            }
            else
            {
                currentPressedNote.Width += left - (int)this.mouseDown.X - currentPressedNote.Width + noteBtnWidth;
            }
        }

        void Note_MouseUp(object sender, MouseButtonEventArgs e)
        {
            BuildNoteCell(e.GetPosition(pianoGrid));
            this.isNotePressed = false;
            this.noteTrace.Clear();
        }

        void Note_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.currentPressedNote = (Button)sender;
            this.mouseDown.X = (int)currentPressedNote.Margin.Left;
            this.mouseDown.Y = (int)currentPressedNote.Margin.Top;
            this.isNotePressed = true;
        }

        void Piano_MouseMove(object sender, MouseEventArgs e)
        {

            if (this.isNotePressed)
            {
                BuildNoteCell(e.GetPosition(pianoGrid));
            }
        }

        void Note_MouseMove(object sender, MouseEventArgs e)
        {

            if (this.isNotePressed)
            {
                BuildNoteCell(e.GetPosition(pianoGrid));
            }
        }
    }
}
