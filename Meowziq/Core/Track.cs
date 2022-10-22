/*
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 2 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Linq;

namespace Meowziq.Core {
    /// <summary>
    /// track class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Track {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Map<string, Item<Note>> _note_item_map = new();

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _type;

        Item<Note> _note_item;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Track(string type) {
            _type = type;
            _note_item = new();
            if (_note_item_map.ContainsKey(key: type)) {
                _note_item_map.Remove(key: type);
            }
            _note_item_map.Add(key: type, value: _note_item);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective] 

        /// <summary>
        /// gets the all Note objects of this song.
        /// </summary>
        /// <note>
        /// not used yet.
        /// </note>
        public static List<Note> AllNote {
            get => _note_item_map.SelectMany(selector: x => x.Value).SelectMany(selector: x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// gets the Item<Note> object.
        /// </summary>
        public Item<Note> NoteItem {
            get {
               optimize();
               return _note_item; 
            }
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// initializes the contents.
        /// </summary>
        /// <note>
        /// not used yet.
        /// </note>
        public void Clear() {
            foreach (Item<Note> value in _note_item_map.Values) { value.Clear(); }
            _note_item_map.Clear();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// TBA
        /// </summary>
        /// <memo_jp>
        /// + previous の発音が続いてる Note を識別する？ => どのように？
        ///     + previous.AllNote.Were(なんとか) 
        /// + current の シンコペ Note <= 判定済み
        /// </memo_jp>
        void optimize() {
            List<Note> note_list = _note_item.SelectMany(x => x.Value).ToList();
            foreach (Note stop_note in note_list.Where(x => x.HasPre)) { // 優先ノートのリスト
                foreach (Note note in note_list) { // このフレーズの全てのノートの中で
                    if (note.Tick == stop_note.Tick) { // 優先ノートと発音タイミングがかぶったら
                        removeBy(note); // ノートを削除
                    }
                }
            }
        }

        /// <summary>
        /// シンコペーションで被る Note を除外します
        /// NOTE: all sound off された後の tick の note を消せばよい
        /// TODO: この処理の高速化が必要：List が遅い？ LINQ が遅い？
        /// FIXME: バグあり ⇒ シンコペーションは小節の頭だけ許可する？
        /// </summary>
        public void removeBy(Note target) {
            int tick1 = target.Tick;
            List<Note> note_list1 = _note_item.Get(key: tick1);
            note_list1 = note_list1.Where(predicate: x => !(!x.HasPre && x.Tick == tick1)).ToList(); // 優先ノートではなく tick が同じものを削除 // FIXME: ドラムは音毎？
            _note_item.SetBy(key: tick1, value: note_list1);
            if (target.PreCount > 1) { // さらにシンコぺの設定値が2の場合
                int tick2 = target.Tick + Length.Of16beat.Int32();
                List<Note> note_list2 = _note_item.Get(tick2);
                if (note_list2 != null) {
                    note_list2 = note_list2.Where(x => !(!x.HasPre && x.Tick == tick2)).ToList(); // さらに被る16音符を削除
                    _note_item.SetBy(key: tick2, value: note_list2);
                }
            }
        }
    }
}
