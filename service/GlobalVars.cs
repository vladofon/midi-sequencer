using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace midi_sequencer.service
{
    static class GlobalVars
    {
        public static int dbgInt = 10;
        public static MidiOut midiOut;
        //public static MidiEventCollection midiEventCollection = PlaybackService.OpenFile("C:\\Users\\kosty\\source\\repos\\midi-sequencer\\Test MIDI files\\d_dead\\d_dead.mid");
        //public static MidiEventCollection midiEventCollection = new(1, 128);
        public static MidiEventCollection midiEventCollection;

    }
}
