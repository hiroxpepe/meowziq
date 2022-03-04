
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// Sanford.Multimedia.Midi を使用した MIDI クラス
    /// </summary>
    public class Manager {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice _outDevice;

        int _outDeviceID = 0; // TODO: 選択出来るように

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Manager() {
            this._outDevice = new OutputDevice(_outDeviceID);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public OutputDevice OutDevice {
            get {
                return _outDevice;
            }
        }
    }
}
