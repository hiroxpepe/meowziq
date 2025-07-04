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
    /// Represents a musical pattern consisting of measures.
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Pattern {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Pattern name.
        /// </summary>
        string _name;

        /// <summary>
        /// List of measures in this pattern.
        /// </summary>
        List<Meas> _meas_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Pattern"/> class.
        /// </summary>
        /// <param name="name">Pattern name.</param>
        /// <param name="meas_list">List of measures.</param>
        /// <exception cref="ArgumentException">Thrown if the number of measures exceeds the maximum allowed.</exception>
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
        /// Gets the number of beats in this pattern.
        /// </summary>
        public int BeatCount { get => _meas_list.Count * BEAT_COUNT_IN_MEASURE; }

        /// <summary>
        /// Gets all measures in this pattern.
        /// </summary>
        public List<Meas> AllMeas { get => _meas_list; }

        /// <summary>
        /// Gets all spans in this pattern.
        /// </summary>
        public List<Span> AllSpan { get => _meas_list.SelectMany(selector: x => x.AllSpan).ToList(); }
    }

    /// <summary>
    /// Represents a measure containing a list of tonal spans.
    /// </summary>
    public class Meas {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// List of Span objects representing tonal segments within the measure.
        /// </summary>
        List<Span> _span_list;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Meas"/> class with a list of spans.
        /// </summary>
        /// <param name="spanList">List of Span objects to fill the measure.</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Divides a span to every beat.</item>
        /// <item>Important: Each measure must have exactly <c>BEAT_COUNT_IN_MEASURE</c> beats.</item>
        /// </list>
        /// </remarks>
        /// <exception cref="ArgumentException">Thrown if the total beat count is not exactly <c>BEAT_COUNT_IN_MEASURE</c>.</exception>
        public Meas(List<Span> spanList) {
            int total_beat_count = 0;
            spanList.ForEach(action: x => total_beat_count += x.Beat); // Count beats.
            if (total_beat_count < BEAT_COUNT_IN_MEASURE || total_beat_count > BEAT_COUNT_IN_MEASURE) {
                throw new ArgumentException($"beat counts needs {BEAT_COUNT_IN_MEASURE}."); // Not enough for one measure or more than one measure.
            }
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
        /// Gets all Span objects in this measure.
        /// </summary>
        public List<Span> AllSpan { get => _span_list; }
    }

    /// <summary>
    /// Represents a span of tonality, including beat, degree, key, and mode information.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>Represents the duration (beat) of the tonality.</item>
    /// <item>Converts the numbers 1 to 7 into the scale note of the church mode.</item>
    /// </list>
    /// </remarks>
    public class Span {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Number of beats for this span.
        /// </summary>
        int _beat;

        /// <summary>
        /// Key of the song for this span.
        /// </summary>
        Key _key;

        /// <summary>
        /// Degree from the key of the song.
        /// </summary>
        Degree _degree;

        /// <summary>
        /// Church mode of the key of the song.
        /// </summary>
        Mode _key_mode;

        /// <summary>
        /// Church mode of this span.
        /// </summary>
        Mode _span_mode;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Span"/> class with beat and degree.
        /// </summary>
        /// <param name="beat">Number of beats for this span.</param>
        /// <param name="degree">Degree from the key of the song.</param>
        public Span(int beat, Degree degree) {
            _beat = beat;
            _key = Key.Undefined;
            _degree = degree;
            _key_mode = Mode.Undefined;
            _span_mode = Mode.Undefined;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Span"/> class with beat, degree, and span mode.
        /// </summary>
        /// <param name="beat">Number of beats for this span.</param>
        /// <param name="degree">Degree from the key of the song.</param>
        /// <param name="span_mode">Church mode of this span.</param>
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
        /// <remarks>
        /// <list type="bullet">
        /// <item>Represents the church mode (e.g., Ionian, Dorian) for the key.</item>
        /// </list>
        /// </remarks>
        public Mode KeyMode { get => _key_mode; set => _key_mode = value; }

        /// <summary>
        /// Gets the church mode of this span.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Do not change the span mode once set.</item>
        /// </list>
        /// </remarks>
        public Mode SpanMode { get => _span_mode; } // Do not change the span mode once set.
    }
}
