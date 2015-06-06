using CannedBytes.Midi.IO;
using System.Collections.Generic;

namespace Badadoom.MidiLibrary
{
    internal class MidiFileData
    {
        public MThdChunk Header;
        public IEnumerable<MTrkChunk> Tracks;
    }
}
