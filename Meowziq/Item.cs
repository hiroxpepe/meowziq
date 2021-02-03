
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// Dictionary の名前が長いので Map と名付ける
    /// NOTE: Meowziq 名前空間で使用される
    /// </summary>
    public class Map<K, V> : Dictionary<K, V> {
    }

    /// <summary>
    /// Item クラス
    /// NOTE: Add された Value を一度だけ取り出す Map(Dictionary)
    /// NOTE: Meowziq 名前空間で使用される
    /// </summary>
    public class Item<T> : Map<int, List<T>> {

        HashSet<int> toAddHashSet; // 追加した判定

        HashSet<int> takeOutHashSet; // 取り出した判定

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Item() {
            toAddHashSet = new HashSet<int>(); // 追加した判定
            takeOutHashSet = new HashSet<int>(); // 取り出した判定
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// key の List に value を追加します
        /// </summary>
        public void Add(int key, T value) {
            if (!ContainsKey(key)) { // key がなければ
                var _newList = new List<T>(); // List を新規作成
                _newList.Add(value); // List に value を追加
                Add(key, _newList); // Item に新規追加
            } else {
                var _list = this[key]; // Item から key の List を取得
                _list.Add(value); // List に value を追加
            }
        }

        /// <summary>
        /// key の Value を返します
        /// </summary>
        public List<T> Get(int key) {
            if (!ContainsKey(key)) {
                return null; // key に存在しない場合は null を返す
            }
            return this[key]; // 初回なら key の value を返す
        }

        /// <summary>
        /// 1回だけ key の Value を返します
        /// </summary>
        public List<T> GetOnce(int key) {
            if (!takeOutHashSet.Add(key)) { // 取り出したことを判定する hashSet で false なら
                return null; // 既に1回取り出したので null を返す
            }
            if (!ContainsKey(key)) {
                return null; // key に存在しない場合は null を返す
            }
            return this[key]; // 初回なら key の value を返す // TODO: ソート： ドラム優先、メロ後：ソートパラメータが必要？
        }

        /// <summary>
        /// key の value を置き換えます
        /// </summary>
        public void SetBy(int key, List<T> value) {
            this.Remove(key);
            this.Add(key, value);
        }

        /// <summary>
        /// key と value を追加します
        /// TODO: 重複 key は？
        /// </summary>
        public new void Add(int key, List<T> value) {
            toAddHashSet.Add(key); // key を追加したフラグを追加
            base.Add(key, value);
        }

        /// <summary>
        /// key があれば true、なければ false を返します
        /// </summary>
        public new bool ContainsKey(int key) {
            return toAddHashSet.Contains(key); // HashSet.Contains() は高速
        }

        /// <summary>
        /// 内容を初期化します
        /// </summary>
        public new void Clear() {
            toAddHashSet.Clear();
            takeOutHashSet.Clear();
            base.Clear();
        }
    }
}
