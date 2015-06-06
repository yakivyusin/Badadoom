using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Badadoom.MidiLibrary;
using Badaddom.MidiLibrary.IO;

namespace BF_to_Badadoom
{
    class Program
    {
        static void Main(string[] args)
        {
            string bfFileName = null;
            string midiFileName = null;

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

            var notes = Translate(File.ReadAllLines(bfFileName).ToList());
            SaveEventsToFile(notes, midiFileName);
        }

        private static List<MidiNote> Translate(List<string> fileContent)
        {
            List<MidiNote> notes = new List<MidiNote>();
            foreach (string line in fileContent)
            {
                foreach (char i in DeleteRandomChars(line))
                {
                    notes.Add(Translate(i, notes.LastOrDefault()?.AbsoluteTime ?? 0));
                }
            }
            return notes;
        }

        private static MidiNote Translate(char symbol, long shift)
        {
            Random rnd = new Random();
            int deltaTime = rnd.Next(10, 30);
            long absoluteTime = shift + deltaTime;

            switch (symbol)
            {
                case ']':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 5584025);
                    }
                case '[':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 5714329);
                    }
                case ',':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 4143001);
                    }
                case '>':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 7284889);
                    }
                case '<':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 3615641);
                    }
                case '-':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 2569113);
                    }
                case '+':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 8332185);
                    }
                case '.':
                    {
                        return new MidiNote(deltaTime, absoluteTime, 6565017);
                    }
            }
            return null;
        }

        private static void SaveEventsToFile(IEnumerable<MidiNote> notes, string filePath)
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

        private static string DeleteRandomChars(string str)
        {
            var allowedChars = "][.,><+-";
            return new string(str.Where(c => allowedChars.Contains(c)).ToArray());
        }
    }
}
