using midi_sequencer.view.component;
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
        public void DrawPianoRoll(Window window)
        {
            window.Show();
            Component pianoRoll = new PianoRoll();

            ScrollViewer scroll = new ScrollViewer();
            scroll.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            scroll.Content = pianoRoll.Draw();

            window.Content = scroll;
        }
    }
}
