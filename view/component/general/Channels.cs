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
    internal class Channels : Component
    {
        private Brush brush;

        private List<Channel> channels = new();

        public Channels()
        {
            brush = Brushes.Yellow;
        }

        public Grid Build()
        {
            Grid grid = new();
            grid.Background = brush;
            foreach (var channel in channels)
            {
                grid.Children.Add(channel.Build());
            }

            return grid;
        }
    }
}
