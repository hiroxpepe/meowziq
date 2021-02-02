
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// PA クラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + TODO: 曲の演奏者について責任を持つ
    ///     + TODO: 演奏中にパンやボリュームが指定できるように
    ///     + MEMO: ここを唯一の IMessage へのインタフェースにしてはどうか？
    /// </summary>
    public static class Mixer<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool use;

        static Dictionary<string, Fader> previousFaderDictionary;

        static Dictionary<string, Fader> currentFaderDictionary;

        static IMessage<T, Note> message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Mixer() {
            use = false;
            previousFaderDictionary = new Dictionary<string, Fader>();
            currentFaderDictionary = new Dictionary<string, Fader>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static bool Use {
            get => use;
            set => use = value;
        }

        /// <summary>
        /// NOTE: 演奏中に値が変更される可能性あり
        /// FIXME: SMF 出力でバグあり
        /// </summary>
        public static Fader AddFader {
            set {
                if (value is null) {
                    return;
                }
                if (!previousFaderDictionary.ContainsKey(value.Type) && !currentFaderDictionary.ContainsKey(value.Type)) { // どちらもなければ初回設定
                    previousFaderDictionary[value.Type] = new Fader() {
                        Type = value.Type,
                        Vol = -1,
                        Pan = Pan.Enum,
                        Mute = false
                    };
                    currentFaderDictionary[value.Type] = value;
                } else {
                    previousFaderDictionary[value.Type] = currentFaderDictionary[value.Type];
                    currentFaderDictionary[value.Type] = value;
                }
                use = true;
            }
        }

        public static (int programNum, string type) ProgramNum {
            set {
                if (!previousFaderDictionary.ContainsKey(value.type) && !currentFaderDictionary.ContainsKey(value.type)) { // どちらもなければ初回設定
                    previousFaderDictionary[value.type].ProgramNum = -1;
                    currentFaderDictionary[value.type].ProgramNum = value.programNum;
                } else {
                    previousFaderDictionary[value.type].ProgramNum = currentFaderDictionary[value.type].ProgramNum;
                    currentFaderDictionary[value.type].ProgramNum = value.programNum;
                }
            }
        }

        /// <summary>
        /// Message オブジェクトを設定します
        /// </summary>
        public static IMessage<T, Note> Message {
            set => message = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// NOTE: 初回に1回だけ実行
        /// </summary>
        public static void Clear() {
            previousFaderDictionary.Clear();
            currentFaderDictionary.Clear();
            use = false;
            if (!(message is null)) {
                Enumerable.Range(0, 15).ToList().ForEach(x => { // TODO: ProgramNum は？
                    message.ApplyVolume(x, 0, 100);
                    message.ApplyPan(x, 0, Pan.Center);
                    message.ApplyMute(x, 0, false);
                });
            }
        }

        /// <summary>
        /// MEMO: 外部からはこれだけ呼ぶ
        /// </summary>
        public static void Apply(int midiCh, int tick, string type) {
            applyProgramChangeBy(midiCh, tick, type);
            applyVolumeBy(midiCh, tick, type);
            applyPanBy(midiCh, tick, type);
            applyMuteBy(midiCh, tick, type);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static void applyProgramChangeBy(int midiCh, int tick, string type) {
            if (use && changedProgramNum(type)) {
                message.ApplyProgramChange(midiCh, tick, currentFaderDictionary[type].ProgramNum);
            }
        }

        static void applyVolumeBy(int midiCh, int tick, string type) {
            if (use && changedVol(type)) {
                message.ApplyVolume(midiCh, tick, currentFaderDictionary[type].Vol);
            }
        }

        static void applyPanBy(int midiCh, int tick, string type) {
            if (use && changedPan(type)) {
                message.ApplyPan(midiCh, tick, currentFaderDictionary[type].Pan);
            }
        }

        static void applyMuteBy(int midiCh, int tick, string type) {
            if (use) {
                message.ApplyMute(midiCh, tick, currentFaderDictionary[type].Mute);
            }
        }

        static bool changedProgramNum(string type) {
            return previousFaderDictionary[type].ProgramNum == currentFaderDictionary[type].ProgramNum ? false : true;
        }

        static bool changedVol(string type) {
            return previousFaderDictionary[type].Vol == currentFaderDictionary[type].Vol ? false : true;
        }

        static bool changedPan(string type) {
            return previousFaderDictionary[type].Pan == currentFaderDictionary[type].Pan ? false : true;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            public string Type {
                get; set;
            }

            public int ProgramNum {
                get; set;
            }

            public int Vol {
                get; set;
            }

            public Pan Pan {
                get; set;
            }

            public bool Mute {
                get; set;
            }
        }
    }
}
