using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace midi_sequencer.view.component.general
{
    internal class Tray : Component
    {
        private Brush brush;

        public Tray()
        {
            brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#36495a");
        }

        public Grid Build()
        {
            Grid grid = new();
            grid.Background = brush;

            return grid;
        }
    }
}
