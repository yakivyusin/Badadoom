using CannedBytes.Midi.IO;
using System.Collections.Generic;

namespace Badadoom_to_BF
{
    class MidiFileData
    {
        public MThdChunk Header;
        public IEnumerable<MTrkChunk> Tracks;
    }
}
