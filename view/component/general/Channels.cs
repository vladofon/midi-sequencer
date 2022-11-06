using midi_sequencer.service;
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

        private List<Channel> channels;

        public Channels()
        {
            channels = new List<Channel>();
            for (int i = 0; i < 16; i++)
            {
                channels.Add(new Channel(i));
            }

            brush = Brushes.Yellow;
        }

        public Grid Build()
        {
            Grid grid = new();

            ColumnDefinition firstCol = new();
            ColumnDefinition secondCol = new();
            ColumnDefinition thirdCol = new();
            ColumnDefinition fourthCol = new();
            RowDefinition firstRow = new();
            RowDefinition secondRow = new();
            RowDefinition thirdRow = new();
            RowDefinition fourthRow = new();

            grid.ColumnDefinitions.Add(firstCol);
            grid.ColumnDefinitions.Add(secondCol);
            grid.ColumnDefinitions.Add(thirdCol);
            grid.ColumnDefinitions.Add(fourthCol);
            grid.RowDefinitions.Add(firstRow);
            grid.RowDefinitions.Add(secondRow);
            grid.RowDefinitions.Add(thirdRow);
            grid.RowDefinitions.Add(fourthRow);

            int x = 0, y = 0;
            foreach (var channel in channels)
            {
                Grid channelGrid = channel.Build();

                channelGrid.SetValue(Grid.RowProperty, x++);
                channelGrid.SetValue(Grid.ColumnProperty, y);
                if (x >= 4)
                {
                    x = 0; y++;
                }

                grid.Children.Add(channelGrid);
            }

            grid.Background = brush;
            return grid;
        }
    }
}
