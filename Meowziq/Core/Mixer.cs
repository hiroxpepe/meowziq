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

namespace Meowziq.Core {
    /// <summary>
    /// Provides mixer functionality for handling faders, program changes, volume, pan, and mute operations.
    /// </summary>
    /// <typeparam name="T">Type of the message element.</typeparam>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Mixer<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        /// <summary>
        /// Indicates whether the mixer is in use.
        /// </summary>
        static bool _use;

        /// <summary>
        /// Stores the previous fader map (key: "type").
        /// </summary>
        static Map<string, Fader> _previous_fader_map;

        /// <summary>
        /// Stores the current fader map (key: "type:name").
        /// </summary>
        static Map<string, Fader> _current_fader_map;

        /// <summary>
        /// Stores the message object for note and control changes.
        /// </summary>
        static IMessage<T, Note> _message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        /// <summary>
        /// Initializes static members of the <see cref="Mixer{T}"/> class.
        /// </summary>
        static Mixer() {
            _use = false;
            _previous_fader_map = new();
            _current_fader_map = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// Adds a Fader object to the current fader map and updates the previous fader map.
        /// </summary>
        /// <value>Fader object to add.</value>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Key for current map: "type:name"</item>
        /// <item>Key for previous map: "type"</item>
        /// <item>Sets _use to true when a Fader is added.</item>
        /// </list>
        /// </remarks>
        /// <todo>
        /// Fix bug in SMF output.
        /// </todo>
        public static Fader AddFader {
            set {
                if (value is null) { return; }
                _current_fader_map[$"{value.Type}:{value.Name}"] = value;
                _previous_fader_map[value.Type] = Fader.NoVaule(type: value.Type);
                _use = true; // Added Fader, so mixier.json exists.
            }
        }

        /// <summary>
        /// Sets the Message object for the mixer.
        /// </summary>
        /// <value>Message object for note and control changes.</value>
        public static IMessage<T, Note> Message { set => _message = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// Clears all fader maps and resets the mixer state.
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Runs only once the first time.</item>
        /// </list>
        /// </remarks>
        public static void Clear() {
            _previous_fader_map.Clear();
            _current_fader_map.Clear();
            _use = false;
        }

        /// <summary>
        /// Applies the Note object to the Message object at the specified tick and MIDI channel.
        /// </summary>
        /// <param name="tick">Tick position to apply the note.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="note">Note object to apply.</param>
        public static void ApplyNote(int tick, int midi_ch, Note note) {
            _message.ApplyNote(tick, midi_ch, note);
        }

        /// <summary>
        /// Applies program change, volume, pan, and other parameters to the Message object.
        /// </summary>
        /// <param name="tick">Tick position to apply the value.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        /// <param name="program_num">Program number (timbre).</param>
        /// <remarks>
        /// <list type="bullet">
        /// <item>Applying values is only executed at pattern changes.</item>
        /// </list>
        /// </remarks>
        public static void ApplyVaule(int tick, int midi_ch, string type, string name, int program_num) {
            if (!_use && !_current_fader_map.ContainsKey(key: $"{type}:default")) {
                _previous_fader_map[type] = Fader.NoVaule(type);
                _current_fader_map[$"{type}:default"] = Fader.Default(type: type); // First time without mixer.json.
            }
            if (_use && !_current_fader_map.ContainsKey(key: $"{type}:{name}")) {
                return; // Missing key when using mixer.json.
            }
            playerProgramNum = (programNum: program_num, type: type, name: name);
            applyValueBy(tick, midi_ch, type, name);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        /// <summary>
        /// Sets the instrument name and program number in player.json.
        /// </summary>
        /// <value>Tuple of program number, type, and name.</value>
        static (int programNum, string type, string name) playerProgramNum {
            set {
                if (!_use) { value.name = "default"; } // No mixer.json is always "default".
                _current_fader_map[$"{value.type}:{value.name}"].PlayerProgramNum = value.programNum;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        /// <summary>
        /// Applies all value changes (program, volume, pan, mute) for the specified instrument.
        /// </summary>
        /// <param name="tick">Tick position to apply the values.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        static void applyValueBy(int tick, int midi_ch, string type, string name) {
            applyProgramChangeBy(tick, midi_ch, type, name);
            applyVolumeBy(tick, midi_ch, type, name);
            applyPanBy(tick, midi_ch, type, name);
            applyMuteBy(tick, midi_ch, type, name);
        }

        /// <summary>
        /// Applies program change for the specified instrument if changed.
        /// </summary>
        /// <param name="tick">Tick position to apply the program change.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        static void applyProgramChangeBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // No mixer.json is always "default".
            if (changedProgramNum(type, name)) {
                int program_num = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
                _message.ApplyProgramChange(tick, midi_ch, program_num);
            }
        }

        /// <summary>
        /// Applies volume for the specified instrument if changed.
        /// </summary>
        /// <param name="tick">Tick position to apply the volume.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        static void applyVolumeBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // No mixer.json is always "default".
            if (changedVol(type, name)) {
                _message.ApplyVolume(tick, midi_ch, volume: _current_fader_map[$"{type}:{name}"].Vol);
            }
        }

        /// <summary>
        /// Applies pan for the specified instrument if changed.
        /// </summary>
        /// <param name="tick">Tick position to apply the pan.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        static void applyPanBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // No mixer.json is always "default".
            if (changedPan(type, name)) {
                _message.ApplyPan(tick, midi_ch, pan: _current_fader_map[$"{type}:{name}"].Pan);
            }
        }

        /// <summary>
        /// Applies mute for the specified instrument.
        /// </summary>
        /// <param name="tick">Tick position to apply mute.</param>
        /// <param name="midi_ch">MIDI channel.</param>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        static void applyMuteBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // No mixer.json is always "default".
            _message.ApplyMute(tick, midi_ch, mute: _current_fader_map[$"{type}:{name}"].Mute);
        }

        /// <summary>
        /// Gets a value indicating whether the program number has changed for the specified instrument.
        /// </summary>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        /// <returns>True if the program number has changed; otherwise, false.</returns>
        static bool changedProgramNum(string type, string name) {
            int program_num = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
            if (_previous_fader_map[type].ProgramNum != program_num) {
                _previous_fader_map[type].ProgramNum = program_num;
                return true; // With value update.
            } 
            else if (_previous_fader_map[type].ProgramNum == program_num) {
                return false; // No value update.
            }
            throw new ArgumentException("Not ProgramNum.");
        }

        /// <summary>
        /// Gets a value indicating whether the volume has changed for the specified instrument.
        /// </summary>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        /// <returns>True if the volume has changed; otherwise, false.</returns>
        static bool changedVol(string type, string name) {
            int vol = _current_fader_map[$"{type}:{name}"].Vol;
            if (_previous_fader_map[type].Vol != vol) {
                _previous_fader_map[type].Vol = vol;
                return true; // With value update.
            } else if (_previous_fader_map[type].Vol == vol) {
                return false; // No value update.
            }
            throw new ArgumentException("Not Vol.");
        }

        /// <summary>
        /// Gets a value indicating whether the pan has changed for the specified instrument.
        /// </summary>
        /// <param name="type">Instrument type.</param>
        /// <param name="name">Instrument name.</param>
        /// <returns>True if the pan has changed; otherwise, false.</returns>
        static bool changedPan(string type, string name) {
            Pan pan = _current_fader_map[$"{type}:{name}"].Pan;
            if (_previous_fader_map[type].Pan != pan) {
                _previous_fader_map[type].Pan = pan;
                return true; // With value update.
            } else if (_previous_fader_map[type].Pan == pan) {
                return false; // No value update.
            }
            throw new ArgumentException("Not Pan.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// Represents a fader for instrument settings such as program, volume, pan, and mute.
        /// </summary>
        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            /// <summary>
            /// The instrument type name (associated with Player object).
            /// </summary>
            string _type, _name;

            /// <summary>
            /// The program number set in mixer.json.
            /// </summary>
            int _program_num;

            /// <summary>
            /// The program number set in player.json.
            /// </summary>
            int _player_program_num;

            /// <summary>
            /// The volume set in mixer.json.
            /// </summary>
            int _vol;

            /// <summary>
            /// The pan value set in mixer.json.
            /// </summary>
            Pan _pan;

            /// <summary>
            /// The mute value set in mixer.json.
            /// </summary>
            bool _mute = false;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            /// <summary>
            /// Gets or sets the instrument type name (associated with Player object).
            /// </summary>
            public string Type { get => _type; set => _type = value; }

            /// <summary>
            /// Gets or sets the instrument name (associated with Pattern object).
            /// </summary>
            public string Name { get => _name; set => _name = value; }

            /// <summary>
            /// Gets or sets the program number set in mixer.json.
            /// </summary>
            public int ProgramNum { get => _program_num; set => _program_num = value; }

            /// <summary>
            /// Gets or sets the program number set in player.json.
            /// </summary>
            public int PlayerProgramNum { get => _player_program_num; set => _player_program_num = value; }

            /// <summary>
            /// Gets or sets the volume set in mixer.json.
            /// </summary>
            public int Vol { get => _vol; set => _vol = value; }

            /// <summary>
            /// Gets or sets the pan value set in mixer.json.
            /// </summary>
            public Pan Pan { get => _pan; set => _pan = value; }

            /// <summary>
            /// Gets or sets the mute value set in mixer.json.
            /// </summary>
            public bool Mute { get => _mute; set => _mute = value; }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private static Properties [noun, adjective] 

            /// <summary>
            /// Creates a Fader object with undefined values (used for missing keys).
            /// </summary>
            /// <param name="type">Instrument type.</param>
            /// <returns>Fader object with undefined values.</returns>
            public static Fader NoVaule(string type) {
                return new Fader() {
                    _type = type,
                    _name = "undefined",
                    _program_num = -1,
                    _player_program_num = -1,
                    _vol = -1,
                    _pan = Pan.Undefined,
                    _mute = false
                };
            }

            /// <summary>
            /// Creates a default Fader object (used when no mixer.json is present).
            /// </summary>
            /// <param name="type">Instrument type.</param>
            /// <returns>Default Fader object.</returns>
            public static Fader Default(string type) {
                return new Fader() {
                    _type = type,
                    _name = "default",
                    _program_num = -1,
                    _player_program_num = -1,
                    _vol = 100,
                    _pan = Pan.Center,
                    _mute = false
                };
            }
        }
    }
}
