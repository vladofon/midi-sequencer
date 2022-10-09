using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace midi_sequencer.view.component.general
{
    internal class Playback : Component
    {
        private Brush brush;

        public Playback()
        {
            brush = Brushes.Blue;
        }

        public Grid Build()
        {
            Grid grid = new();
            grid.Background = brush;

            return grid;
        }
    }
}
