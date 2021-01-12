
using System;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// Phrase クラスの Data クラス
    /// NOTE: Loader クラスから操作されるので公開必須
    /// </summary>
    public class Data {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Percussion[] percussionArray;

        string[] noteTextArray;

        int[] octArray;

        string[] preArray;

        string[] postArray;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        /// <summary>
        /// Percussion の音色設定
        /// </summary>
        public Percussion[] PercussionArray {
            get => percussionArray;
            set => percussionArray = value;
        }

        /// <summary>
        /// Note テキストの配列
        /// </summary>
        public string[] NoteTextArray {
            get => noteTextArray;
            set => noteTextArray = value;
        }

        /// <summary>
        /// Note テキストのオクターブ設定
        /// </summary>
        public int[] OctArray {
            get => octArray;
            set {
                checkValue();
                if (value == null) {
                    octArray = new int[noteTextArray.Length];
                    octArray.Select(x => x = 0); // オクターブの設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
                    throw new ArgumentException("noteOctArray must be same count as noteTextArray.");
                } else {
                    // TODO: バリデーション
                    octArray = value;
                }
            }
        }

        /// <summary>
        /// Note テキストの前方音価設定
        /// </summary>
        public string[] PreArray {
            get => preArray;
            set {
                checkValue();
                if (value == null) {
                    preArray = new string[noteTextArray.Length];
                    preArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
                    throw new ArgumentException("preArray must be same count as noteTextArray.");
                } else {
                    preArray = value; // TODO: バリデーション
                }
            }
        }

        /// <summary>
        /// Note テキストの後方音価設定
        /// </summary>
        public string[] PostArray {
            get => postArray;
            set {
                checkValue();
                if (value == null) {
                    postArray = new string[noteTextArray.Length];
                    postArray.Select(x => x = null); // 初期設定を自動生成
                } else if (value.Length != noteTextArray.Length) {
                    throw new ArgumentException("postArray must be same count as noteTextArray.");
                } else {
                    postArray = value; // TODO: バリデーション
                }
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        void checkValue() {
            if (noteTextArray == null) {
                throw new ArgumentException("must set noteTextArray.");
            }
        }
    }

    /// <summary>
    /// Phrase クラスの Range クラス
    /// </summary>
    public class Range {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Range(int min, int max) {
            Min = min;
            Max = max;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Min {
            get;
        }

        public int Max {
            get;
        }
    }

    /// <summary>
    /// Phrase クラスの Text クラス
    /// TODO: 引数クラスとして Range 等を含める？
    /// </summary>
    public class Text {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Text(string body, DataType type) {
            Body = body;
            Type = type;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public string Body {
            get;
        }

        public DataType Type {
            get;
        }
    }
}
