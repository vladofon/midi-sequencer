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

        private Button addChannelButton;

        private List<Channel> channels = new();

        public Channels()
        {
            brush = Brushes.Yellow;

            addChannelButton = new();
            addChannelButton.Height = 10;
            addChannelButton.Width = 10;
            addChannelButton.Click += addChannelButton_Click;
        }

        public Grid Build()
        {
            Grid grid = new();
            grid.Background = brush;
            grid.Children.Add(addChannelButton);
            foreach (var channel in channels)
            {
                grid.Children.Add(channel.Build());
            }

            return grid;
        }

        private void addChannelButton_Click(object sender, RoutedEventArgs e)
        {
            channels.Add(new Channel());
        }
    }
}
