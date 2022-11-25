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

        private const int amountOfTracks = 17;

        public PlaybackService playbackService;

        private MidiService()
        {
            collection = CreateNewCollection();
            midiOut = new(0);

            playbackService = new PlaybackService();
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

            for (int i = 0; i < amountOfTracks; i++) // Заполнение новой коллекции 17 треками (0 - для настроек, 1-16 - для инструментов)
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

            for (int i = 0; i < collection.Tracks; i++)
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

        public void ChangeInstrumentOnTrack(int patchNumber, int trackNumber)
        {
            bool found = false;

            for (int i = 0; i < collection[0].Count; i++)
            {
                if (collection[0][i].CommandCode == MidiCommandCode.PatchChange && ((PatchChangeEvent)collection[0][i]).Channel == trackNumber)
                {
                    collection[0][i] = new PatchChangeEvent(0, trackNumber, patchNumber);
                    found = true;
                }
            }

            if (found == false)
            {
                collection[0].Add(new PatchChangeEvent(0, trackNumber, patchNumber));
            }
            
            //if (collection[0].FirstOrDefault(midiEvent => midiEvent.CommandCode == MidiCommandCode.PatchChange && ((PatchChangeEvent)midiEvent).Channel == trackNumber, null) != null)
            //{
            //}
        }

        public void ImportFileToCollection(string path)
        {
            collection = PlaybackService.OpenFile(path);
        }

        public void ExportCollection()
        {

        }
    }
}
