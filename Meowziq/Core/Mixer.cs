
using System;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// PA クラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + TODO: 曲の演奏者について責任を持つ
    ///     + TODO: 演奏中にパンやボリュームが指定できるように
    ///     + MEMO: ここを唯一の IMessage へのインタフェースにしてはどうか？
    ///     + MEMO: そもそもこのアプリでボリュームやパンを詳細に設定出来る必要があるのか？
    ///     + MEMO: Mixer に設定がある場合は楽器は Mixer の設定を使用する
    ///         + ただし Mixer 経由で値を Message に適用した方が好ましい
    /// </summary>
    public static class Mixer<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool _use;

        static Map<string, Fader> _previous_fader_map; // key は "type" 形式とする

        static Map<string, Fader> _current_fader_map; // key は "type:name" 形式とする

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
        /// NOTE: 演奏中に値が変更される可能性あり
        /// FIXME: SMF 出力でバグあり
        /// </summary>
        public static Fader AddFader {
            set {
                if (value is null) {
                    return;
                }
                _current_fader_map[$"{value.Type}:{value.Name}"] = value;
                _previous_fader_map[value.Type] = Fader.NoVaule(value.Type);
                _use = true; // Fader を追加された、つまり mixier.json が存在する
            }
        }

        /// <summary>
        /// Message オブジェクトを設定します
        /// </summary>
        public static IMessage<T, Note> Message {
            set => _message = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// 状態を初期化します
        /// NOTE: 初回に1回だけ実行
        /// </summary>
        public static void Clear() {
            _previous_fader_map.Clear();
            _current_fader_map.Clear();
            _use = false;
        }

        /// <summary>
        /// Message に対して Note を適用します
        /// </summary>
        public static void ApplyNote(int tick, int midi_ch, Note note) {
            _message.ApplyNote(tick, midi_ch, note);
        }

        /// <summary>
        /// Message に対してプログラムチェンジ、ボリューム、Pan、その他の設定を適用します
        /// </summary>
        public static void ApplyVaule(int tick, int midi_ch, string type, string name, int program_num) {
            if (!_use && !_current_fader_map.ContainsKey($"{type}:default")) {
                _previous_fader_map[type] = Fader.NoVaule(type);
                _current_fader_map[$"{type}:default"] = Fader.Default(type); // mixer.json なしの初回
            }
            if (_use && !_current_fader_map.ContainsKey($"{type}:{name}")) {
                return; // mixer.json 使用時で存在しないキー
            }
            playerProgramNum = (program_num, type, name);
            applyValueBy(tick, midi_ch, type, name);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Properties [noun, adjective] 

        /// <summary>
        /// player.json に記述された 音色を設定します
        /// </summary>
        static (int programNum, string type, string name) playerProgramNum {
            set {
                if (!_use) {
                    value.name = "default"; // mixer.json なしは常に "default"
                }
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
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedProgramNum(type, name)) {
                var programNum = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
                _message.ApplyProgramChange(tick, midi_ch, programNum);
            }
        }

        static void applyVolumeBy(int tick, int midi_ch, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedVol(type, name)) {
                _message.ApplyVolume(tick, midi_ch, _current_fader_map[$"{type}:{name}"].Vol);
            }
        }

        static void applyPanBy(int tick, int midi_ch, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedPan(type, name)) {
                _message.ApplyPan(tick, midi_ch, _current_fader_map[$"{type}:{name}"].Pan);
            }
        }

        static void applyMuteBy(int tick, int midi_ch, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            _message.ApplyMute(tick, midi_ch, _current_fader_map[$"{type}:{name}"].Mute);
        }

        static bool changedProgramNum(string type, string name) {
            var program_num = _use ? _current_fader_map[$"{type}:{name}"].ProgramNum : _current_fader_map[$"{type}:{name}"].PlayerProgramNum;
            if (_previous_fader_map[type].ProgramNum != program_num) {
                _previous_fader_map[type].ProgramNum = program_num;
                return true; // 値の更新あり
            } 
            else if (_previous_fader_map[type].ProgramNum == program_num) {
                return false; // 値の更新なし
            }
            throw new ArgumentException("not ProgramNum.");
        }

        static bool changedVol(string type, string name) {
            var vol = _current_fader_map[$"{type}:{name}"].Vol;
            if (_previous_fader_map[type].Vol != vol) {
                _previous_fader_map[type].Vol = vol;
                return true; // 値の更新あり
            } else if (_previous_fader_map[type].Vol == vol) {
                return false; // 値の更新なし
            }
            throw new ArgumentException("not Vol.");
        }

        static bool changedPan(string type, string name) {
            var pan = _current_fader_map[$"{type}:{name}"].Pan;
            if (_previous_fader_map[type].Pan != pan) {
                _previous_fader_map[type].Pan = pan;
                return true; // 値の更新あり
            } else if (_previous_fader_map[type].Pan == pan) {
                return false; // 値の更新なし
            }
            throw new ArgumentException("not Pan.");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Fields

            bool _mute = false;

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            /// <summary>
            /// NOTE: Player と紐づけ
            /// </summary>
            public string Type {
                get; set;
            }

            /// <summary>
            /// NOTE: Pattern と紐づけ
            /// </summary>
            public string Name {
                get; set;
            }

            /// <summary>
            /// NOTE: mixer.json に設定された値
            /// </summary>
            public int ProgramNum {
                get; set;
            }

            /// <summary>
            /// NOTE: player.json に設定された値
            /// </summary>
            public int PlayerProgramNum {
                get; set;
            }

            public int Vol {
                get; set;
            }

            public Pan Pan {
                get; set;
            }

            public bool Mute {
                get => _mute;
                set => _mute = value;
            }

            ///////////////////////////////////////////////////////////////////////////////////////////
            // private static Properties [noun, adjective] 

            public static Fader NoVaule(string type) {
                return new Fader() {
                    Type = type,
                    Name = "undefined",
                    ProgramNum = -1,
                    PlayerProgramNum = -1,
                    Vol = -1,
                    Pan = Pan.Undefined,
                    Mute = false
                };
            }

            public static Fader Default(string type) {
                return new Fader() {
                    Type = type,
                    Name = "default",
                    ProgramNum = -1,
                    PlayerProgramNum = -1,
                    Vol = 100,
                    Pan = Pan.Center,
                    Mute = false
                };
            }
        }
    }
}
