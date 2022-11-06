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

        private MidiService()
        {
            collection = CreateNewCollection();
        }

        public static MidiService GetInstance()
        {
            if (instance == null)
            {
                instance = new MidiService();
            }
            return instance;
        }

        public MidiEventCollection collection { get; set; }

        public void AppendEndMarker(IList<MidiEvent> eventList)
        {
            long absoluteTime = 0;
            if (eventList.Count > 0)
                absoluteTime = eventList[eventList.Count - 1].AbsoluteTime;
            eventList.Add(new MetaEvent(MetaEventType.EndTrack, 0, absoluteTime));
        }

        public MidiEventCollection CreateNewCollection()
        {
            MidiEventCollection collection = new MidiEventCollection(1, 128); // Создание новой коллекции

            for (int i = 0; i < 16; i++) // Заполнение новой коллекции 17 треками (0 - для настроек, 1-16 - для инструментов)
            {
                collection.AddTrack();
                collection[i].Add(new MetaEvent(MetaEventType.EndTrack, 0, 0));
            }

            return collection;
        }

        public void ExportCollection()
        {

        }
    }
}
