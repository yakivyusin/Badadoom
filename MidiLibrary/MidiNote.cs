using CannedBytes.Midi.IO;
using CannedBytes.Midi.Message;

namespace Badadoom.MidiLibrary
{
    public class MidiNote
    {
        private MidiFileEvent midiEvent;

        public MidiNote(long deltaTime, long absoluteTime, int data)
        {
            midiEvent = new MidiFileEvent();
            midiEvent.DeltaTime = deltaTime;
            midiEvent.AbsoluteTime = absoluteTime;
            midiEvent.Message = new MidiChannelMessage(data);
        }

        internal MidiNote(MidiFileEvent midiEvent)
        {
            this.midiEvent = midiEvent;
        }

        public int InstrumentCode
        {
            get
            {
                return ((MidiChannelMessage)midiEvent.Message).Parameter1;
            }
        }

        public int Channel
        {
            get
            {
                return ((MidiChannelMessage)midiEvent.Message).MidiChannel;
            }
        }

        public long AbsoluteTime
        {
            get
            {
                return midiEvent.AbsoluteTime;
            }
        }

        internal MidiFileEvent InnerEvent
        {
            get
            {
                return midiEvent;
            }
        }
    }
}
