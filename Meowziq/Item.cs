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

using System;
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// name of Dictionary is too long, it be named Map.
    /// </summary>
    /// <note>
    /// + used in the Meowziq namespace. <br/>
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Map<K, V> : Dictionary<K, V> {
    }

    /// <summary>
    /// changed event args.
    /// </summary>
    public class EvtArgs : EventArgs {
        public EvtArgs(string name) {
            Name = name;
        }
        public string Name { get; }
        public string? Value { get; set; }
    }

    /// <summary>
    /// changed event handler.
    /// </summary>
    public delegate void Changed(object sender, EvtArgs e);

    /// <summary>
    /// item class.
    /// </summary>
    /// <note>
    /// + Map(Dictionary) class that can take out from the key of the value only once. <br/>
    /// + used in the Meowziq namespace. <br/>
    /// </note>
    public class Item<T> : Map<int, List<T>> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// hasset for whether the key was added.
        /// </summary>
        HashSet<int> _to_add_hashset;

        /// <summary>
        /// hasset for whether the key was taken out.
        /// </summary>
        HashSet<int> _take_out_hashset;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Item() {
            _to_add_hashset = new();
            _take_out_hashset = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// adds the value of the key to list.
        /// </summary>
        public void Add(int key, T value) {
            /// <note>
            /// creates a new list if this does not have a key. <br/>
            /// adds the value to the list and adds as new to this.
            /// </note>
            if (!ContainsKey(key)) {
                List<T> new_list = new();
                new_list.Add(value);
                Add(key, new_list);
            }
            /// <note>
            /// gets the list of the key from this. <br/>
            /// adds the value to list.
            /// </note>
            else {
                List<T> list = this[key];
                list.Add(value);
            }
        }

        /// <summary>
        /// returns the value of the key.
        /// </summary>
        public List<T> Get(int key) {
            /// <note>
            /// returns null if the key does not exist.
            /// </note>
            if (!ContainsKey(key)) {
                return null;
            }
            /// <note>
            /// returns the value of the key.
            /// </note>
            return this[key];
        }

        /// <summary>
        /// returns the value of the key only once.
        /// </summary>
        public List<T> GetOnce(int key) {
            /// <note>
            /// adds the key to hashset for whether the key was taken out. <br/>
            /// if the return value is false, already being taken out once, so return null.
            /// </note>
            if (!_take_out_hashset.Add(key)) {
                return null;
            }
            /// <note>
            /// returns null if the key does not exist.
            /// </note>
            if (!ContainsKey(key)) {
                return null;
            }
            /// <note>
            /// first time returns the value of the key.
            /// </note>
            /// <todo>
            /// needs a sorting parameter, such as drums first and melody later?
            /// </todo>
            return this[key];
        }

        /// <summary>
        /// replaces the value in key.
        /// </summary>
        public void SetBy(int key, List<T> value) {
            Remove(key);
            Add(key, value);
        }

        /// <summary>
        /// adds key and value.
        /// </summary>
        /// <todo>
        /// what about duplicate keys?
        /// </todo>
        public new void Add(int key, List<T> value) {
            /// <note>
            /// adds the added key to hashset.
            /// </note>
            _to_add_hashset.Add(key);
            base.Add(key, value);
        }

        /// <summary>
        /// returns true if the key exists, false otherwise.
        /// </summary>
        public new bool ContainsKey(int key) {
            /// <note>
            /// HashSet.Contains() is fast.
            /// </note>
            return _to_add_hashset.Contains(key);
        }

        /// <summary>
        /// initializes the contents.
        /// </summary>
        public new void Clear() {
            _to_add_hashset.Clear();
            _take_out_hashset.Clear();
            base.Clear();
        }
    }
}
