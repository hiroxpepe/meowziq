
using Sanford.Multimedia.Midi;

namespace Meowziq {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した MIDI クラス
    /// </summary>
    public class Midi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice outDevice;

        int outDeviceID = 0; // TODO: 選択出来るように

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Midi() {
            this.outDevice = new OutputDevice(outDeviceID);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public OutputDevice OutDevice {
            get {
                return outDevice;
            }
        }
    }
}
