﻿/*
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
    /// Represents a note with tick, pitch, gate, velocity, and syncopation information.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Converts to ChannelMessage.</item>
    /// <item>Does not provide modify operations.</item>
    /// </list>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// The tick value of the note (absolute position in sequencer).
        /// </summary>
        int _tick;

        /// <summary>
        /// The MIDI note number.
        /// </summary>
        int _num;

        /// <summary>
        /// The length from note on to note off in MIDI.
        /// </summary>
        int _gate;

        /// <summary>
        /// The MIDI note velocity.
        /// </summary>
        int _velo;

        /// <summary>
        /// The syncopation parameter (pre-count).
        /// </summary>
        int _pre_count;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <note>
        /// States cannot be changed once created.<br/>
        /// </note>
        /// <summary>
        /// Initializes a new instance of the <see cref="Note"/> class.
        /// </summary>
        /// <param name="tick">Tick value of the note.</param>
        /// <param name="num">MIDI note number.</param>
        /// <param name="gate">Length from note on to note off in MIDI.</param>
        /// <param name="velo">MIDI note velocity.</param>
        /// <param name="pre_count">Syncopation parameter (optional).</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>States cannot be changed once created.</item>
        /// </list>
        /// </remarks>
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
        /// Gets the tick value of the quarter note (480 resolution sequencer).
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Always an absolute value.</item>
        /// </list>
        /// </remarks>
        public int Tick { get => _tick; }

        /// <summary>
        /// Gets the MIDI note number.
        /// </summary>
        public int Num { get => _num; }

        /// <summary>
        /// Gets the length from note on to note off in MIDI.
        /// </summary>
        public int Gate { get => _gate; }

        /// <summary>
        /// Gets the MIDI note velocity.
        /// </summary>
        public int Velo { get => _velo; }

        /// <summary>
        /// Gets a value indicating whether there is a syncopation parameter.
        /// </summary>
        /// <returns>True if syncopation parameter is present; otherwise, false.</returns>
        public bool HasPre { get => _pre_count > 0; }

        /// <summary>
        /// Gets the syncopation parameter.
        /// </summary>
        public int PreCount { get => _pre_count; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// Checks the length of the gate.
        /// </summary>
        /// <param name="target">Gate length to check.</param>
        /// <returns>Validated gate length.</returns>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Throws <see cref="FormatException"/> if not divisible by TICK_INTERVAL.</item>
        /// </list>
        /// </remarks>
        static int gateValue(int target) {
            if (target is 0) { return target; } // No value, return as is.
            // Check if the gate length is divisible by the tick interval.
            if (target % TICK_INTERVAL is not 0) {
                throw new FormatException($"A gate length must be divisible by the tick interval: {TICK_INTERVAL}.");
            }
            return target; // Returns the original value if validation is ok.
        }
    }
}
