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

namespace midi_sequencer.playback
{
    enum PlaybackStates // Состояния проигрывания
    {
        Playing, Stopped, Paused, Closed, Undefined
    }

    internal class Playback
    {
        private MidiOut midiOut; // Устройство вывода

        private MidiEventCollection midiEvents; // Коллекция миди эвентов
        public List<MidiEvent> bigEventList; // Один большой список всех эвентов из коллекции

        public PlaybackStates playbackState; // Состояние воспроизведения
        public List<NoteEvent>? pausedNotes; // Список тех нот, которые нужно остановить при приостановке воспроизведения и запустить при продолжении воспроизведения (одни и те же)

        private byte[]? binaryData; // Массив для хранения MIDI эвентов в бинарном формате
        private long refAT; // Референсный AbsoluteTime. Хз зачем нужен, но при нуле всё работает

        private int tempo; // Количество микросекунд на четвёртую ноту (500000 - стандарт, до того как темп будет задан мета событием, соответствует 120 BPM) (60 / BPM * 1000000)
        private int dtpqn; // Количество тактов на четвёртую ноту
        private double multiplier; // Количество микросекунд на один такт (тик) (tempo / dtpqn)

        public long currAbsoluteTime; // AbsoluteTime предыдущего эвента. Измеряется в тактах
        public long maxAbsoluteTime; // Последний AbsoluteTime

        public int currEvent; // Индекс текущего эвента

        private Thread playThread; // Поток воспроизведения

        public Playback(MidiOut midiOut, MidiEventCollection midiEvents) // Объект воспроизведения коллекции. midiOut - устройство воспроизведения, midiEvents - коллекция
        {
            this.midiOut = midiOut; // Запись аргументов в экземпляр класса
            this.midiEvents = midiEvents;

            bigEventList = new List<MidiEvent>(); // Создание списка эвентов bigEventList
            for (int i = 0; i < midiEvents.Tracks; i++) // В цикле объединение всех треков (списков эвентов в MidiEventCollection events) в один список List<MidiEvent> bigEventList
            {
                bigEventList.AddRange(midiEvents[i]);
            }

            bigEventList.Sort(delegate (MidiEvent a, MidiEvent b) // Сортировка списка eventList по полю AbsoluteTime
            {
                return a.AbsoluteTime.CompareTo(b.AbsoluteTime);
            });

            VarsReset(); // Сброс всех переменных до стандартных значений

            playbackState = PlaybackStates.Stopped; // Начальное состояние воспроизведения

            playThread = new Thread(PlaybackThread); // Запуск нового потока с методом PlaybackThread
            playThread.Start();
        }

        public void VarsReset() // Сброс всех переменных до стандартных значений
        {
            pausedNotes = null;

            binaryData = null;
            refAT = 0;

            tempo = 500000;
            dtpqn = midiEvents.DeltaTicksPerQuarterNote;
            multiplier = tempo / dtpqn;

            currAbsoluteTime = 0;
            maxAbsoluteTime = bigEventList.Last().AbsoluteTime;

            currEvent = 0;
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

        public List<NoteEvent> ReturnAllPlayingNotes() // Метод возвращает все ноты, что не были выключены до currEvent
        {
            List<NoteEvent> playingNotes = new List<NoteEvent>(); // Создаем новый список

            for (int i = 0; i < currEvent; i++) // В цикле проходим по эвентам bigEventList до текущего эвента
            {
                if (bigEventList[i].CommandCode == MidiCommandCode.NoteOn) // Если эвент типа NoteOn -
                {
                    playingNotes.Add((NoteEvent)bigEventList[i]); // - записываем в список playingNotes
                }
                else if (bigEventList[i].CommandCode == MidiCommandCode.NoteOff) // Если эвент типа NoteOff -
                {
                    NoteEvent noteOff = (NoteEvent)bigEventList[i]; // - преобразуем его в NoteEvent

                    playingNotes.Remove(playingNotes.Where(noteOn => noteOn.Channel == noteOff.Channel && noteOn.NoteNumber == noteOff.NoteNumber).Last()); // и удаляем соответствующий ему NoteOn из списка playingNotes
                }
            }

            return playingNotes; // Возвращаем список
        }

        public void PlaybackThread() // Поток воспроизведения
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

                    while (currEvent < bigEventList.Count() && playbackState == PlaybackStates.Playing) // Цикл воспроизведения. Проходит по всем элементым списка bigEventList когда состояние воспроизведения == Playing
                    {
                        MicrosecondDelay((long)((bigEventList[currEvent].AbsoluteTime - currAbsoluteTime) * multiplier)); // Задержка потока в микросекундах. (Абсолютное время текущей комманды - Абсолютное время пред. комманды) * Количество микросекунд на один такт

                        currAbsoluteTime = bigEventList[currEvent].AbsoluteTime; // Запись нового AbsoluteTime из текущей комманды в currAbsoluteTime

                        if (bigEventList[currEvent].CommandCode == MidiCommandCode.MetaEvent) // Обработка мета эвентов
                        {
                            MemoryStream ms = new MemoryStream(); // Объект для записи в память двоичных значений
                            BinaryWriter binaryWriter = new BinaryWriter(ms); // Объект, что записывает в поток бинарные данные
                            bigEventList[currEvent].Export(ref refAT, binaryWriter); // Экспорт бинарных данных в память
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
                            midiOut.Send(bigEventList[currEvent].GetAsShortMessage()); // Отправка МИДИ комманды на воспроизводящее Midi устройство
                        }

                        currEvent++; // Действительно блять, почему нихуя не играло?
                    }

                    if (currEvent == bigEventList.Count()) // После завершения цикла воспроизведения по причине полного проигрывания трека, присваиваем воспроизведению состояние Stopped
                    {
                        playbackState = PlaybackStates.Stopped;
                    }
                }
            }

            pausedNotes = ReturnAllPlayingNotes(); // После закрытия основного цикла, в конце останавливаем все воспроизводящиеся ноты (нужно если поток будет закрыт до зарершения проигрывания трека)
            TurnOffPlayingNotes();
        }

        public void TurnOffPlayingNotes()
        {
            if (pausedNotes == null) return; // Выходим из метода если pausedNotes == null

            for (int i = 0; i < pausedNotes.Count(); i++) // Заглушаем в цикле все играющие ноты, не изменяя их список
            {
                midiOut.Send(new NoteOnEvent(0, pausedNotes[i].Channel, pausedNotes[i].NoteNumber, 0, 0).GetAsShortMessage());
            }
        }

        public void TurnOnPlayingNotes()
        {
            if (pausedNotes == null) return; // Выходим из метода если pausedNotes == null

            for (int i = 0; i < pausedNotes.Count(); i++) // В цикле воспроизводим все ноты из списка pausedNotes
            {
                midiOut.Send(pausedNotes[i].GetAsShortMessage());
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
