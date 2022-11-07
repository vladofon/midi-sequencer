using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using NAudio.Midi;
using NAudio.Wave;

namespace midi_sequencer.service
{
    enum PlaybackStates // Состояния проигрывания
    {
        Playing, Stopped, Paused, Closed, Undefined
    }

    internal class PlaybackService
    {
        private PlaybackStates playbackState; // Состояние воспроизведения
        private List<NoteEvent>? pausedNotes; // Список тех нот, которые нужно остановить при приостановке воспроизведения и запустить при продолжении воспроизведения (одни и те же)

        private List<long> currTimes;
        private List<int> currEvents;
        private int playingTrack;
        private long prevTime;

        private byte[]? binaryData; // Массив для хранения MIDI эвентов в бинарном формате
        private long refAT; // Референсный AbsoluteTime. Хз зачем нужен, но при нуле всё работает

        private int tempo; // Количество микросекунд на четвёртую ноту (500000 - стандарт, до того как темп будет задан мета событием, соответствует 120 BPM) (60 / BPM * 1000000)
        private int dtpqn; // Количество тактов на четвёртую ноту
        private double multiplier; // Количество микросекунд на один такт (тик) (tempo / dtpqn)

        private Thread playThread; // Поток воспроизведения

        public PlaybackService() // Объект воспроизведения коллекции. midiOut - устройство воспроизведения, midiEvents - коллекция
        {
            VarsReset(); // Сброс всех переменных до стандартных значений

            playbackState = PlaybackStates.Stopped; // Начальное состояние воспроизведения

            playThread = new Thread(PlaybackThread); // Запуск нового потока с методом PlaybackThread
            playThread.Start();
        }

        private void VarsReset() // Сброс всех переменных до стандартных значений
        {
            pausedNotes = null;

            binaryData = null;
            refAT = 0;

            tempo = 500000;
            dtpqn = MidiService.GetInstance().collection.DeltaTicksPerQuarterNote;
            multiplier = tempo / dtpqn;

            currTimes = new List<long>();
            currEvents = new List<int>();
            playingTrack = 0;
            prevTime = 0;

            for (int i = 0; i < MidiService.GetInstance().collection.Tracks; i++)
            {
                currTimes.Add(0);
                currEvents.Add(0);
            }
            currTimes.Add(-1);
            currEvents.Add(-1);
        }

        //public TimeSpan GetTime() // Это как-нибудь потом
        //{
        //    return new TimeSpan((long)(currAbsoluteTime * multiplier * 10));
        //}

        public void Play() // Начать воспроизведение из точки currEvent (по умолчанию - 0)
        {
            playbackState = PlaybackStates.Playing; // Изменение состояния проигрывания
        }

        public void Pause() // Приостановить воспроизведение
        {
            playbackState = PlaybackStates.Paused;
        }

        public void Stop() // Полностью остановить воспроизведение
        {
            playbackState = PlaybackStates.Stopped;
        }

        public void Close() // Закрыть поток (обязательно нужно выполнить перед закрытием программы)
        {
            playbackState = PlaybackStates.Closed;
        }

        private List<NoteEvent> ReturnAllPlayingNotes() // Метод возвращает все ноты, что не были выключены до currEvent
        {
            List<NoteEvent> playingNotes = new List<NoteEvent>(); // Создаем новый список

            List<long> tempCurrTimes = new List<long>();
            List<int> tempCurrEvents = new List<int>();
            int tempPlayingTrack = 0;

            for (int i = 0; i < MidiService.GetInstance().collection.Tracks; i++)
            {
                tempCurrTimes.Add(0);
                tempCurrEvents.Add(0);
            }
            tempCurrTimes.Add(-1);
            tempCurrEvents.Add(-1);

            while (!tempCurrEvents.SequenceEqual(currEvents))
            {
                tempPlayingTrack = 0;

                for (int track = 0; track < MidiService.GetInstance().collection.Tracks; track++)
                {
                    if (MidiService.GetInstance().collection[track].Count > tempCurrEvents[track] && tempCurrEvents[track] > -1)
                    {
                        tempCurrTimes[track] = MidiService.GetInstance().collection[track][tempCurrEvents[track]].AbsoluteTime;

                        if (tempCurrTimes[track] < tempCurrTimes[tempPlayingTrack])
                        {
                            tempPlayingTrack = track;
                        }
                    }
                    else
                    {
                        if (tempPlayingTrack == track)
                        {
                            tempPlayingTrack++;
                        }
                        tempCurrEvents[track] = -1;
                        continue;
                    }
                }

                if (tempCurrEvents[tempPlayingTrack] != -1)
                {
                    if (MidiService.GetInstance().collection[tempPlayingTrack][tempCurrEvents[tempPlayingTrack]].CommandCode == MidiCommandCode.NoteOn) // Если эвент типа NoteOn -
                    {
                        playingNotes.Add((NoteEvent)MidiService.GetInstance().collection[tempPlayingTrack][tempCurrEvents[tempPlayingTrack]]); // - записываем в список playingNotes
                    }
                    else if (MidiService.GetInstance().collection[tempPlayingTrack][tempCurrEvents[tempPlayingTrack]].CommandCode == MidiCommandCode.NoteOff) // Если эвент типа NoteOff -
                    {
                        NoteEvent noteOff = (NoteEvent)MidiService.GetInstance().collection[tempPlayingTrack][tempCurrEvents[tempPlayingTrack]]; // - преобразуем его в NoteEvent

                        playingNotes.Remove(playingNotes.Where(noteOn => noteOn.Channel == noteOff.Channel && noteOn.NoteNumber == noteOff.NoteNumber).Last()); // и удаляем соответствующий ему NoteOn из списка playingNotes
                    }

                    tempCurrEvents[tempPlayingTrack]++;
                }
            }

            return playingNotes; // Возвращаем список
        }

        private void PlaybackThread() // Поток воспроизведения
        {
            while (playbackState != PlaybackStates.Closed) // Основной цикл, исполняется до закрытия потока
            {
                if (playbackState == PlaybackStates.Paused) // Если воспроизведение приостановлено
                {
                    //MessageBox.Show("Paused");

                    pausedNotes = ReturnAllPlayingNotes(); // Берём список играющих нот
                    TurnOffPlayingNotes(); // Заглушаем все играющие ноты, не изменяя их список

                    playbackState = PlaybackStates.Undefined; // Устанавливаем состояние воспроизведения Undefined, для того чтобы не завершался основной цикл, но при этом ничего не происходило
                }

                else if (playbackState == PlaybackStates.Stopped) // Если воспроизведение полностью остановлено
                {
                    //MessageBox.Show("Stopped");

                    pausedNotes = ReturnAllPlayingNotes(); // Делаем то же самое, что и при паузе, но - 
                    TurnOffPlayingNotes();
                    pausedNotes = null; // - удаляем список pausedNotes - 

                    VarsReset(); // - и сбрасываем все переменные

                    playbackState = PlaybackStates.Undefined; // Устанавливаем состояние воспроизведения Undefined
                }

                else if (playbackState == PlaybackStates.Playing) // Если воспроизводится
                {
                    //MessageBox.Show("Playing");

                    if (pausedNotes != null) // Если список pausedNotes существует (то есть если трек был приостановлен)
                    {
                        TurnOnPlayingNotes(); // Воспроизводим все ноты из списка pausedNotes
                    }

                    while (!currEvents.All(num => num == -1) && playbackState == PlaybackStates.Playing)
                    {
                        playingTrack = 0;

                        for (int track = 0; track < MidiService.GetInstance().collection.Tracks; track++)
                        {
                            if (MidiService.GetInstance().collection[track].Count > currEvents[track] && currEvents[track] > -1)
                            {
                                currTimes[track] = MidiService.GetInstance().collection[track][currEvents[track]].AbsoluteTime;

                                if (currTimes[track] < currTimes[playingTrack])
                                {
                                    playingTrack = track;
                                }
                            }
                            else
                            {
                                if (playingTrack == track)
                                {
                                    playingTrack++;
                                }
                                currEvents[track] = -1;
                                continue;
                            }
                        }

                        if (currEvents[playingTrack] != -1)
                        {
                            MicrosecondDelay((long)((MidiService.GetInstance().collection[playingTrack][currEvents[playingTrack]].AbsoluteTime - prevTime) * multiplier));

                            if (MidiService.GetInstance().collection[playingTrack][currEvents[playingTrack]].CommandCode == MidiCommandCode.MetaEvent) // Обработка мета эвентов
                            {
                                MemoryStream ms = new MemoryStream(); // Объект для записи в память двоичных значений
                                BinaryWriter binaryWriter = new BinaryWriter(ms); // Объект, что записывает в поток бинарные данные
                                MidiService.GetInstance().collection[playingTrack][currEvents[playingTrack]].Export(ref refAT, binaryWriter); // Экспорт бинарных данных в память
                                binaryData = ms.ToArray(); // Перевод бинарных данных в массив binaryData
                                if (BitConverter.IsLittleEndian) Array.Reverse(binaryData); // Реверс массива
                                if (binaryData.Length >= 5)
                                {
                                    if (binaryData[3] == 0x03 && binaryData[4] == 0x51 && binaryData[5] == 0xFF) // Мета эвент FF 51 (темп)
                                    {
                                        binaryData[3] = 0;
                                        tempo = BitConverter.ToInt32(binaryData, 0);
                                        multiplier = tempo / dtpqn;
                                    }
                                }
                                ms.Dispose(); // Очистка и закрытие лишних объектов
                                ms.Close();
                                binaryWriter.Close();
                            }

                            else // Обработка остальных эвентов
                            {
                                MidiService.GetInstance().midiOut.Send(MidiService.GetInstance().collection[playingTrack][currEvents[playingTrack]].GetAsShortMessage()); // Отправка МИДИ комманды на воспроизводящее Midi устройство
                            }

                            prevTime = currTimes[playingTrack];
                            currEvents[playingTrack]++;
                        }
                    }

                    if (currEvents.All(num => num == -1)) // После завершения цикла воспроизведения по причине полного проигрывания трека, присваиваем воспроизведению состояние Stopped
                    {
                        playbackState = PlaybackStates.Stopped;
                    }
                }
            }

            pausedNotes = ReturnAllPlayingNotes(); // После закрытия основного цикла, в конце останавливаем все воспроизводящиеся ноты (нужно если поток будет закрыт до зарершения проигрывания трека)
            TurnOffPlayingNotes();
        }

        private void TurnOffPlayingNotes()
        {
            if (pausedNotes == null) return; // Выходим из метода если pausedNotes == null

            for (int i = 0; i < pausedNotes.Count(); i++) // Заглушаем в цикле все играющие ноты, не изменяя их список
            {
                MidiService.GetInstance().midiOut.Send(new NoteOnEvent(0, pausedNotes[i].Channel, pausedNotes[i].NoteNumber, 0, 0).GetAsShortMessage());
            }
        }

        private void TurnOnPlayingNotes()
        {
            if (pausedNotes == null) return; // Выходим из метода если pausedNotes == null

            for (int i = 0; i < pausedNotes.Count(); i++) // В цикле воспроизводим все ноты из списка pausedNotes
            {
                MidiService.GetInstance().midiOut.Send(pausedNotes[i].GetAsShortMessage());
            }
        }

        static public MidiEventCollection OpenFile(string filePath) // Вернуть коллекцию из файла
        {
            MidiFile midiFile = new MidiFile(filePath); // Открытие файла
            return midiFile.Events; // Возвращение коллекции файла
        }

        static private void MicrosecondDelay(long durationMicroseconds) // Задержка в микросекундах
        {
            long durationTicks = durationMicroseconds * Stopwatch.Frequency / 1000000;
            Stopwatch stopwatch = Stopwatch.StartNew();

            while (stopwatch.ElapsedTicks < durationTicks)
            {

            }
        }
    }
}
