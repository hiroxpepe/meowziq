
using System.Collections.Generic;

namespace Meowzic.Core {
    /// <summary>
    /// ソングとはキーとそのタイミングでの旋法を表現する
    ///     + バンドのコード譜のような概念
    /// メロディも知見である
    ///     + 多彩なフレーズを駆使するプレイヤーを育てていく仕組み
    /// </summary>
    public class Song {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key key; // キー ※ひとまず転調ではなく旋法の切り替えを実装する TODO: 転調キーを変えて再計算

        Mode mode; // キー全体の旋法

        List<Part> partList; // このキーでの度数がどれくらいの長さどの旋法で演奏されるか

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(Key key, Mode mode) {
            this.partList = new List<Part>(); // TODO: 順番付き？
            this.key = key;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Key Key {
            get => key;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Part part) {
            if (part.Mode == Mode.Undefined) {
                part.Mode = mode; // song の旋法を設定
            }
            partList.Add(part);
        }

        // TODO
        public Song Repeat(int count) {
            return this;
        }

        public List<Part> GetAllPart() {
            return partList;
        }
    }
}
