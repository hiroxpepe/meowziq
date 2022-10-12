
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// midi class using Sanford.Multimedia.Midi
    /// </summary>
    public class Manager {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        OutputDevice _out_device;

        int _out_device_id = 0; // TODO: to be able to choose.

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Manager() {
            _out_device = new OutputDevice(_out_device_id);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        public OutputDevice OutDevice {
            get {
                return _out_device;
            }
        }
    }
}
