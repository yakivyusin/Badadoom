using CannedBytes.Media.IO;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Badadoom.MidiLibrary.IO
{
    public class MidiFileReader
    {
        private FileChunkReader reader;
        private string file;
        private MidiFileData data;

        public MidiFileReader(string filePath)
        {
            file = filePath;
            reader = FileReaderFactory.CreateReader(file);
            data = new MidiFileData();

            ReadMidiFile();
        }

        public List<MidiNote> Notes
        {
            get
            {
                return data.Tracks
                    .SelectMany(track => track.Events.Where(note => !(note.Message is MidiLongMessage)))
                    .Select(note => new MidiNote(note))
                    .ToList();
            }
        }

        private void ReadMidiFile()
        {
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
                }
                catch (Exception e)
                {
                    reader.SkipCurrentChunk();
                }
            }

            data.Tracks = tracks;
        }
    }
}
