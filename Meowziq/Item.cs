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
    /// A shorter alias for Dictionary, used as Map in the Meowziq namespace.
    /// </summary>
    /// <remarks>
    /// Used throughout the Meowziq namespace as a convenient replacement for Dictionary.
    /// </remarks>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Map<K, V> : Dictionary<K, V> {
    }

    /// <summary>
    /// Provides event data for change notifications.
    /// </summary>
    public class EvtArgs : EventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="EvtArgs"/> class with the specified name.
        /// </summary>
        /// <param name="name">The name associated with the event.</param>
        public EvtArgs(string name) {
            Name = name;
        }
        /// <summary>
        /// Gets the name associated with the event.
        /// </summary>
        public string Name { get; }
        public string? Value { get; set; }
    }

    /// <summary>
    /// Represents a delegate for change event handlers.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="e">The event data.</param>
    public delegate void Changed(object sender, EvtArgs e);

    /// <summary>
    /// Represents a map that allows retrieving values by key only once, with change tracking.
    /// </summary>
    /// <remarks>
    /// A Map (Dictionary) class that allows retrieving the value for a key only once.<br/>
    /// Used in the Meowziq namespace.
    /// </remarks>
    public class Item<T> : Map<int, List<T>> {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        /// <summary>
        /// Tracks which keys have been added.
        /// </summary>
        HashSet<int> _to_add_hashset;

        /// <summary>
        /// Tracks which keys have been taken out.
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
        /// Adds a value to the list for the specified key. Creates a new list if the key does not exist.
        /// </summary>
        /// <param name="key">The key to add the value to.</param>
        /// <param name="value">The value to add.</param>
        public void Add(int key, T value) {
            // Creates a new list if the key does not exist, then adds the value.
            if (!ContainsKey(key)) {
                List<T> new_list = new();
                new_list.Add(value);
                Add(key, new_list);
            }
            // Adds the value to the existing list for the key.
            else {
                List<T> list = this[key];
                list.Add(value);
            }
        }

        /// <summary>
        /// Gets the list of values for the specified key.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The list of values for the key, or <c>null</c> if the key does not exist.</returns>
        public List<T> Get(int key) {
            if (!ContainsKey(key)) {
                return null;
            }
            return this[key];
        }

        /// <summary>
        /// Gets the list of values for the specified key only once. Returns null on subsequent calls for the same key.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <returns>The list of values for the key, or <c>null</c> if already taken or the key does not exist.</returns>
        public List<T> GetOnce(int key) {
            // Adds the key to the taken-out hash set. If already present, return null.
            if (!_take_out_hashset.Add(key)) {
                return null;
            }
            if (!ContainsKey(key)) {
                return null;
            }
            // Returns the value for the key the first time.
            return this[key];
        }

        /// <summary>
        /// Replaces the value list for the specified key.
        /// </summary>
        /// <param name="key">The key to replace.</param>
        /// <param name="value">The new list of values.</param>
        public void SetBy(int key, List<T> value) {
            Remove(key);
            Add(key, value);
        }

        /// <summary>
        /// Adds a key and value list to the map, tracking the added key.
        /// </summary>
        /// <param name="key">The key to add.</param>
        /// <param name="value">The list of values to associate with the key.</param>
        /// <remarks>
        /// Adds the key to the internal hash set for fast existence checks.
        /// </remarks>
        public new void Add(int key, List<T> value) {
            _to_add_hashset.Add(key);
            base.Add(key, value);
        }

        /// <summary>
        /// Determines whether the specified key exists in the map.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
        /// <remarks>
        /// Uses <see cref="HashSet{T}.Contains"/> for fast lookup.
        /// </remarks>
        public new bool ContainsKey(int key) {
            return _to_add_hashset.Contains(key);
        }

        /// <summary>
        /// Clears all contents and resets the internal state.
        /// </summary>
        public new void Clear() {
            _to_add_hashset.Clear();
            _take_out_hashset.Clear();
            base.Clear();
        }
    }
}
