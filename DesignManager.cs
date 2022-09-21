using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace midi_sequencer
{
    internal class DesignManager
    {
        public void DrawPianoRoll(Window window)
        {
            int countOfNotes = 127;
            int pianoWidth = 100;

            Thickness START_POSITION = new(0, 0, 0, 0);
            int noteBtnWidth = 15;
            int noteBtnHeight = 10;

            Grid grid = new();
            //grid.Width = window.Width;
            //grid.Height = window.Height;
            grid.Background = Brushes.Gray;
            //myStackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            //myStackPanel.VerticalAlignment = VerticalAlignment.Center;

            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scroll.Content = grid;

            window.Content = scroll;

            for (int col = 0; col < countOfNotes; col++)
            {
                for (int row = 0; row < pianoWidth; row++)
                {
                    Button noteCell = new Button();
                    Thickness location = new Thickness(
                        START_POSITION.Left + (row * noteBtnWidth),
                        START_POSITION.Top + (col * noteBtnHeight), 0, 0
                    );

                    noteCell.Width = noteBtnWidth;
                    noteCell.Height = noteBtnHeight;
                    noteCell.Margin = location;
                    noteCell.HorizontalAlignment = HorizontalAlignment.Left;
                    noteCell.VerticalAlignment = VerticalAlignment.Top;


                    grid.Children.Add(noteCell);
                }
            }
        }
    }
}
