using CannedBytes.Media.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel.Composition.Hosting;
using CannedBytes.ComponentModel.Composition;
using CannedBytes.Midi.IO;
using CannedBytes.Media.IO.Services;
using Badadoom.MidiLibrary;
using System;

namespace Badaddom.MidiLibrary.IO
{
    public class MidiFileSerializer : IDisposable
    {
        private ChunkFileContext context = new ChunkFileContext();

        public MidiFileSerializer(string filePath)
        {
            this.context.ChunkFile = ChunkFileInfo.OpenWrite(filePath);
            this.context.CompositionContainer = CreateCompositionContainer();
        }

        public void Serialize(IEnumerable<MidiNote> events)
        {
            Serialize(events.Select(x => x.InnerEvent));
        }

        private CompositionContainer CreateCompositionContainer()
        {
            var factory = new CompositionContainerFactory();
            var midiIOAssembly = typeof(MTrkChunkHandler).Assembly;

            // add basic file handlers
            factory.AddMarkedTypesInAssembly(null, typeof(IFileChunkHandler));

            // add midi file handlers
            factory.AddMarkedTypesInAssembly(midiIOAssembly, typeof(IFileChunkHandler));

            // note that Midi files use big endian.
            // and the chunks are not aligned.
            factory.AddTypes(
                typeof(BigEndianNumberWriter),
                typeof(SizePrefixedStringWriter),
                typeof(ChunkTypeFactory),
                typeof(FileChunkHandlerManager));

            var container = factory.CreateNew();

            var chunkFactory = container.GetService<ChunkTypeFactory>();

            // add midi chunks
            chunkFactory.AddChunksFrom(midiIOAssembly, true);

            return container;
        }

        private void Serialize(IEnumerable<MidiFileEvent> events)
        {
            var builder = new MidiTrackBuilder(events);
            builder.BuildTracks();
            builder.AddEndOfTrackMarkers();

            var header = new MThdChunk();
            header.Format = (ushort)MidiFileFormat.MultipleTracks;
            header.NumberOfTracks = (ushort)builder.Tracks.Count();
            header.TimeDivision = 120;

            var writer = new FileChunkWriter(this.context);

            writer.WriteNextChunk(header);

            foreach (var trackChunk in builder.Tracks)
            {
                writer.WriteNextChunk(trackChunk);
            }
        }

        public void Dispose()
        {
            context.Dispose();
        }
    }
}
