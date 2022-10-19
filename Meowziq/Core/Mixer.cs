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
    /// mixer class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public static class Mixer<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool _use;

        static Map<string, Fader> _previous_fader_map; // key should be of the form "type".

        static Map<string, Fader> _current_fader_map; // key should be of the form "type:name".

        static IMessage<T, Note> _message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Mixer() {
            _use = false;
            _previous_fader_map = new();
            _current_fader_map = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// adds a Fader object.
        /// </summary>
        /// <fixme>
        /// FIXME: bug in SMF output.
        /// </fixme>
        public static Fader AddFader {
            set {
                if (value is null) { return; }
                _current_fader_map[$"{value.Type}:{value.Name}"] = value;
                _previous_fader_map[value.Type] = Fader.NoVaule(type: value.Type);
                _use = true; // added Fader, so mixier.json exists.
            }
        }

        /// <summary>
        /// sets the Message object.
        /// </summary>
        public static IMessage<T, Note> Message {
            set => _message = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// initializes the state.
        /// </summary>
        /// <note>
        /// runs only once the first time.
        /// </note>
        public static void Clear() {
            _previous_fader_map.Clear();
            _current_fader_map.Clear();
            _use = false;
        }

        /// <summary>
        /// applies the Note object to the Message object.
        /// </summary>
        public static void ApplyNote(int tick, int midi_ch, Note note) {
            _message.ApplyNote(tick, midi_ch, note);
        }

        /// <summary>
        /// applies the program change, volume, pan, and other parameters to the Message object.
        /// </summary>
        public static void ApplyVaule(int tick, int midi_ch, string type, string name, int program_num) {
            if (!_use && !_current_fader_map.ContainsKey(key: $"{type}:default")) {
                _previous_fader_map[type] = Fader.NoVaule(type);
                _current_fader_map[$"{type}:default"] = Fader.Default(type: type); // first time without mixer.json.
            }
            if (_use && !_current_fader_map.ContainsKey(key: $"{type}:{name}")) {
                return; // missing key when using mixer.json.
            }
            playerProgramNum = (programNum: program_num, type: type, name: name);
            applyValueBy(tick, midi_ch, type, name);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        /// <summary>
        /// sets the instrument name in player.json.
        /// </summary>
        static (int programNum, string type, string name) playerProgramNum {
            set {
                if (!_use) { value.name = "default"; } // no mixer.json is always "default".
                _current_fader_map[$"{value.type}:{value.name}"].PlayerProgramNum = value.programNum;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static void applyValueBy(int tick, int midi_ch, string type, string name) {
            applyProgramChangeBy(tick, midi_ch, type, name);
            applyVolumeBy(tick, midi_ch, type, name);
            applyPanBy(tick, midi_ch, type, name);
            applyMuteBy(tick, midi_ch, type, name);
        }

        static void applyProgramChangeBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // no mixer.json is always "default".
            if (changedProgramNum(type, name)) {
                int program_num = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
                _message.ApplyProgramChange(tick, midi_ch, program_num);
            }
        }

        static void applyVolumeBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // no mixer.json is always "default".
            if (changedVol(type, name)) {
                _message.ApplyVolume(tick, midi_ch, volume: _current_fader_map[$"{type}:{name}"].Vol);
            }
        }

        static void applyPanBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // no mixer.json is always "default".
            if (changedPan(type, name)) {
                _message.ApplyPan(tick, midi_ch, pan: _current_fader_map[$"{type}:{name}"].Pan);
            }
        }

        static void applyMuteBy(int tick, int midi_ch, string type, string name) {
            if (!_use) { name = "default"; } // no mixer.json is always "default".
            _message.ApplyMute(tick, midi_ch, mute: _current_fader_map[$"{type}:{name}"].Mute);
        }

        /// <summary>
        /// gets whether the program number has changed.
        /// </summary>
        static bool changedProgramNum(string type, string name) {
            int program_num = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
            if (_previous_fader_map[type].ProgramNum != program_num) {
                _previous_fader_map[type].ProgramNum = program_num;
                return true; // with value update.
            } 
            else if (_previous_fader_map[type].ProgramNum == program_num) {
                return false; // no value update.
            }
            throw new ArgumentException("not ProgramNum.");
        }

        /// <summary>
        /// gets whether the volume has changed.
        /// </summary>
        static bool changedVol(string type, string name) {
            int vol = _current_fader_map[$"{type}:{name}"].Vol;
            if (_previous_fader_map[type].Vol != vol) {
                _previous_fader_map[type].Vol = vol;
                return true; // with value update.
            } else if (_previous_fader_map[type].Vol == vol) {
                return false; // no value update.
            }
            throw new ArgumentException("not Vol.");
        }

        /// <summary>
        /// gets whether the pan has changed.
        /// </summary>
        static bool changedPan(string type, string name) {
            Pan pan = _current_fader_map[$"{type}:{name}"].Pan;
            if (_previous_fader_map[type].Pan != pan) {
                _previous_fader_map[type].Pan = pan;
                return true; // with value update.
            } else if (_previous_fader_map[type].Pan == pan) {
                return false; // no value update.
            }
            throw new ArgumentException("not Pan.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            string _type, _name;

            int _program_num, _player_program_num, _vol;

            Pan _pan;

            bool _mute = false;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            /// <note>
            /// associates with the Player object.
            /// </note>
            public string Type {
                get => _type; set => _type = value;
            }

            /// <note>
            /// associates with the Pattern object.
            /// </note>
            public string Name {
                get => _name; set => _name = value;
            }

            /// <summary>
            /// gets the program number set in mixer.json.
            /// </summary>
            public int ProgramNum {
                get => _program_num; set => _program_num = value;
            }

            /// <summary>
            /// gets the program number set in player.json.
            /// </summary>
            public int PlayerProgramNum {
                get => _player_program_num; set => _player_program_num = value;
            }

            public int Vol {
                get => _vol; set => _vol = value;
            }

            public Pan Pan {
                get => _pan; set => _pan = value;
            }

            public bool Mute {
                get => _mute; set => _mute = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private static Properties [noun, adjective] 

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
