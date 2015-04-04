using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Badadoom
{
    class Program
    {
        static void Main(string[] args)
        {
            string midiFileName = null;

            if (args.Length > 0)
            {
                midiFileName = args[0];
            }
            if (args.Length <= 0)
            {
                Console.WriteLine("Usage: Badadoom 'MidiFile.mid'");
                return;
            }

            var fileData = ReadMidiFile(midiFileName);

            var notes = GetNotes(fileData);

            Interpreter(notes.ToList());
        }

        internal static IEnumerable<MidiFileEvent> GetNotes(MidiFileData fileData)
        {
            IEnumerable<MidiFileEvent> notes = null;

            // merge all track notes and filter out sysex and meta events
            foreach (var track in fileData.Tracks)
            {
                if (notes == null)
                {
                    notes = from note in track.Events
                            where !(note.Message is MidiLongMessage)
                            select note;
                }
                else
                {
                    notes = (from note in track.Events
                             where !(note.Message is MidiLongMessage)
                             select note).Union(notes);
                }
            }

            // order track notes by absolute-time.
            notes = from note in notes
                    where (note.Message as MidiChannelMessage).MidiChannel == 9
                    orderby note.AbsoluteTime
                    select note;

            return notes;
        }

        internal static int GetParameter(MidiFileEvent note)
        {
            var channelMessage = (MidiChannelMessage)note.Message;

            return channelMessage.Parameter1;
        }

        internal static void Interpreter (List<MidiFileEvent> notes)
        {
            byte[] cpu = new byte[30000];
            UInt32 j = 0;
            int brc = 0;

            for (int i = 0; i < notes.Count; i++)
            {
                int param = GetParameter(notes[i]);

                if (param == 40) j++; //analog '>' Snare Drum 2
                if (param == 43) j--; //analog '<' Low Tom 1
                if (param == 35) cpu[j]++; //analog '+' Bass Drum 2
                if (param == 51) cpu[j]--; //analog '-' Ride Cymbal 1
                if (param == 44) Console.Write((char)cpu[j]); //analog '.' Pedal Hi-hat
                if (param == 55) cpu[j] = (byte)Console.Read(); //analog ',' Splash Cymbal
                if (param == 49) // analog '[' Crash Cymbal
                {
                    if (cpu[j] == 0)
                    {
                        ++brc;
                        while (brc != 0)
                        {
                            ++i;
                            if (GetParameter(notes[i]) == 49) ++brc;
                            if (GetParameter(notes[i]) == 52) --brc;
                        }
                    }
                    else continue;
                }
                else if (param == 52) // analog ']' Chinese Cymbal
                {
                    if (cpu[j] == 0)
                    {
                        continue;
                    }
                    else
                    {
                        if (GetParameter(notes[i]) == 52) brc++;
                        while (brc != 0)
                        {
                            --i;
                            if (GetParameter(notes[i]) == 49) brc--;
                            if (GetParameter(notes[i]) == 52) brc++;
                        }
                        --i;
                    }
                }
            }
        }

        internal static MidiFileData ReadMidiFile(string filePath)
        {
            var data = new MidiFileData();
            var reader = FileReaderFactory.CreateReader(filePath);

            data.Header = reader.ReadNextChunk() as MThdChunk;

            var tracks = new List<MTrkChunk>();

            for (int i = 0; i < data.Header.NumberOfTracks; i++)
            {
                try
                {
                    var track = reader.ReadNextChunk() as MTrkChunk;

                    if (track != null)
                    {
                        tracks.Add(track);
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Track '{0}' was not read successfully.", i + 1));
                    }
                }
                catch (Exception e)
                {
                    reader.SkipCurrentChunk();

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Failed to read track: " + (i + 1));
                    Console.WriteLine(e);
                    Console.ForegroundColor = ConsoleColor.Gray;
                }
            }

            data.Tracks = tracks;
            return data;
        }
    }
}
