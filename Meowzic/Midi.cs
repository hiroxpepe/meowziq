
using Sanford.Multimedia.Midi;

namespace Meowzic {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した MIDI クラス
    /// </summary>
    public class Midi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice _outDevice;

        int outDeviceID = 0;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Midi() {
            _outDevice = new OutputDevice(outDeviceID);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public OutputDevice outDevice {
            get {
                return _outDevice;
            }
        }
    }
}
