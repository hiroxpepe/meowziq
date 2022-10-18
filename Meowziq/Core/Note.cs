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

namespace Meowziq.Core {
    /// <summary>
    /// note class.
    /// </summary>
    /// <note>
    /// + converts to ChannelMessage
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _tick, _num, _gate, _velo, _pre_count;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <note>
        /// + states cannot be changed once created.
        /// </note>
        public Note(int tick, int num, int gate, int velo, int pre_count = 0) {
            _tick = tick;
            _num = num;
            _gate = gate;
            _velo = velo;
            _pre_count = pre_count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// tick value of the quarter note 480 resolution sequencer.
        /// </summary>
        /// <note>
        /// + always an absolute value. <br/>
        /// + not provide modify operations. <br/>
        /// </note>
        public int Tick {
            get => _tick;
        }

        /// <summary>
        /// MIDI note number.
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Num {
            get => _num;
        }

        /// <summary>
        /// length from a note on to a note off of MIDI.
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Gate {
            get => _gate;
        }

        /// <summary>
        /// MIDI note strength.
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Velo {
            get => _velo;
        }

        /// <summary>
        /// whether has a syncopation parameter.
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public bool HasPre {
            get => _pre_count > 0;
        }

        /// <summary>
        /// syncopation parameter.
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int PreCount {
            get => _pre_count;
        }
    }
}
