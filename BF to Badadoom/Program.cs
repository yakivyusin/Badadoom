using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace BF_to_Badadoom
{
    class Program
    {
        static void Main(string[] args)
        {
            string bfFileName = null;
            string midiFileName = null;
            List<string> sourceList = new List<string>();
            List<MidiFileEvent> notes = new List<MidiFileEvent>();
            Random rnd = new Random();

            if (args.Length > 1)
            {
                bfFileName = args[0];
                midiFileName = args[1];
            }
            if (args.Length <= 1)
            {
                Console.WriteLine("Usage: BFToBadadoom 'BFFile.bf' 'MidiFile.mid'");
                return;
            }

            using (StreamReader sr = new StreamReader(bfFileName))
            {
                while (!sr.EndOfStream)
                {
                    sourceList.Add(sr.ReadLine());
                }
            }

            foreach (string line in sourceList)
            {
                string rafined = deleteRandomChars(line);
                foreach (char i in rafined)
                {
                    MidiFileEvent note = new MidiFileEvent();

                    if (notes.Count > 0)
                    {
                        note.DeltaTime = rnd.Next(10, 30);
                        note.AbsoluteTime = notes.Last().AbsoluteTime + note.DeltaTime;
                    }
                    else
                    {
                        note.AbsoluteTime = 0;
                        note.DeltaTime = 0;
                    }

                    switch (i)
                    {
                        case ']':
                            {
                                note.Message = new MidiChannelMessage(5584025);
                                break;
                            }
                        case '[':
                            {
                                note.Message = new MidiChannelMessage(5714329);
                                break;
                            }
                        case ',':
                            {
                                note.Message = new MidiChannelMessage(4143001);
                                break;
                            }
                        case '>':
                            {
                                note.Message = new MidiChannelMessage(7284889);
                                break;
                            }
                        case '<':
                            {
                                note.Message = new MidiChannelMessage(3615641);
                                break;
                            }
                        case '-':
                            {
                                note.Message = new MidiChannelMessage(2569113);
                                break;
                            }
                        case '+':
                            {
                                note.Message = new MidiChannelMessage(8332185);
                                break;
                            }
                        case '.':
                            {
                                note.Message = new MidiChannelMessage(6565017);
                                break;
                            }
                    }

                    notes.Add(note);
                }
            }

            SaveEventsToFile(notes, midiFileName);
        }

        private static void SaveEventsToFile(IEnumerable<MidiFileEvent> notes, string filePath)
        {
            if (notes != null &&
                notes.Count() > 0)
            {
                if (filePath != null)
                {
                    using (var serializer = new MidiFileSerializer(filePath))
                    {
                        serializer.Serialize(notes);
                    }
                }
            }
        }

        private static string deleteRandomChars(string str)
        {
            var allowedChars = "][.,><+-";
            return new string(str.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
