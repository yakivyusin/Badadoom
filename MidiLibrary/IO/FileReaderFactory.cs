using CannedBytes.ComponentModel.Composition;
using CannedBytes.Media.IO;
using CannedBytes.Media.IO.Services;
using CannedBytes.Midi.IO;
using System.ComponentModel.Composition.Hosting;

namespace Badadoom.MidiLibrary.IO
{
    internal class FileReaderFactory
    {
        public static FileChunkReader CreateReader(string filePath)
        {
            var context = CreateFileContextForReading(filePath);

            var reader = context.CompositionContainer.CreateFileChunkReader();

            return reader;
        }

        private static ChunkFileContext CreateFileContextForReading(string filePath)
        {
            var context = new ChunkFileContext();
            context.ChunkFile = ChunkFileInfo.OpenRead(filePath);

            context.CompositionContainer = CreateCompositionContextForReading();

            return context;
        }

        private static CompositionContainer CreateCompositionContextForReading()
        {
            var factory = new CompositionContainerFactory();

            factory.AddMarkedTypesInAssembly(null, typeof(IFileChunkHandler));
            
            factory.AddMarkedTypesInAssembly(typeof(MTrkChunkHandler).Assembly, typeof(IFileChunkHandler));
            
            factory.AddTypes(
                typeof(BigEndianNumberReader),
                typeof(SizePrefixedStringReader),
                typeof(ChunkTypeFactory),
                typeof(FileChunkHandlerManager));

            var container = factory.CreateNew();

            var chunkFactory = container.GetService<ChunkTypeFactory>();
            
            chunkFactory.AddChunksFrom(typeof(MTrkChunkHandler).Assembly, true);

            return container;
        }
    }
}
