
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// PA クラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + TODO: 曲の演奏者について責任を持つ
    /// </summary>
    public static class Mixer<T> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool use;

        static List<Fader> previousFaderList;

        static List<Fader> currentFaderList;

        static IMessage<T, Note> message;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Mixer() {
            use = false;
            previousFaderList = new List<Fader>();
            currentFaderList = new List<Fader>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        public static bool Use {
            get => use;
            set => use = value;
        }

        /// <summary>
        /// NOTE: 演奏中に値が変更される可能性あり
        /// </summary>
        public static List<Fader> FaderList {
            get => currentFaderList;
            set {
                previousFaderList = currentFaderList;
                currentFaderList = value;
                if (!(value is null)) {
                    use = true;
                }
            }
        }

        public static IMessage<T, Note> Message {
            set => message = value;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        /// <summary>
        /// NOTE: 初回に1回だけ実行
        /// </summary>
        public static void Clear() {
            previousFaderList.Clear();
            currentFaderList.Clear();
            use = false;
            if (!(message is null)) {
                Enumerable.Range(0, 15).ToList().ForEach(x => { // TODO: ProgramNum は？
                    message.ApplyVolume(x, 0, 100);
                    message.ApplyPan(x, 0, Pan.Center);
                    message.ApplyMute(x, 0, false);
                });
            }
        }

        public static Fader GetBy(string type) {
            return currentFaderList.Where(x => x.Type.Equals(type)).First(); // TODO: ない時
        }

        public static void SetVolumeBy(int midiCh, int tick, string type) {
            if (!changedVol(type)) {
                return;
            }
            message.ApplyVolume(midiCh, tick, currentFaderList.Where(x => x.Type.Equals(type)).First().Vol); // TODO: 変化があれば
        }

        public static void Add(Fader fader) {
            currentFaderList.Add(fader);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private static Methods [verb]

        static bool changedVol(string type) {
            var _previous = previousFaderList.Where(x => x.Type.Equals(type)).First().Vol;
            var _current = currentFaderList.Where(x => x.Type.Equals(type)).First().Vol;
            return _previous == _current ? false : true;
        }
 
        static int getVolby(string type) { // TODO: LINQ を回すより Dict
            return currentFaderList.Where(x => x.Type.Equals(type)).First().Vol;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            int currentVol;

            int previousVol;

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
