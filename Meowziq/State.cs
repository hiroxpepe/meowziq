
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// 曲がどのような状況で演奏されているかを表す情報を保持するクラス
    /// NOTE: 設定されて読み出させるだけ、これを状態を変更する目的で使わない
    /// </summary>
    public static class State {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static　Constructor

        static State() {
            HashSet = new HashSet<int>(); // ※Dictionary.ContainsKey() が遅いのでその対策
            Dictionary = new Dictionary<int, Item16beat>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Properties [noun, adjective] 

        public static int Tempo {
            get; set;
        }

        public static int Beat {
            get; set;
        }

        public static int Meas {
            get; set;
        }

        public static HashSet<int> HashSet {
            get; set;
        }

        public static Dictionary<int, Item16beat> Dictionary {
            get; set; // TODO: 他の Dictionary が必要になったら？
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Clear() {
            Beat = 0;
            Meas = 0;
            HashSet.Clear();
            Dictionary.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// どのようなキー、度数、旋法で演奏されているかを表す情報
        /// NOTE: 16beat 毎に作成される
        /// </summary>
        public class Item16beat {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            string spanMode; // TODO: Mode 型に変更

            ///////////////////////////////////////////////////////////////////////////////////////////
            // static Properties [noun, adjective] 

            public int Tick {
                get; set;
            }

            public string Key {
                get; set;
            }

            public string Degree {
                get; set;
            }

            public string KeyMode {
                get; set;
            }

            public string SpanMode {
                get {
                    if (spanMode.Equals("Undefined")) {
                        return KeyMode;
                    }
                    return spanMode;
                } 
                set => spanMode = value;
            }

            public bool AutoMode {
                get {
                    if (spanMode.Equals("Undefined")) {
                        return true;
                    }
                    return false;
                }
            }
        }
    }
}
