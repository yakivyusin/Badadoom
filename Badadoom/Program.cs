using Badadoom.MidiLibrary;
using Badadoom.MidiLibrary.IO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Badadoom
{
    class Program
    {
        private static readonly int PercussionChannel = 9;

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

            var reader = new MidiFileReader(midiFileName);

            Interpreter(reader
                .Notes
                .Where(x => x.Channel == PercussionChannel)
                .OrderBy(x => x.AbsoluteTime)
                .ToList());
        }

        internal static void Interpreter (List<MidiNote> notes)
        {
            byte[] cpu = new byte[30000];
            UInt32 j = 0;
            int brc = 0;

            for (int i = 0; i < notes.Count; i++)
            {
                int param = notes[i].InstrumentCode;

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
                            if (notes[i].InstrumentCode == 49) ++brc;
                            if (notes[i].InstrumentCode == 52) --brc;
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
                        if (notes[i].InstrumentCode == 52) brc++;
                        while (brc != 0)
                        {
                            --i;
                            if (notes[i].InstrumentCode == 49) brc--;
                            if (notes[i].InstrumentCode == 52) brc++;
                        }
                        --i;
                    }
                }
            }
        }
    }
}
