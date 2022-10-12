
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// the name of Dictionary is too long so it names Map.
    /// </summary>
    /// <note>
    /// used in the Meowziq namespace.
    /// </note>
    public class Map<K, V> : Dictionary<K, V> {
    }

    /// <summary>
    /// the item class.
    /// </summary>
    /// <remarks>
    /// Map(Dictionary) class that can take out from the key of the value only once.
    /// </remarks>
    /// <note>
    /// used in the Meowziq namespace.
    /// </note>
    public class Item<T> : Map<int, List<T>> {

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
            /// <remarks>
            /// creates a new list if this does not have a key. <br/>
            /// adds the value to the list and adds as new to this.
            /// </remarks>
            if (!ContainsKey(key)) {
                List<T> new_list = new();
                new_list.Add(value);
                Add(key, new_list);
            }
            /// <remarks>
            /// gets the list of the key from this. <br/>
            /// adds the value to list.
            /// </remarks>
            else {
                var list = this[key];
                list.Add(value);
            }
        }

        /// <summary>
        /// returns the value of the key.
        /// </summary>
        public List<T> Get(int key) {
            /// <remarks>
            /// returns null if the key does not exist.
            /// </remarks>
            if (!ContainsKey(key)) {
                return null;
            }
            /// <remarks>
            /// returns the value of the key.
            /// </remarks>
            return this[key];
        }

        /// <summary>
        /// returns the value of the key only once.
        /// </summary>
        public List<T> GetOnce(int key) {
            /// <remarks>
            /// adds the key to hashset for whether the key was taken out. <br/>
            /// if the return value is false, already being taken out once, so return null.
            /// </remarks>
            if (!_take_out_hashset.Add(key)) {
                return null;
            }
            /// <remarks>
            /// returns null if the key does not exist.
            /// </remarks>
            if (!ContainsKey(key)) {
                return null;
            }
            /// <remarks>
            /// first time returns the value of the key.
            /// </remarks>
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
            /// <remarks>
            /// adds the added key to hashset.
            /// </remarks>
            _to_add_hashset.Add(key);
            base.Add(key, value);
        }

        /// <summary>
        /// returns true if the key exists, false otherwise.
        /// </summary>
        public new bool ContainsKey(int key) {
            /// <remarks>
            /// HashSet.Contains() is fast.
            /// </remarks>
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
