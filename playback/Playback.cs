using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Midi;

namespace midi_sequencer.playback
{
    internal class Playback
    {

        static private MidiOut midiOut = new MidiOut(0); // Выходное устройство

        static private void MicrosecondDelay(long durationMicroseconds)
        {
            long durationTicks = durationMicroseconds * Stopwatch.Frequency / 1000000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedTicks < durationTicks)
            {

            }
        }

        static public void PlayFile(string filename)
        {
            MidiFile midiFile = new MidiFile(filename); // Открытие файла
            MidiEventCollection events = midiFile.Events; // Преобразование файла в коллекцию

            PlayCollection(events);
        }

        static public void PlayCollection(MidiEventCollection events)
        {
            List<MidiEvent> eventList = new List<MidiEvent>(); // Создание списка эвентов
            for (int i = 0; i < events.Tracks; i++) // В цикле объединение всех треков (списков эвентов в MidiEventCollection events) в один список List<MidiEvent> eventList
            {
                eventList.AddRange(events[i]);
            }

            eventList.Sort(delegate (MidiEvent a, MidiEvent b) // Сортировка списка eventList по полю AbsoluteTime
            {
                return a.AbsoluteTime.CompareTo(b.AbsoluteTime);
            });

            Console.WriteLine(eventList.Count());

            //Console.ReadKey(true);

            int dtpqn = events.DeltaTicksPerQuarterNote; // Количество тактов на четвёртую ноту
            int tempo = 500000; // Количество микросекунд на четвёртую ноту
            double multiplier = tempo / dtpqn; // Количество микросекунд на один такт (тик)

            long prevAbsoluteTime = 0; // AbsoluteTime предыдущего эвента

            byte[] binaryData; // Массив для хранения MIDI эвентов в бинарном формате
            long refAT = 0; // Референсный AbsoluteTime. Хз зачем нужен, но при нуле всё работает

            for (int i = 0; i < eventList.Count(); i++) // Прохождение по всему списку eventList
            {
                //Stopwatch timer = new Stopwatch(); // Дебаг: включение таймера в начале обработки эвента
                //timer.Start();

                if (eventList[i].CommandCode == MidiCommandCode.MetaEvent)
                {
                    MemoryStream ms = new MemoryStream(); // Объект для записи в память двоичных значений
                    BinaryWriter binaryWriter = new BinaryWriter(ms); // Объект, что записывает в поток бинарные данные

                    eventList[i].Export(ref refAT, binaryWriter); // Экспорт бинарных данных в память
                    binaryData = ms.ToArray(); // Перевод бинарных данных в массив binaryData
                    if (BitConverter.IsLittleEndian) Array.Reverse(binaryData); // Реверс массива
                    if (binaryData.Length >= 5) // TODO: Сделать возможность легко добавить действия для любой MIDI комманды
                    {
                        if (binaryData[3] == 0x03 && binaryData[4] == 0x51 && binaryData[5] == 0xFF)
                        {
                            binaryData[3] = 0;
                            tempo = BitConverter.ToInt32(binaryData, 0);
                            //Console.WriteLine(tempo);
                            multiplier = tempo / dtpqn;
                        }
                    }
                    ms.Dispose();
                    ms.Close();
                    binaryWriter.Close();
                }
                
                MicrosecondDelay((long)((eventList[i].AbsoluteTime - prevAbsoluteTime) * multiplier));

                prevAbsoluteTime = eventList[i].AbsoluteTime;

                midiOut.Send(eventList[i].GetAsShortMessage());

                //timer.Stop();  // Дебаг: выключение таймера в конце обработки эвента и вывод эго времени на экран
                               //Console.WriteLine(timer.Elapsed + " | " + eventList[i] + " | " + eventList[i].GetAsShortMessage().ToString("X"));

                //Console.Write(eventList[i].GetAsShortMessage().ToString("X") + "\t | ");
                //foreach (var item in binaryData)
                //{
                //    Console.Write(item.ToString("X") + " ");
                //}
                //Console.Write("\t | " + eventList[i]);
                //Console.WriteLine();
            }
        }
    }
}
