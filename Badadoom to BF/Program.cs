using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Badadoom_to_BF
{
    class Program
    {
        static void Main(string[] args)
        {
            string midiFileName = null;
            string bfFileName = null;

            if (args.Length > 1)
            {
                midiFileName = args[0];
                bfFileName = args[1];
            }
            if (args.Length <= 1)
            {
                Console.WriteLine("Usage: BadadoomToBF 'MidiFile.mid' 'BFFile.bf'");
                return;
            }

            var fileData = ReadMidiFile(midiFileName);

            var notes = GetNotes(fileData);

            Translate(notes.ToList(), bfFileName);
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

        internal static void Translate(List<MidiFileEvent> notes, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var note in notes)
                {
                    int param = GetParameter(note);

                    if (param == 40) sw.Write('>');
                    if (param == 43) sw.Write('<');
                    if (param == 35) sw.Write('+');
                    if (param == 51) sw.Write('-');
                    if (param == 44) sw.Write('.');
                    if (param == 55) sw.Write(',');
                    if (param == 49) sw.Write('[');
                    else if (param == 52) sw.Write(']');
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
