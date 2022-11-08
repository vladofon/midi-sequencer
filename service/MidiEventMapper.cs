using midi_sequencer.model;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace midi_sequencer.service
{
    internal class MidiEventMapper
    {
        public List<MidiEvent> map(NoteButton noteButton)
        {
            //MessageBox.Show(noteButton.GetNoteNumber().ToString());

            NoteOnEvent midiEvent = new NoteOnEvent(
                noteButton.GetAbsoluteTime(),
                noteButton.GetChannel(),
                noteButton.GetNoteNumber(),
                127,
                noteButton.GetDuration()
            );

            NoteOnEvent off = new(noteButton.GetAbsoluteTime() + noteButton.GetDuration(), noteButton.GetChannel(), noteButton.GetNoteNumber(), 0, 0);

            List<MidiEvent> note = new();
            note.Add(midiEvent);
            note.Add(off);

            return note;
        }

        public List<MidiEvent> mapAll(List<NoteButton> noteButtons)
        {
            List<MidiEvent> midiEvents = new();

            foreach (NoteButton noteButton in noteButtons)
            {
                midiEvents.AddRange(map(noteButton));
            }

            return midiEvents;
        }
    }
}
