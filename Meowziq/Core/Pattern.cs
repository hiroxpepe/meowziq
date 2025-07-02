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
using System.Collections.Generic;
using System.Linq;

using static Meowziq.Env;

namespace Meowziq.Core {
    /// <summary>
    /// Represents a pattern.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _name;

        List<Meas> _meas_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Pattern(string name, List<Meas> meas_list) {
            if (meas_list.Count > Measure.Max.Int32()) {
                throw new ArgumentException($"measure counts are until {Measure.Max.Int32()}.");
            }
            _name = name;
            _meas_list = meas_list;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the pattern name.
        /// </summary>
        public string Name { get => _name; }

        /// <summary>
        /// Gets the number of beats.
        /// </summary>
        public int BeatCount { get => _meas_list.Count * BEAT_COUNT_IN_MEASURE; }

        /// <summary>
        /// Gets all Meas objects.
        /// </summary>
        public List<Meas> AllMeas { get => _meas_list; }

        /// <summary>
        /// Gets all Span objects.
        /// </summary>
        public List<Span> AllSpan { get => _meas_list.SelectMany(selector: x => x.AllSpan).ToList(); }
    }

    /// <summary>
    /// Represents a measure.
    /// </summary>
    public class Meas {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        List<Span> _span_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Meas(List<Span> spanList) {
            int total_beat_count = 0;
            spanList.ForEach(action: x => total_beat_count += x.Beat); // Count beats.
            if (total_beat_count < BEAT_COUNT_IN_MEASURE || total_beat_count > BEAT_COUNT_IN_MEASURE) {
                throw new ArgumentException($"Beat count must be {BEAT_COUNT_IN_MEASURE}."); // Not enough for one measure or more than one measure.
            }
            /// <note>
            /// + Divides a span to every beat. <br/>
            /// + Important! <br/>
            /// </note>
            _span_list = new();
            spanList.ForEach(action: x => {
                Enumerable.Range(start: 0, count: x.Beat).ToList().ForEach(
                    action: _x => _span_list.Add(new Span(beat: 1, degree: x.Degree, span_mode: x.SpanMode))
                );
            });
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets all Span objects.
        /// </summary>
        public List<Span> AllSpan { get => _span_list; }
    }

    /// <summary>
    /// Represents a span.
    /// </summary>
    /// <note>
    /// Represents the duration (beat) of the tonality.<br/>
    /// Converts the numbers 1 to 7 into the scale note of the church mode.<br/>
    /// </note>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _beat;

        Key _key;

        Degree _degree;

        Mode _key_mode, _span_mode;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Span(int beat, Degree degree) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _key_mode = Mode.Undefined;
            _span_mode = Mode.Undefined;
        }

        public Span(int beat, Degree degree, Mode span_mode) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _key_mode = Mode.Undefined;
            _span_mode = span_mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// Gets the number of beats.
        /// </summary>
        public int Beat { get => _beat; }

        /// <summary>
        /// Gets the degree from the key of the song.
        /// </summary>
        public Degree Degree { get => _degree; }

        /// <summary>
        /// Gets or sets the key of the song.
        /// </summary>
        public Key Key { get => _key; set => _key = value; }

        /// <summary>
        /// Gets or sets the church mode of the key of the song.
        /// </summary>
        public Mode KeyMode { get => _key_mode; set => _key_mode = value; }

        /// <summary>
        /// Gets the church mode of this span.
        /// </summary>
        public Mode SpanMode { get => _span_mode; } // Do not change the span mode once set.
    }
}
