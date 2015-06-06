using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Badadoom.MidiLibrary;
using Badadoom.MidiLibrary.IO;

namespace Badadoom_to_BF
{
    class Program
    {
        private static readonly int PercussionChannel = 9;

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

            var fileData = new MidiFileReader(midiFileName);

            var notes = fileData.Notes
                            .Where(x => x.Channel == PercussionChannel)
                            .OrderBy(x => x.AbsoluteTime)
                            .ToList();

            Translate(notes, bfFileName);
        }

        internal static void Translate(List<MidiNote> notes, string path)
        {
            using (StreamWriter sw = new StreamWriter(path))
            {
                foreach (var note in notes)
                {
                    int param = note.InstrumentCode;

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
    }
}
