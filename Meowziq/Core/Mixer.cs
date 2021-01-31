
using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// PAさんクラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + 曲の演奏者について責任を持つ
    /// + MIDI側にメッセージを渡すのをどうするか
    ///     + 必要なデータをラップするクラス
    ///     + こちらのタイミングでイベントを呼ぶ？
    ///         + 初期実装ではイニシャルの処理で良い
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

        public static Fader GetBy(string type) {
            return faderList.Where(x => x.Type.Equals(type)).First(); // TODO: ない時
        }

        public static void Add(Fader fader) {
            faderList.Add(fader);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        public class Fader {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Properties [noun, adjective]

            public string Type {
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
