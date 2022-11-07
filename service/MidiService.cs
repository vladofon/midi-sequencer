using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace midi_sequencer.service
{
    internal class MidiService
    {
        private static MidiService? instance;

        public MidiEventCollection collection { get; set; }
        public MidiOut midiOut { get; set; }

        private const int amountOfTracks = 16;

        private MidiService()
        {
            collection = CreateNewCollection();
            //collection = PlaybackService.OpenFile("C:\\Users\\kosty\\source\\repos\\midi-sequencer\\Test MIDI files\\d_dead\\d_dead.mid");
            midiOut = new(0);
        }

        public static MidiService GetInstance()
        {
            if (instance == null)
            {
                instance = new MidiService();
            }
            return instance;
        }

        public MidiEventCollection CreateNewCollection()
        {
            MidiEventCollection collection = new MidiEventCollection(1, 128); // Создание новой коллекции

            for (int i = 0; i <= amountOfTracks; i++) // Заполнение новой коллекции 17 треками (0 - для настроек, 1-16 - для инструментов)
            {
                collection.AddTrack();
                AppendEndMarker(collection[i]);
            }

            return collection;
        }

        public void AppendEndMarker(IList<MidiEvent> eventList)
        {
            long absoluteTime = 0;
            if (eventList.Count > 0)
                absoluteTime = eventList[eventList.Count - 1].AbsoluteTime;
            eventList.Add(new MetaEvent(MetaEventType.EndTrack, 0, absoluteTime));
        }

        public void WriteInTrack(List<MidiEvent> eventList, int trackNumber)
        {
            MidiEventCollection tempCollection = new MidiEventCollection(1, 128);

            for (int i = 0; i <= amountOfTracks; i++)
            {
                if (i == trackNumber)
                {
                    tempCollection.AddTrack(eventList);
                    AppendEndMarker(tempCollection[i]);
                }
                else
                {
                    tempCollection.AddTrack(this.collection[i]);
                }
            }

            collection = tempCollection;
        }

        public void ExportCollection()
        {

        }
    }
}
