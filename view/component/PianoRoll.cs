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
        private Point mouseDownOnBuild = new(0, 0);
        private Point mouseDownOnMove = new(0, 0);
        private bool isNotePressed = false;
        private bool isMouseAndNoteLocationSync = false;

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

            this.mouseDownOnBuild.X = left;
            this.mouseDownOnBuild.Y = top;

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
            this.notes[(int)this.mouseDownOnBuild.Y / noteBtnHeight].Add(currentPressedNote);

            this.isNotePressed = true;
        }

        private void Piano_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ManipulateNote(e.GetPosition(pianoGrid));
            this.isNotePressed = false;
            this.isMouseAndNoteLocationSync = false;
        }

        void ManipulateNote(Point mousePosition)
        {
            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteBtnWidth;

            int noteCellEdge = (int)(currentPressedNote.Margin.Left + currentPressedNote.Width);

            if (fixedMousePosition >= noteCellEdge - noteBtnWidth * 2 && fixedMousePosition <= noteCellEdge)
            {
                BuildNoteCell(mousePosition);
            }
            else if (this.isMouseAndNoteLocationSync)
            {
                MoveNote(mousePosition);
            }

            if (!isMouseAndNoteLocationSync)
            {
                Point currentNoteLocation = new Point(fixedMousePosition, this.currentPressedNote.Margin.Top);
                this.mouseDownOnMove = currentNoteLocation;
                isMouseAndNoteLocationSync = true;
            }
        }

        bool isIntersect(int left, int right)
        {
            int noteTrack = (int)this.currentPressedNote.Margin.Top / noteBtnHeight;

            Predicate<Button> isSameButton = i => (!i.Equals(currentPressedNote));
            Predicate<Button> hasIntersectRight = i => (right > i.Margin.Left && left < i.Margin.Left) && isSameButton.Invoke(i);
            Predicate<Button> hasIntersectLeft = i => (left < i.Margin.Left + i.Width && left > i.Margin.Left) && isSameButton.Invoke(i);
            Predicate<Button> hasIntersectWhole = i => (left <= i.Margin.Left && right >= i.Margin.Left + i.Width) && isSameButton.Invoke(i);

            bool isIntersectRight = notes[noteTrack].Find(hasIntersectRight) != null;
            bool isIntersectLeft = notes[noteTrack].Find(hasIntersectLeft) != null;
            bool isIntersectWhole = notes[noteTrack].Find(hasIntersectWhole) != null;

            return isIntersectRight || isIntersectLeft || isIntersectWhole;
        }

        void BuildNoteCell(Point mousePosition)
        {
            if (mousePosition.X <= this.mouseDownOnBuild.X) return;

            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteBtnWidth;

            int addition;
            if (currentPressedNote.Width == noteBtnWidth)
            {
                addition = fixedMousePosition - (int)this.mouseDownOnBuild.X;
            }
            else
            {
                addition = (int)(fixedMousePosition - (int)this.mouseDownOnBuild.X - currentPressedNote.Width + noteBtnWidth);
            }

            int left = (int)currentPressedNote.Margin.Left;
            int right = left + (int)(currentPressedNote.Width + addition);

            if (isIntersect(left, right)) return;

            this.currentPressedNote.Width += addition;

        }

        void MoveNote(Point mousePosition)
        {
            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteBtnWidth;

            Thickness currentMargin = currentPressedNote.Margin;
            Thickness upadatedMargin = new Thickness(
                currentMargin.Left += fixedMousePosition - this.mouseDownOnMove.X,
                currentMargin.Top, 0, 0
             );

            int left = (int)upadatedMargin.Left;
            int right = left + (int)(currentPressedNote.Width);

            if (isIntersect(left, right)) return;

            currentPressedNote.Margin = upadatedMargin;
            this.mouseDownOnMove = new Point(fixedMousePosition, currentPressedNote.Margin.Top);
        }

        void Note_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ManipulateNote(e.GetPosition(pianoGrid));
            this.isNotePressed = false;
            this.isMouseAndNoteLocationSync = false;
            this.noteTrace.Clear();
        }

        void Note_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.currentPressedNote = (Button)sender;
            this.mouseDownOnBuild.X = (int)currentPressedNote.Margin.Left;
            this.mouseDownOnBuild.Y = (int)currentPressedNote.Margin.Top;
            this.isNotePressed = true;
        }

        void Piano_MouseMove(object sender, MouseEventArgs e)
        {

            if (this.isNotePressed)
            {
                ManipulateNote(e.GetPosition(pianoGrid));
            }
        }

        void Note_MouseMove(object sender, MouseEventArgs e)
        {

            if (this.isNotePressed)
            {
                ManipulateNote(e.GetPosition(pianoGrid));
            }
        }
    }
}
