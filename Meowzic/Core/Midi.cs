
using Sanford.Multimedia.Midi;

namespace Meowzic.Core {
    public class Midi {

        private OutputDevice _outDevice;

        private int outDeviceID = 0;

        public OutputDevice outDevice {
            get {
                return _outDevice;
            }
        }

        public Midi() {
            _outDevice = new OutputDevice(outDeviceID);
        }
    }
}
