
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// PA クラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + TODO: 曲の演奏者について責任を持つ
    /// </summary>
    public static class Mixer {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static bool use;

        static List<Fader> faderList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Mixer() {
            use = false;
            faderList = new List<Fader>();
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
            get => faderList;
            set {
                faderList = value;
                if (!(value is null)) {
                    use = true;
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// NOTE: 初回に1回だけ実行
        /// </summary>
        public static void Clear() {
            faderList.Clear();
            use = false;
            Enumerable.Range(0, 15).ToList().ForEach( x => {
                Message.ApplyVolume(x, 0, 100);
                Message.ApplyPan(x, 0, Pan.Center);
                Message.ApplyMute(x, 0, false);
            });
        }

        public static Fader GetBy(string type) {
            return faderList.Where(x => x.Type.Equals(type)).First(); // TODO: ない時
        }

        public static void Add(Fader fader) {
            faderList.Add(fader);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////////
            // Fields

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
