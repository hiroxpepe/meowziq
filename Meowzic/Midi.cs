
using Sanford.Multimedia.Midi;

namespace Meowzic {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した MIDI クラス
    /// </summary>
    public class Midi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice outDevice;

        int outDeviceID = 0;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Midi() {
            this.outDevice = new OutputDevice(outDeviceID);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public OutputDevice OutDevice {
            get {
                return outDevice;
            }
        }
    }
}
