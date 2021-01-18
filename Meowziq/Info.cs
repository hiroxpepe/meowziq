
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// 曲がどのような状況で演奏されているかを表す情報を保持するクラス
    /// NOTE: 設定されて読み出させるだけ、これを状態を変更する目的で使わない
    /// </summary>
    public static class Info {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static　Constructor

        static Info() {
            HashSet = new HashSet<int>(); // ※Dictionary.ContainsKey() が遅いのでその対策
            ItemDictionary = new Dictionary<int, Item>();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Properties [noun, adjective] 

        public static int Beat {
            get; set;
        }

        public static int Meas {
            get; set;
        }

        public static HashSet<int> HashSet {
            get; set;
        }

        public static Dictionary<int, Item> ItemDictionary {
            get; set;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static void Clear() {
            Beat = 0;
            Meas = 0;
            HashSet.Clear();
            ItemDictionary.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // inner Classes

        /// <summary>
        /// どのようなキー、度数、旋法で演奏されているかを表す情報
        /// </summary>
        public class Item {

            ///////////////////////////////////////////////////////////////////////////////////////////
            // Fields

            string spanMode;

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
