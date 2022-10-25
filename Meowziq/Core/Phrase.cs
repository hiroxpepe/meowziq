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
using System.Linq;

using Meowziq.Value;

namespace Meowziq.Core {
    /// <summary>
    /// phrase class
    /// </summary>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Phrase {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        string _type, _name, _base;

        Data _data;

        Item<Note> _note_item;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Phrase() {
            _data = new();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// provides the phrase type.
        /// </summary>
        public string Type {
            get => _type;
            set {
                _type = value;
                if (_type.Equals("seque")) {
                    _data.Seque.Use = true;
                }
            }
        }

        /// <summary>
        /// provides the phrase name.
        /// </summary>
        public string Name { get => _name; set => _name = value; }

        /// <summary>
        /// provides the Data object.
        /// </summary>
        public Data Data { get => _data; set => _data = value; }

        /// <summary>
        /// provides the name of the base phrase.
        /// </summary>
        public string Base { get => _base; set => _base = value; }

        /// <summary>
        /// sets the range.
        /// </summary>
        public string Range {
            set {
                if (value is null) { return; }
                string range_text = value;
                if (!range_text.Contains(":")) {
                    throw new ArgumentException("invalid range format.");
                }
                string[] range_array = range_text.Split(':');
                if (range_array.Length != 2) {
                    throw new ArgumentException("invalid range format.");
                }
                if (_data.HasChord) {
                    _data.Chord.Range = new(
                        min: int.Parse(range_array[0]),
                        max: int.Parse(range_array[1])
                    );
                } else if (_data.HasSeque) {
                    _data.Seque.Range = new(
                        min: int.Parse(range_array[0]),
                        max: int.Parse(range_array[1])
                    );
                }
            }
        }

        /// <summary>
        /// sets the Item<Note> object.
        /// </summary>
        public Item<Note> NoteItem { set => _note_item = value; }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        /// <summary>
        /// creates Note objects.
        /// </summary>
        public void Build(int tick, Pattern pattern) {
            onBuild(tick, pattern);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // protected Methods [verb]

        /// <summary>
        /// creates Note objects by executing the appropriate method of the Generator object.
        /// </summary>
        protected void onBuild(int tick, Pattern pattern) {
            Generator generator = Generator.GetInstance(note_item: _note_item);
            DataType data_type = defineDataType();
            switch (data_type) {
                case DataType.Mono:
                    {
                        Param param = new(note: _data.Note, exp: _data.Exp, type: data_type);
                        generator.ApplyNote(start_tick: tick, beat_count: pattern.BeatCount, span_list: pattern.AllSpan, param: param);
                    }
                    break;
                case DataType.Chord:
                    {
                        Param param = new(chord: _data.Chord, exp: _data.Exp, type: data_type);
                        generator.ApplyNote(start_tick: tick, beat_count: pattern.BeatCount, span_list: pattern.AllSpan, param: param);
                    }
                    break;
                case DataType.Multi:
                    string[] string_array = _data.Auto ? _data.AutoArray : _data.NoteArray;
                    for (int index = 0; index < string_array.Length; index++) {
                        Param param = new(
                            note: new Value.Note(string_array[index], _data.OctArray[index]),
                            exp: new Value.Exp(_data.PreArray[index], _data.PostArray[index]),
                            type: data_type,
                            auto_note: _data.Auto
                        );
                        generator.ApplyNote(start_tick: tick, beat_count: pattern.BeatCount, span_list: pattern.AllSpan, param: param);
                    }
                    break;
                case DataType.Drum:
                    _data.BeatArray.ToList().Select(selector: (x, idx) => new Param(
                        note: new Value.Note(text: x, oct: 0), // octave is always 0.
                        percussion_note_num: (int) _data.PercussionArray[idx],
                        exp: new Value.Exp(pre: _data.PreArray[idx], post: string.Empty),
                        type: data_type
                    )).ToList().ForEach(action: x => generator.ApplyDrumNote(start_tick: tick, beat_count: pattern.BeatCount, param: x));
                    break;
                case DataType.Seque:
                    {
                        Param param = new(seque: _data.Seque, type: data_type);
                        generator.ApplySequeNote(start_tick: tick, beat_count: pattern.BeatCount, span_list: pattern.AllSpan, param: param);
                    }
                    break;
            }
            // creates UI display information.
            generator.ApplyInfo(start_tick: tick, beat_count: pattern.BeatCount, span_list: pattern.AllSpan);
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // private Methods [verb]

        /// <summary>
        /// determines the type of data written in json.
        /// </summary>
        DataType defineDataType() {
            if (!_data.HasMulti && (_data.HasNote || _data.HasAuto)) { return DataType.Mono; }
            if (_data.HasMulti && (_data.HasNote || _data.HasAuto)) { return DataType.Multi; }
            if (_data.HasChord) { return DataType.Chord; }
            if (_data.HasBeat) { return DataType.Drum; }
            if (_data.HasSeque) { return DataType.Seque; }
            throw new ArgumentException("not understandable DataType.");
        }
    }
}
