
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

        static Map<string, Fader> _previousFaderMap; // key は "type" 形式とする

        static Map<string, Fader> _currentFaderMap; // key は "type:name" 形式とする

        static IMessage<T, Note> _message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Mixer() {
            _use = false;
            _previousFaderMap = new Map<string, Fader>();
            _currentFaderMap = new Map<string, Fader>();
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
                _currentFaderMap[$"{value.Type}:{value.Name}"] = value;
                _previousFaderMap[value.Type] = Fader.NoVaule(value.Type);
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
            _previousFaderMap.Clear();
            _currentFaderMap.Clear();
            _use = false;
        }

        /// <summary>
        /// Message に対して Note を適用します
        /// </summary>
        public static void ApplyNote(int tick, int midiCh, Note note) {
            _message.ApplyNote(tick, midiCh, note);
        }

        /// <summary>
        /// Message に対してプログラムチェンジ、ボリューム、Pan、その他の設定を適用します
        /// </summary>
        public static void ApplyVaule(int tick, int midiCh, string type, string name, int programNum) {
            if (!_use && !_currentFaderMap.ContainsKey($"{type}:default")) {
                _previousFaderMap[type] = Fader.NoVaule(type);
                _currentFaderMap[$"{type}:default"] = Fader.Default(type); // mixer.json なしの初回
            }
            if (_use && !_currentFaderMap.ContainsKey($"{type}:{name}")) {
                return; // mixer.json 使用時で存在しないキー
            }
            playerProgramNum = (programNum, type, name);
            applyValueBy(tick, midiCh, type, name);
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
                _currentFaderMap[$"{value.type}:{value.name}"].PlayerProgramNum = value.programNum;
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static void applyValueBy(int tick, int midiCh, string type, string name) {
            applyProgramChangeBy(tick, midiCh, type, name);
            applyVolumeBy(tick, midiCh, type, name);
            applyPanBy(tick, midiCh, type, name);
            applyMuteBy(tick, midiCh, type, name);
        }

        static void applyProgramChangeBy(int tick, int midiCh, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedProgramNum(type, name)) {
                var programNum = _use ? _currentFaderMap[$"{type}:{name}"].ProgramNum : _currentFaderMap[$"{type}:{name}"].PlayerProgramNum;
                _message.ApplyProgramChange(tick, midiCh, programNum);
            }
        }

        static void applyVolumeBy(int tick, int midiCh, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedVol(type, name)) {
                _message.ApplyVolume(tick, midiCh, _currentFaderMap[$"{type}:{name}"].Vol);
            }
        }

        static void applyPanBy(int tick, int midiCh, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            if (changedPan(type, name)) {
                _message.ApplyPan(tick, midiCh, _currentFaderMap[$"{type}:{name}"].Pan);
            }
        }

        static void applyMuteBy(int tick, int midiCh, string type, string name) {
            if (!_use) {
                name = "default"; // mixer.json なしは常に "default"
            }
            _message.ApplyMute(tick, midiCh, _currentFaderMap[$"{type}:{name}"].Mute);
        }

        static bool changedProgramNum(string type, string name) {
            var programNum = _use ? _currentFaderMap[$"{type}:{name}"].ProgramNum : _currentFaderMap[$"{type}:{name}"].PlayerProgramNum;
            if (_previousFaderMap[type].ProgramNum != programNum) {
                _previousFaderMap[type].ProgramNum = programNum;
                return true; // 値の更新あり
            } 
            else if (_previousFaderMap[type].ProgramNum == programNum) {
                return false; // 値の更新なし
            }
            throw new ArgumentException("not ProgramNum.");
        }

        static bool changedVol(string type, string name) {
            var vol = _currentFaderMap[$"{type}:{name}"].Vol;
            if (_previousFaderMap[type].Vol != vol) {
                _previousFaderMap[type].Vol = vol;
                return true; // 値の更新あり
            } else if (_previousFaderMap[type].Vol == vol) {
                return false; // 値の更新なし
            }
            throw new ArgumentException("not Vol.");
        }

        static bool changedPan(string type, string name) {
            var pan = _currentFaderMap[$"{type}:{name}"].Pan;
            if (_previousFaderMap[type].Pan != pan) {
                _previousFaderMap[type].Pan = pan;
                return true; // 値の更新あり
            } else if (_previousFaderMap[type].Pan == pan) {
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
