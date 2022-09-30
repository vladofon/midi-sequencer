using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace midi_sequencer.view.component.piano_roll
{
    internal class NotesGrid : Component
    {
        private Button currentPressedNote;

        private Point mouseDownOnBuild;
        private Point mouseDownOnMove;

        private bool isNotePressed;
        private bool isMouseAndNoteLocationSync;

        private Dictionary<int, List<Button>> notes;
        //private List<Button> noteTrace;

        private int noteButtonWidth;
        private int noteButtonHeight;
        private int countOfNotes;

        private Grid notesGrid;
        private Brush bgColor;

        public NotesGrid(int countOfNotes, int noteButtonWidth, int noteButtonHeight)
        {
            this.currentPressedNote = new Button();
            this.mouseDownOnBuild = new Point(0, 0);
            this.mouseDownOnMove = new Point(0, 0);
            this.isNotePressed = false;
            this.isMouseAndNoteLocationSync = false;
            this.notes = new Dictionary<int, List<Button>>();
            //this.noteTrace = new List<button>();
            this.noteButtonWidth = noteButtonWidth;
            this.noteButtonHeight = noteButtonHeight;
            this.countOfNotes = countOfNotes;
            this.notesGrid = new Grid();
            this.bgColor = Brushes.Gray;

            InitPianoGrid();
        }

        public Grid Build()
        {
            this.notesGrid.Background = bgColor;
            this.notesGrid.MouseDown += Piano_MouseDown;
            this.notesGrid.MouseUp += Piano_MouseUp;
            this.notesGrid.MouseMove += Piano_MouseMove;
            this.notesGrid.HorizontalAlignment = HorizontalAlignment.Stretch;
            this.notesGrid.VerticalAlignment = VerticalAlignment.Stretch;

            return notesGrid;
        }

        public Button BuildNoteButton(int left, int top)
        {
            Button noteButton = new Button();
            noteButton.Margin = new Thickness(left, top, 0, 0);
            noteButton.Width = noteButtonWidth;
            noteButton.Height = noteButtonHeight;
            noteButton.HorizontalAlignment = HorizontalAlignment.Left;
            noteButton.VerticalAlignment = VerticalAlignment.Top;

            noteButton.PreviewMouseDown += Note_MouseDown;
            noteButton.PreviewMouseUp += Note_MouseUp;
            noteButton.PreviewMouseMove += Note_MouseMove;

            return noteButton;
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

            Point p = e.GetPosition(notesGrid);
            double x = p.X;
            double y = p.Y;

            int left = (int)x - (int)x % noteButtonWidth;
            int top = (int)y - (int)y % noteButtonHeight;

            this.mouseDownOnBuild.X = left;
            this.mouseDownOnBuild.Y = top;

            this.currentPressedNote = BuildNoteButton(left, top);

            this.notesGrid.Children.Add(currentPressedNote);
            this.notes[(int)mouseDownOnBuild.Y / noteButtonHeight].Add(currentPressedNote);

            this.isNotePressed = true;
        }

        private void Piano_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ManipulateNote(e.GetPosition(notesGrid));
            this.isNotePressed = false;
            this.isMouseAndNoteLocationSync = false;
        }

        void ManipulateNote(Point mousePosition)
        {
            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteButtonWidth;

            int noteCellEdge = (int)(currentPressedNote.Margin.Left + currentPressedNote.Width);

            if (fixedMousePosition >= noteCellEdge - noteButtonWidth * 2 && fixedMousePosition <= noteCellEdge)
            {
                BuildNoteButton(mousePosition);
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

        bool IsIntersect(int left, int right)
        {
            int noteTrack = (int)this.currentPressedNote.Margin.Top / noteButtonHeight;

            Predicate<Button> isSameButton = i => (!i.Equals(currentPressedNote));
            Predicate<Button> hasIntersectRight = i => (right > i.Margin.Left && left < i.Margin.Left) && isSameButton.Invoke(i);
            Predicate<Button> hasIntersectLeft = i => (left < i.Margin.Left + i.Width && left > i.Margin.Left) && isSameButton.Invoke(i);
            Predicate<Button> hasIntersectOver = i => (left <= i.Margin.Left && right >= i.Margin.Left + i.Width) && isSameButton.Invoke(i);
            Predicate<Button> hasIntersectInside = i => (left >= i.Margin.Left && right <= i.Margin.Left + i.Width) && isSameButton.Invoke(i);

            bool isIntersectRight = notes[noteTrack].Find(hasIntersectRight) != null;
            bool isIntersectLeft = notes[noteTrack].Find(hasIntersectLeft) != null;
            bool isIntersectOver = notes[noteTrack].Find(hasIntersectOver) != null;
            bool isIntersectInside = notes[noteTrack].Find(hasIntersectInside) != null;
            bool isIntersectBorder = left < 0;

            return isIntersectRight || isIntersectLeft || isIntersectOver || isIntersectInside || isIntersectBorder;
        }

        void BuildNoteButton(Point mousePosition)
        {
            if (mousePosition.X <= this.mouseDownOnBuild.X) return;

            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteButtonWidth;

            int addition;
            if (currentPressedNote.Width == noteButtonWidth)
            {
                addition = fixedMousePosition - (int)this.mouseDownOnBuild.X;
            }
            else
            {
                addition = (int)(fixedMousePosition - (int)this.mouseDownOnBuild.X - currentPressedNote.Width + noteButtonWidth);
            }

            int left = (int)currentPressedNote.Margin.Left;
            int right = left + (int)(currentPressedNote.Width + addition);

            if (IsIntersect(left, right)) return;

            this.currentPressedNote.Width += addition;

        }

        void MoveNote(Point mousePosition)
        {
            int fixedMousePosition = (int)mousePosition.X - (int)mousePosition.X % noteButtonWidth;

            Thickness currentMargin = currentPressedNote.Margin;
            Thickness upadatedMargin = new Thickness(
                currentMargin.Left += fixedMousePosition - this.mouseDownOnMove.X,
                currentMargin.Top, 0, 0
             );

            int left = (int)upadatedMargin.Left;
            int right = left + (int)(currentPressedNote.Width);

            if (IsIntersect(left, right)) return;

            currentPressedNote.Margin = upadatedMargin;
            this.mouseDownOnMove = new Point(fixedMousePosition, currentPressedNote.Margin.Top);
        }

        void Note_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ManipulateNote(e.GetPosition(notesGrid));
            this.isNotePressed = false;
            this.isMouseAndNoteLocationSync = false;
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
                ManipulateNote(e.GetPosition(notesGrid));
            }
        }

        void Note_MouseMove(object sender, MouseEventArgs e)
        {

            if (this.isNotePressed)
            {
                ManipulateNote(e.GetPosition(notesGrid));
            }
        }
    }
}

