using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;
using NAudio.Midi;

namespace midi_sequencer.playback
{
    internal class Playback
    {
        public MidiOut midiOut; // Устройство вывода

        public MidiEventCollection midiEvents; // Коллекция миди эвентов
        public List<MidiEvent> bigEventList; // Один большой список всех эвентов из коллекции

        public int tempo; // Количество микросекунд на четвёртую ноту (500000 - стандарт, до того как темп будет задан мета событием, соответствует 120 BPM)
        public int dtpqn; // Количество тактов на четвёртую ноту
        public double multiplier; // Количество микросекунд на один такт (тик) (tempo / dtpqn)

        public long prevAbsoluteTime = 0; // AbsoluteTime предыдущего эвента



        public Playback(MidiOut midiOut, MidiEventCollection midiEvents) // Объект воспроизведения коллекции. midiOut - устройство воспроизведения, midiEvents - коллекция
        {
            this.midiOut = midiOut;
            this.midiEvents = midiEvents;

            bigEventList = new List<MidiEvent>(); // Создание списка эвентов
            for (int i = 0; i < midiEvents.Tracks; i++) // В цикле объединение всех треков (списков эвентов в MidiEventCollection events) в один список List<MidiEvent> eventList
            {
                bigEventList.AddRange(midiEvents[i]);
            }

            bigEventList.Sort(delegate (MidiEvent a, MidiEvent b) // Сортировка списка eventList по полю AbsoluteTime
            {
                return a.AbsoluteTime.CompareTo(b.AbsoluteTime);
            });

            tempo = 500000;
            dtpqn = midiEvents.DeltaTicksPerQuarterNote;
            multiplier = tempo / dtpqn;
        }

        public void Play() // Воспроизвести коллекцию
        {
            //prevAbsoluteTime = 0;

            byte[] binaryData; // Массив для хранения MIDI эвентов в бинарном формате
            long refAT = 0; // Референсный AbsoluteTime. Хз зачем нужен, но при нуле всё работает

            for (int i = 0; i < bigEventList.Count(); i++) // Прохождение по всему списку eventList
            {
                if (bigEventList[i].CommandCode == MidiCommandCode.MetaEvent)
                {
                    MemoryStream ms = new MemoryStream(); // Объект для записи в память двоичных значений
                    BinaryWriter binaryWriter = new BinaryWriter(ms); // Объект, что записывает в поток бинарные данные

                    bigEventList[i].Export(ref refAT, binaryWriter); // Экспорт бинарных данных в память
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
                    ms.Dispose(); // Очистка и закрытие лишних объектов
                    ms.Close();
                    binaryWriter.Close();
                }

                MicrosecondDelay((long)((bigEventList[i].AbsoluteTime - prevAbsoluteTime) * multiplier)); // Задержка потока. (Абсолютное время текущей комманды - Абсолютное время пред. комманды) * Количество микросекунд на один такт

                prevAbsoluteTime = bigEventList[i].AbsoluteTime; // Запись нового AbsoluteTime из текущей комманды в prevAbsoluteTime

                midiOut.Send(bigEventList[i].GetAsShortMessage()); // Отправка МИДИ комманды на воспроизводящее Midi устройство
            }
        }

        static public MidiEventCollection OpenFile(string filePath) // Открыть файл
        {
            MidiFile midiFile = new MidiFile(filePath); // Открытие файла
            return midiFile.Events; // Возвращение коллекции файла
        }

        /*static public void PlayOpenedCollection()
        {
            if (events == null)
            {
                return;
            }

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
        }*/

        static private void MicrosecondDelay(long durationMicroseconds)
        {
            long durationTicks = durationMicroseconds * Stopwatch.Frequency / 1000000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedTicks < durationTicks)
            {

            }
        }
    }
}
