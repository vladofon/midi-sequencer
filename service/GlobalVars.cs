using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace midi_sequencer.service
{
    internal static class GlobalVars
    {
        static public MidiOut midiOut = new(0);
        static public MidiEventCollection midiEventCollection = new(1, 128);
    }
}
