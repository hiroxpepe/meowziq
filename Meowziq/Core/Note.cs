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

using System;

using static Meowziq.Env;

namespace Meowziq.Core {
    /// <summary>
    /// note class
    /// </summary>
    /// <note>
    /// + converts to ChannelMessage. <br/>
    /// + not provide modify operations. <br/>
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _tick, _num, _gate, _velo, _pre_count;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <note>
        /// + states cannot be changed once created. <br/>
        /// </note>
        public Note(int tick, int num, int gate, int velo, int pre_count = 0) {
            _tick = tick;
            _num = num;
            _gate = gateValue(target: gate);
            _velo = velo;
            _pre_count = pre_count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// gets the tick value of the quarter note 480 resolution sequencer.
        /// </summary>
        /// <note>
        /// + always an absolute value. <br/>
        /// </note>
        public int Tick {
            get => _tick;
        }

        /// <summary>
        /// gets the MIDI note number.
        /// </summary>
        public int Num {
            get => _num;
        }

        /// <summary>
        /// gets the length from a note on to a note off of MIDI.
        /// </summary>
        public int Gate {
            get => _gate;
        }

        /// <summary>
        /// gets the MIDI note strength.
        /// </summary>
        public int Velo {
            get => _velo;
        }

        /// <summary>
        /// gets whether has a syncopation parameter.
        /// </summary>
        public bool HasPre {
            get => _pre_count > 0;
        }

        /// <summary>
        /// gets the syncopation parameter.
        /// </summary>
        public int PreCount {
            get => _pre_count;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// checks the length of the gate.
        /// </summary>
        static int gateValue(int target) {
            if (target is 0) { return target; } // no value, return as is.
            // whether the length of the gate is divisible by the tick interval.
            if (target % TICK_INTERVAL is not 0) {
                throw new FormatException($"a gate length must be divisible by the tick interval: {TICK_INTERVAL}.");
            }
            return target; // returns the original value if validation is ok.
        }
    }
}
