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

namespace Meowziq {
    /// <summary>
    /// Provides utility methods for music theory calculations and conversions.
    /// </summary>
    /// <remarks>
    /// <para>This class must be top level.</para>
    /// <para>Do not use implementation packages here.</para>
    /// <para>Arguments must be primitive types or enums only.</para>
    /// <para>Do not define variables with <c>var</c> in this class.</para>
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Utils {

        ///////////////////////////////////////////////////////////////////////////////////////////
        // Const [nouns]

        /// <summary>
        /// Gets the offset for zero-based indexing.
        /// </summary>
        const int TO_ZERO_BASE = 1;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Gets the random number generator for internal use.
        /// </summary>
        static Random _random = new Random();

        ///////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Gets the length in 16th notes for the specified index value.
        /// </summary>
        public static int To16beatLength(int index) {
            return Length.Of16beat.Int32() * index;
        }

        /// <summary>
        /// Gets the number of 16th notes in the specified beat count (one beat).
        /// </summary>
        public static int To16beatCount(int beat_count) {
            return beat_count * 4; // Multiplies by 4 because there are 4 16th notes in one beat.
        }

        /// <summary>
        /// Gets an array of note numbers from a phrase chord notation (1-9).
        /// If the span mode is undefined, determines the mode from the key mode and degree.
        /// If the span mode is defined, uses the specified span mode.
        /// </summary>
        public static int[] ToNoteArray(Key key, Degree degree, Mode key_mode, Mode span_mode, int number) {
            int degree_note = rootNoteBy(key, degree, key_mode); // Gets the root note of the degree from the key and key mode.
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree, key_mode) : span_mode; // If the span mode is undefined, determines the mode automatically from the degree and key mode.
            int[] scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // Gets the scale for the root note and mode.
            return noteArrayBy(number, scale7); // Returns an array of chord tones corresponding to the given index from the mode scale.
        }

        /// <summary>
        /// Gets the note number from a phrase note notation (1-7).
        /// If <paramref name="auto_note"/> is true, determines the mode automatically.
        /// If the span mode is undefined, determines the mode from the key mode and degree.
        /// If the span mode is defined, uses the specified span mode.
        /// </summary>
        public static int ToNote(Key key, Degree degree, Mode key_mode, Mode span_mode, int number, bool auto_note = true) {
            int degree_note = rootNoteBy(key, degree, key_mode); // Gets the root note of the degree from the key and key mode.
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree, key_mode) : span_mode; // Gets the mode for the degree, using the span mode if defined, otherwise determining it from the key mode.
            int[] scale7 = null;
            if (auto_note) { // Uses the mode based on the span (automatic or specified)
                scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // Gets the scale for the root note and mode.
            } else { // Determines the mode from the key automatically and gets the scale.
                Mode key_mode_current = ToKeyMode(key, degree, key_mode, span_mode: mode);
                if (key_mode_current is not Mode.Undefined) { // If the mode can be determined
                    scale7 = scale7By(key, key_mode: key_mode_current); // Gets the scale for the determined mode.
                }
            }
            return scale7[number - TO_ZERO_BASE]; // Returns the note number from the zero-based scale array for the given index.
        }

        /// <summary>
        /// Gets a random note from the specified parameters.
        /// </summary>
        /// <remarks>
        /// FIXME: This method may need further refinement.
        /// </remarks>
        public static int ToRandomNote(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            return noteOfRandom3By(key, degree, key_mode, span_mode);
        }

        /// <summary>
        /// Gets the church mode used for the specified degree, based on the church mode of the song's key and the degree.
        /// </summary>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The key mode to use.</param>
        /// <returns>The span mode for the given degree and key mode.</returns>
        public static Mode ToSpanMode(Degree degree, Mode key_mode) {
            return spanModeBy(degree, key_mode);
        }

        /// <summary>
        /// Gets the church mode of the song's key from the church mode of the span.
        /// </summary>
        /// <param name="key">The key of the song.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The key mode of the song.</param>
        /// <param name="span_mode">The span mode to use.</param>
        /// <returns>The key mode for the given parameters, or <see cref="Mode.Undefined"/> if the scales do not match.</returns>
        public static Mode ToKeyMode(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            Mode key_mode_maybe = keyModeMaybeBy(degree, span_mode); // Gets the church mode of the key from the church mode of the span and that degree.
            int[] key_scale = scale7By(key, key_mode: key_mode_maybe); // Creates a scale from the degree and church mode of the key.
            int degree_note = rootNoteBy(key, degree, key_mode); // Gets the root note number from the key, the degree, and the mode of the song.
            int[] span_scale = scale7By(key: Key.Enum.Parse(degree_note), key_mode: span_mode); // Creates a scale from the degree and church mode of span.
            if (!compareScale(scale1: key_scale, scale2: span_scale)) { // Two scales should be the same.
                return Mode.Undefined; // When two scales are different.
            }
            return key_mode_maybe;
        }

        /// <summary>
        /// Gets a simple chord name as major or minor.
        /// </summary>
        /// <param name="key">The key of the song.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The key mode of the song.</param>
        /// <param name="span_mode">The span mode to use.</param>
        /// <param name="auto_mode">If true, determines the mode automatically; otherwise, uses the specified span mode.</param>
        /// <returns>The chord name as a string (e.g., "C", "Cm").</returns>
        public static string ToSimpleCodeName(Key key, Degree degree, Mode key_mode, Mode span_mode, bool auto_mode = true) {
            int degree_note = rootNoteBy(key, degree, key_mode); // Gets the root note of the degree from the degree and the key mode of the song.
            string code_base = Key.Enum.Parse(degree_note).ToString(); // Gets the code name basics.
            Mode mode;
            if (auto_mode) {
                mode = spanModeBy(degree, key_mode); // Gets the church mode of this span.
            } else {
                mode = span_mode; // Applies Span mode as is.
            }
            string major_or_miner_string = majorOrMiner(mode: mode) ? string.Empty : "m";
            return code_base + major_or_miner_string;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Determines whether two scales have the same notes.
        /// </summary>
        /// <param name="scale1">The first scale as an array of note numbers.</param>
        /// <param name="scale2">The second scale as an array of note numbers.</param>
        /// <returns>True if both scales contain the same notes; otherwise, false.</returns>
        static bool compareScale(int[] scale1, int[] scale2) {
            List<Key> list1 = scale1.Select(selector: x => Key.Enum.Parse(target: x)).OrderBy(keySelector: x => x).ToList();
            List<Key> list2 = scale2.Select(selector: x => Key.Enum.Parse(target: x)).OrderBy(keySelector: x => x).ToList();
            IEnumerable<Key> result = list1.Where(predicate: x => !list2.Contains(item: x));
            if (result.Count() == 0) {
                return true; // Scale notes match.
            }
            return false; // Scale notes not match.
        }

        /// <summary>
        /// Determines whether the specified church mode is major or minor.
        /// </summary>
        /// <param name="mode">The church mode to check.</param>
        /// <returns>True if the mode is major (Lydian, Ionian, Mixolydian); false if minor (Dorian, Aeolian, Phrygian, Locrian).</returns>
        static bool majorOrMiner(Mode mode) {
            switch (mode) {
                case Mode.Lyd:
                case Mode.Ion:
                case Mode.Mix:
                    return true;
                case Mode.Dor:
                case Mode.Aeo:
                case Mode.Phr:
                case Mode.Loc:
                    return false;
                default:
                    throw new ArgumentException("Invalid mode.");
            }
        }

        /// <summary>
        /// Gets an array of note numbers from a chord notation (3-9).
        /// </summary>
        /// <param name="number">The chord type (3, 4, 5, 6, 7, or 9).</param>
        /// <param name="scale7">The scale notes to use for the chord.</param>
        /// <returns>An array of note numbers representing the chord.</returns>
        static int[] noteArrayBy(int number, int[] scale7) {
            // TODO: Validate
            int[] note2 = new int[2]; // Extracts two notes as chord notes from scale notes.
            int[] note3 = new int[3]; // Extracts three notes as chord notes from scale notes.
            int[] note4 = new int[4]; // Extracts four notes as chord notes from scale notes.
            switch (number) {
                // FIXME: index = 2.
                case 3: // e.g. C, Cm, Cm(b5)
                    note3[0] = scale7[1 - TO_ZERO_BASE];
                    note3[1] = scale7[3 - TO_ZERO_BASE];
                    note3[2] = scale7[5 - TO_ZERO_BASE];
                    return note3;
                case 4: // e.g. C(#4), Csus4, Csus4(-5)
                    note3[0] = scale7[1 - TO_ZERO_BASE];
                    note3[1] = scale7[4 - TO_ZERO_BASE];
                    note3[2] = scale7[5 - TO_ZERO_BASE];
                    return note3;
                case 5: // e.g. C5(no3), C(-5,no3)
                    note2[0] = scale7[1 - TO_ZERO_BASE];
                    note2[1] = scale7[5 - TO_ZERO_BASE];
                    return note2;
                case 6: // e.g. C6, Cm6, Cm(b6), Cm(-5,b6)
                    note4[0] = scale7[1 - TO_ZERO_BASE];
                    note4[1] = scale7[3 - TO_ZERO_BASE];
                    note4[2] = scale7[5 - TO_ZERO_BASE];
                    note4[3] = scale7[6 - TO_ZERO_BASE];
                    return note4;
                case 7: // e.g. CM7, C7, Cm7, Cm7(-5)
                    note4[0] = scale7[1 - TO_ZERO_BASE];
                    note4[1] = scale7[3 - TO_ZERO_BASE];
                    note4[2] = scale7[5 - TO_ZERO_BASE];
                    note4[3] = scale7[7 - TO_ZERO_BASE];
                    return note4;
                // TODO: two octaves in 8?
                case 9: // e.g. Cadd9, Cmadd9, Cm(b9), Cm(-5,b9)
                    note4[0] = scale7[1 - TO_ZERO_BASE];
                    note4[1] = scale7[3 - TO_ZERO_BASE];
                    note4[2] = scale7[5 - TO_ZERO_BASE];
                    note4[3] = scale7[2 - TO_ZERO_BASE] + 12; // 1 octave up. // FIXME: Considers not supported Range.
                    return note4;
                default:
                    throw new ArgumentException("Invalid index or scale7.");
            }
        }

        /// <summary>
        /// Gets the root note number from the key, degree, and mode of the song.
        /// </summary>
        /// <param name="key">The key to use.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The church mode to use.</param>
        /// <returns>The root note number for the given key, degree, and mode.</returns>
        static int rootNoteBy(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key, key_mode);
            return scale7[(int) degree];
        }

        /// <summary>
        /// Gets the three constituent notes of a chord for the specified degree.
        /// </summary>
        /// <param name="key">The key for which to get the chord notes.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The church mode to use for the chord.</param>
        /// <returns>An array of three note numbers representing the chord for the given degree.</returns>
        static int[] noteArray3By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key, key_mode);
            int[] note3 = new int[3];
            switch (degree) {
                case Degree.I:
                    note3[0] = scale7[1 - TO_ZERO_BASE];
                    note3[1] = scale7[3 - TO_ZERO_BASE];
                    note3[2] = scale7[5 - TO_ZERO_BASE];
                    break;
                case Degree.II:
                    note3[0] = scale7[2 - TO_ZERO_BASE];
                    note3[1] = scale7[4 - TO_ZERO_BASE];
                    note3[2] = scale7[6 - TO_ZERO_BASE];
                    break;
                case Degree.III:
                    note3[0] = scale7[3 - TO_ZERO_BASE];
                    note3[1] = scale7[5 - TO_ZERO_BASE];
                    note3[2] = scale7[7 - TO_ZERO_BASE];
                    break;
                case Degree.IV:
                    note3[0] = scale7[4 - TO_ZERO_BASE];
                    note3[1] = scale7[6 - TO_ZERO_BASE];
                    note3[2] = scale7[1 - TO_ZERO_BASE];
                    break;
                case Degree.V:
                    note3[0] = scale7[5 - TO_ZERO_BASE];
                    note3[1] = scale7[7 - TO_ZERO_BASE];
                    note3[2] = scale7[2 - TO_ZERO_BASE];
                    break;
                case Degree.VI:
                    note3[0] = scale7[6 - TO_ZERO_BASE];
                    note3[1] = scale7[1 - TO_ZERO_BASE];
                    note3[2] = scale7[3 - TO_ZERO_BASE];
                    break;
                case Degree.VII:
                    note3[0] = scale7[7 - TO_ZERO_BASE];
                    note3[1] = scale7[2 - TO_ZERO_BASE];
                    note3[2] = scale7[4 - TO_ZERO_BASE];
                    break;
            }
            return note3;
        }

        /// <summary>
        /// Gets the four constituent notes of a chord for the specified degree.
        /// </summary>
        /// <param name="key">The key for which to get the chord notes.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The church mode to use for the chord.</param>
        /// <returns>An array of four note numbers representing the chord for the given degree.</returns>
        static int[] noteArray4By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key, key_mode);
            int[] note4 = new int[4];
            switch (degree) {
                case Degree.I:
                    note4[0] = scale7[1 - TO_ZERO_BASE];
                    note4[1] = scale7[3 - TO_ZERO_BASE];
                    note4[2] = scale7[5 - TO_ZERO_BASE];
                    note4[3] = scale7[7 - TO_ZERO_BASE];
                    break;
                case Degree.II:
                    note4[0] = scale7[2 - TO_ZERO_BASE];
                    note4[1] = scale7[4 - TO_ZERO_BASE];
                    note4[2] = scale7[6 - TO_ZERO_BASE];
                    note4[3] = scale7[1 - TO_ZERO_BASE];
                    break;
                case Degree.III:
                    note4[0] = scale7[3 - TO_ZERO_BASE];
                    note4[1] = scale7[5 - TO_ZERO_BASE];
                    note4[2] = scale7[7 - TO_ZERO_BASE];
                    note4[3] = scale7[2 - TO_ZERO_BASE];
                    break;
                case Degree.IV:
                    note4[0] = scale7[4 - TO_ZERO_BASE];
                    note4[1] = scale7[6 - TO_ZERO_BASE];
                    note4[2] = scale7[1 - TO_ZERO_BASE];
                    note4[3] = scale7[3 - TO_ZERO_BASE];
                    break;
                case Degree.V:
                    note4[0] = scale7[5 - TO_ZERO_BASE];
                    note4[1] = scale7[7 - TO_ZERO_BASE];
                    note4[2] = scale7[2 - TO_ZERO_BASE];
                    note4[3] = scale7[4 - TO_ZERO_BASE];
                    break;
                case Degree.VI:
                    note4[0] = scale7[6 - TO_ZERO_BASE];
                    note4[1] = scale7[1 - TO_ZERO_BASE];
                    note4[2] = scale7[3 - TO_ZERO_BASE];
                    note4[3] = scale7[5 - TO_ZERO_BASE];
                    break;
                case Degree.VII:
                    note4[0] = scale7[7 - TO_ZERO_BASE];
                    note4[1] = scale7[2 - TO_ZERO_BASE];
                    note4[2] = scale7[4 - TO_ZERO_BASE];
                    note4[3] = scale7[6 - TO_ZERO_BASE];
                    break;
            }
            return note4;
        }

        /// <summary>
        /// Gets the seven constituent notes of a chord for the specified degree.
        /// </summary>
        /// <param name="key">The key for which to get the chord notes.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The church mode to use for the chord.</param>
        /// <returns>An array of seven note numbers representing the chord for the given degree.</returns>
        static int[] noteArray7By(Key key, Degree degree, Mode key_mode) {
            key.Validate();
            degree.Validate();
            key_mode.Validate();
            int[] scale7 = scale7By(key, key_mode);
            int[] note7 = new int[7];
            switch (degree) {
                case Degree.I:
                    note7[0] = scale7[1 - TO_ZERO_BASE];
                    note7[1] = scale7[3 - TO_ZERO_BASE];
                    note7[2] = scale7[5 - TO_ZERO_BASE];
                    note7[3] = scale7[7 - TO_ZERO_BASE];
                    note7[4] = scale7[2 - TO_ZERO_BASE];
                    note7[5] = scale7[4 - TO_ZERO_BASE];
                    note7[6] = scale7[6 - TO_ZERO_BASE];
                    break;
                case Degree.II:
                    note7[0] = scale7[2 - TO_ZERO_BASE];
                    note7[1] = scale7[4 - TO_ZERO_BASE];
                    note7[2] = scale7[6 - TO_ZERO_BASE];
                    note7[3] = scale7[1 - TO_ZERO_BASE];
                    note7[4] = scale7[3 - TO_ZERO_BASE];
                    note7[5] = scale7[5 - TO_ZERO_BASE];
                    note7[6] = scale7[7 - TO_ZERO_BASE];
                    break;
                case Degree.III:
                    note7[0] = scale7[3 - TO_ZERO_BASE];
                    note7[1] = scale7[5 - TO_ZERO_BASE];
                    note7[2] = scale7[7 - TO_ZERO_BASE];
                    note7[3] = scale7[2 - TO_ZERO_BASE];
                    note7[4] = scale7[4 - TO_ZERO_BASE];
                    note7[5] = scale7[6 - TO_ZERO_BASE];
                    note7[6] = scale7[1 - TO_ZERO_BASE];
                    break;
                case Degree.IV:
                    note7[0] = scale7[4 - TO_ZERO_BASE];
                    note7[1] = scale7[6 - TO_ZERO_BASE];
                    note7[2] = scale7[1 - TO_ZERO_BASE];
                    note7[3] = scale7[3 - TO_ZERO_BASE];
                    note7[4] = scale7[5 - TO_ZERO_BASE];
                    note7[5] = scale7[7 - TO_ZERO_BASE];
                    note7[6] = scale7[2 - TO_ZERO_BASE];
                    break;
                case Degree.V:
                    note7[0] = scale7[5 - TO_ZERO_BASE];
                    note7[1] = scale7[7 - TO_ZERO_BASE];
                    note7[2] = scale7[2 - TO_ZERO_BASE];
                    note7[3] = scale7[4 - TO_ZERO_BASE];
                    note7[4] = scale7[6 - TO_ZERO_BASE];
                    note7[5] = scale7[1 - TO_ZERO_BASE];
                    note7[6] = scale7[3 - TO_ZERO_BASE];
                    break;
                case Degree.VI:
                    note7[0] = scale7[6 - TO_ZERO_BASE];
                    note7[1] = scale7[1 - TO_ZERO_BASE];
                    note7[2] = scale7[3 - TO_ZERO_BASE];
                    note7[3] = scale7[5 - TO_ZERO_BASE];
                    note7[4] = scale7[7 - TO_ZERO_BASE];
                    note7[5] = scale7[2 - TO_ZERO_BASE];
                    note7[6] = scale7[4 - TO_ZERO_BASE];
                    break;
                case Degree.VII:
                    note7[0] = scale7[7 - TO_ZERO_BASE];
                    note7[1] = scale7[2 - TO_ZERO_BASE];
                    note7[2] = scale7[4 - TO_ZERO_BASE];
                    note7[3] = scale7[6 - TO_ZERO_BASE];
                    note7[4] = scale7[1 - TO_ZERO_BASE];
                    note7[5] = scale7[3 - TO_ZERO_BASE];
                    note7[6] = scale7[5 - TO_ZERO_BASE];
                    break;
            }
            return note7;
        }

        /// <summary>
        /// Gets the scale notes of the specified church mode from the key and mode.
        /// </summary>
        /// <param name="key">The key for which to generate the scale.</param>
        /// <param name="key_mode">The church mode to use for the scale.</param>
        /// <returns>An array of note numbers representing the scale for the given key and mode.</returns>
        static int[] scale7By(Key key, Mode key_mode) {
            key.Validate();
            key_mode.Validate();
            int root_note = (int) key;
            /// <note>
            /// Creates a mode scale consisting of seven notes.
            /// </note>
            int[] scale7 = new int[7];
            switch (key_mode) {
                /// <note>
                /// Lydian mode scale.
                /// </note>
                case Mode.Lyd:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 2;  //  2
                    scale7[2] = root_note + 4;  //  3
                    scale7[3] = root_note + 6;  // #4
                    scale7[4] = root_note + 7;  //  5
                    scale7[5] = root_note + 9;  //  6
                    scale7[6] = root_note + 11; //  7
                    break;
                /// <note>
                /// Ionian mode scale.
                /// </note>
                case Mode.Ion:
                    scale7[0] = root_note;      // 1
                    scale7[1] = root_note + 2;  // 2
                    scale7[2] = root_note + 4;  // 3
                    scale7[3] = root_note + 5;  // 4
                    scale7[4] = root_note + 7;  // 5
                    scale7[5] = root_note + 9;  // 6
                    scale7[6] = root_note + 11; // 7
                    break;
                /// <note>
                /// Mixolydian mode scale.
                /// </note>
                case Mode.Mix:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 2;  //  2
                    scale7[2] = root_note + 4;  //  3
                    scale7[3] = root_note + 5;  //  4
                    scale7[4] = root_note + 7;  //  5
                    scale7[5] = root_note + 9;  //  6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// Dorian mode scale.
                /// </note>
                case Mode.Dor:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 2;  //  2
                    scale7[2] = root_note + 3;  // b3
                    scale7[3] = root_note + 5;  //  4
                    scale7[4] = root_note + 7;  //  5
                    scale7[5] = root_note + 9;  //  6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// Aeolian mode scale.
                /// </note>
                case Mode.Aeo:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 2;  //  2
                    scale7[2] = root_note + 3;  // b3
                    scale7[3] = root_note + 5;  //  4
                    scale7[4] = root_note + 7;  //  5
                    scale7[5] = root_note + 8;  // b6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// Phrygian mode scale.
                /// </note>
                case Mode.Phr:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 1;  // b2
                    scale7[2] = root_note + 3;  // b3
                    scale7[3] = root_note + 5;  //  4
                    scale7[4] = root_note + 7;  //  5
                    scale7[5] = root_note + 8;  // b6
                    scale7[6] = root_note + 10; // b7
                    break;
                /// <note>
                /// Locrian mode scale.
                /// </note>
                case Mode.Loc:
                    scale7[0] = root_note;      //  1
                    scale7[1] = root_note + 1;  // b2
                    scale7[2] = root_note + 3;  // b3
                    scale7[3] = root_note + 5;  //  4
                    scale7[4] = root_note + 6;  // b5
                    scale7[5] = root_note + 8;  // b6
                    scale7[6] = root_note + 10; // b7
                    break;
            }
            return scale7;
        }

        /// <summary>
        /// Gets the church mode of the key from the church mode of the span and the specified degree.
        /// </summary>
        /// <param name="degree">The degree to use.</param>
        /// <param name="span_mode">The span mode to use.</param>
        /// <returns>The key mode for the given degree and span mode.</returns>
        static Mode keyModeMaybeBy(Degree degree, Mode span_mode) {
            span_mode.Validate();
            degree.Validate();
            if (span_mode is Mode.Lyd) {
                switch (degree) {
                    case Degree.I:   return Mode.Lyd;
                    case Degree.II:  return Mode.Phr;
                    case Degree.III: return Mode.Dor;
                    case Degree.IV:  return Mode.Ion;
                    case Degree.V:   return Mode.Loc;
                    case Degree.VI:  return Mode.Aeo;
                    case Degree.VII: return Mode.Mix;
                }
            } else if (span_mode is Mode.Ion) {
                switch (degree) {
                    case Degree.I:   return Mode.Ion;
                    case Degree.II:  return Mode.Loc;
                    case Degree.III: return Mode.Aeo;
                    case Degree.IV:  return Mode.Mix;
                    case Degree.V:   return Mode.Lyd;
                    case Degree.VI:  return Mode.Phr;
                    case Degree.VII: return Mode.Dor;
                }
            } else if (span_mode is Mode.Mix) {
                switch (degree) {
                    case Degree.I:   return Mode.Mix;
                    case Degree.II:  return Mode.Lyd;
                    case Degree.III: return Mode.Phr;
                    case Degree.IV:  return Mode.Dor;
                    case Degree.V:   return Mode.Ion;
                    case Degree.VI:  return Mode.Loc;
                    case Degree.VII: return Mode.Aeo;
                }
            } else if (span_mode is Mode.Dor) {
                switch (degree) {
                    case Degree.I:   return Mode.Dor;
                    case Degree.II:  return Mode.Ion;
                    case Degree.III: return Mode.Loc;
                    case Degree.IV:  return Mode.Aeo;
                    case Degree.V:   return Mode.Mix;
                    case Degree.VI:  return Mode.Lyd;
                    case Degree.VII: return Mode.Phr;
                }
            } else if (span_mode is Mode.Aeo) {
                switch (degree) {
                    case Degree.I:   return Mode.Aeo;
                    case Degree.II:  return Mode.Mix;
                    case Degree.III: return Mode.Lyd;
                    case Degree.IV:  return Mode.Phr;
                    case Degree.V:   return Mode.Dor;
                    case Degree.VI:  return Mode.Ion;
                    case Degree.VII: return Mode.Loc;
                }
            } else if (span_mode is Mode.Phr) {
                switch (degree) {
                    case Degree.I:   return Mode.Phr;
                    case Degree.II:  return Mode.Dor;
                    case Degree.III: return Mode.Ion;
                    case Degree.IV:  return Mode.Loc;
                    case Degree.V:   return Mode.Aeo;
                    case Degree.VI:  return Mode.Mix;
                    case Degree.VII: return Mode.Lyd;
                }
            } else if (span_mode is Mode.Loc) {
                switch (degree) {
                    case Degree.I:   return Mode.Loc;
                    case Degree.II:  return Mode.Aeo;
                    case Degree.III: return Mode.Mix;
                    case Degree.IV:  return Mode.Lyd;
                    case Degree.V:   return Mode.Phr;
                    case Degree.VI:  return Mode.Dor;
                    case Degree.VII: return Mode.Ion;
                }
            }
            throw new ArgumentException("Invalid degree or span_mode.");
        }

        /// <summary>
        /// Gets the church mode used for the specified degree, based on the church mode of the song's key and the degree.
        /// </summary>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The key mode to use.</param>
        /// <returns>The span mode for the given degree and key mode.</returns>
        static Mode spanModeBy(Degree degree, Mode key_mode) {
            key_mode.Validate();
            degree.Validate();
            if (key_mode is Mode.Lyd) {
                switch (degree) {
                    case Degree.I:   return Mode.Lyd;
                    case Degree.II:  return Mode.Mix;
                    case Degree.III: return Mode.Aeo;
                    case Degree.IV:  return Mode.Loc;
                    case Degree.V:   return Mode.Ion;
                    case Degree.VI:  return Mode.Dor;
                    case Degree.VII: return Mode.Phr;
                }
            } else if (key_mode is Mode.Ion) {
                switch (degree) {
                    case Degree.I:   return Mode.Ion;
                    case Degree.II:  return Mode.Dor;
                    case Degree.III: return Mode.Phr;
                    case Degree.IV:  return Mode.Lyd;
                    case Degree.V:   return Mode.Mix;
                    case Degree.VI:  return Mode.Aeo;
                    case Degree.VII: return Mode.Loc;
                }
            } else if (key_mode is Mode.Mix) {
                switch (degree) {
                    case Degree.I:   return Mode.Mix;
                    case Degree.II:  return Mode.Aeo;
                    case Degree.III: return Mode.Loc;
                    case Degree.IV:  return Mode.Ion;
                    case Degree.V:   return Mode.Dor;
                    case Degree.VI:  return Mode.Phr;
                    case Degree.VII: return Mode.Lyd;
                }
            } else if (key_mode is Mode.Dor) {
                switch (degree) {
                    case Degree.I:   return Mode.Dor;
                    case Degree.II:  return Mode.Phr;
                    case Degree.III: return Mode.Lyd;
                    case Degree.IV:  return Mode.Mix;
                    case Degree.V:   return Mode.Aeo;
                    case Degree.VI:  return Mode.Loc;
                    case Degree.VII: return Mode.Ion;
                }
            } else if (key_mode is Mode.Aeo) {
                switch (degree) {
                    case Degree.I:   return Mode.Aeo;
                    case Degree.II:  return Mode.Loc;
                    case Degree.III: return Mode.Ion;
                    case Degree.IV:  return Mode.Dor;
                    case Degree.V:   return Mode.Phr;
                    case Degree.VI:  return Mode.Lyd;
                    case Degree.VII: return Mode.Mix;
                }
            } else if (key_mode is Mode.Phr) {
                switch (degree) {
                    case Degree.I:   return Mode.Phr;
                    case Degree.II:  return Mode.Lyd;
                    case Degree.III: return Mode.Mix;
                    case Degree.IV:  return Mode.Aeo;
                    case Degree.V:   return Mode.Loc;
                    case Degree.VI:  return Mode.Ion;
                    case Degree.VII: return Mode.Dor;
                }
            } else if (key_mode is Mode.Loc) {
                switch (degree) {
                    case Degree.I:   return Mode.Loc;
                    case Degree.II:  return Mode.Ion;
                    case Degree.III: return Mode.Dor;
                    case Degree.IV:  return Mode.Phr;
                    case Degree.V:   return Mode.Lyd;
                    case Degree.VI:  return Mode.Mix;
                    case Degree.VII: return Mode.Aeo;
                }
            }
            throw new ArgumentException("Invalid degree or key_mode.");
        }

        /// <summary>
        /// Gets a random triad note from the specified key, degree, and mode.
        /// </summary>
        /// <param name="key">The key of the song.</param>
        /// <param name="degree">The degree to use.</param>
        /// <param name="key_mode">The key mode to use.</param>
        /// <param name="span_mode">The span mode to use. If undefined, determines the mode automatically.</param>
        /// <returns>A random note number from the triad for the given parameters.</returns>
        static int noteOfRandom3By(Key key, Degree degree, Mode key_mode, Mode span_mode) {
            Mode mode = span_mode is Mode.Undefined ? spanModeBy(degree, key_mode) : span_mode; // Gets the mode for the degree, using the span mode if defined, otherwise determining it from the key mode.
            int[] key_scale = scale7By(key, key_mode); // Gets the scale for the key and key mode.
            int degree_note = key_scale[(int) degree]; // Gets the root note for the degree.
            int[] scale7 = scale7By(key: Key.Enum.Parse(degree_note), key_mode: mode); // Gets the scale for the root note and mode.
            int[] note_array3 = noteArrayBy(number: 3, scale7: scale7); // Gets the triad notes from the scale.
            int random = _random.Next(minValue: 0, maxValue: 3); // Gets a random note from the triad.
            return note_array3[random];
        }
    }
}
