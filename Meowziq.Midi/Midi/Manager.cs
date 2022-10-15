/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// MIDI class using Sanford.Multimedia.Midi
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
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
