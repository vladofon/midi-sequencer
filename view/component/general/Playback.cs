using midi_sequencer.service;
using NAudio.Midi;
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

        private const double buttonWidth = 50;
        private const double buttonHeight = 50;
        private const double distanceBetweenButtons = 5;

        private PlaybackService playbackService;

        public Playback()
        {
            playbackService = new PlaybackService();

            brush = (SolidColorBrush)new BrushConverter().ConvertFromString("#2c3d4d");
        }

        public Grid Build()
        {
            Button pauseButton = new()
            {
                Width = buttonWidth,
                Height = buttonHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(distanceBetweenButtons, 0, 0, 0),
                Content = "Pause",
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5f717d"),
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#c4ccd0")
            };

            Button playButton = new()
            {
                Width = buttonWidth,
                Height = buttonHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(buttonWidth + distanceBetweenButtons * 2, 0, 0, 0),
                Content = "Play",
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5f717d"),
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#c4ccd0")
            };

            Button stopButton = new()
            {
                Width = buttonWidth,
                Height = buttonHeight,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(buttonWidth * 2 + distanceBetweenButtons * 3, 0, 0, 0),
                Content = "Stop",
                Background = (SolidColorBrush)new BrushConverter().ConvertFromString("#5f717d"),
                Foreground = (SolidColorBrush)new BrushConverter().ConvertFromString("#c4ccd0")
            };

            pauseButton.Click += pauseButton_Click;
            playButton.Click += playButton_Click;
            stopButton.Click += stopButton_Click;

            Grid grid = new();

            grid.Children.Add(pauseButton);
            grid.Children.Add(playButton);
            grid.Children.Add(stopButton);

            grid.Background = brush;

            return grid;
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            playbackService.Pause();
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            playbackService.Play();
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            playbackService.Stop();
        }
    }
}
