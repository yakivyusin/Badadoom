using CannedBytes.Midi.IO;
using System.Collections.Generic;

namespace Badadoom
{
    class MidiFileData
    {
        public MThdChunk Header;
        public IEnumerable<MTrkChunk> Tracks;
    }
}
