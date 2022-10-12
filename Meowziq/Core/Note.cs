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
    /// ChannelMessage に変換される音情報保持クラス
    /// @author h.adachi
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _tick; // 4分音符 480の分解能のシーケンサーの tick 値 ※絶対値

        int _num; // MIDI ノート番号

        int _gate; // MIDI ノートON -> ノートOFF までの長さ

        int _velo; // MIDI ノート強さ

        int _pre_count; // シンコペーション設定

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: 作成したら状態は変更できません
        /// </summary>
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

        public int Tick {
            get => _tick; // NOTE: 変更操作を提供しません
        }

        public int Num {
            get => _num; // NOTE: 変更操作を提供しません
        }

        public int Gate {
            get => _gate; // NOTE: 変更操作を提供しません
        }

        public int Velo {
            get => _velo; // NOTE: 変更操作を提供しません
        }

        public bool HasPre {
            get => _pre_count > 0; // NOTE: 変更操作を提供しません
        }

        public int PreCount {
            get => _pre_count; // NOTE: 変更操作を提供しません
        }
    }
}
