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

namespace Meowziq.Core {
    /// <summary>
    /// 音情報保持クラス
    /// </summary>
    /// <note>
    /// + ChannelMessage に変換されます
    /// </note>
    /// <author>h.adachi (STUDIO MeowToon)</author>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _tick, _num, _gate, _velo, _pre_count;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <note>
        /// + 作成したら状態は変更できません
        /// </note>
        public Note(int tick, int num, int gate, int velo, int pre_count = 0) {
            _tick = tick;
            _num = num;
            _gate = gate;
            _velo = velo;
            _pre_count = pre_count;
            Log.Trace($"tick: {tick}");
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjective] 

        /// <summary>
        /// 4分音符 480の分解能のシーケンサーの tick 値 ※絶対値
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Tick {
            get => _tick;
        }

        /// <summary>
        /// MIDI ノート番号
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Num {
            get => _num;
        }

        /// <summary>
        /// MIDI ノートON -> ノートOFF までの長さ
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Gate {
            get => _gate;
        }

        /// <summary>
        /// MIDI ノート強さ
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int Velo {
            get => _velo;
        }

        /// <summary>
        /// シンコペーション設定があるかどうか
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public bool HasPre {
            get => _pre_count > 0;
        }

        /// <summary>
        /// シンコペーション設定
        /// </summary>
        /// <note>
        /// + not provide modify operations.
        /// </note>
        public int PreCount {
            get => _pre_count;
        }
    }
}
