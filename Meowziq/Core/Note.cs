
namespace Meowziq.Core {
    /// <summary>
    /// ChannelMessage に変換される音情報保持クラス
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int _tick; // 4分音符 480の分解能のシーケンサーの tick 値 ※絶対値

        int _num; // MIDI ノート番号

        int _gate; // MIDI ノートON -> ノートOFF までの長さ

        int _velo; // MIDI ノート強さ

        int _preCount; // シンコペーション設定

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: 作成したら状態は変更できません
        /// </summary>
        public Note(int tick, int num, int gate, int velo, int preCount = 0) {
            _tick = tick;
            _num = num;
            _gate = gate;
            _velo = velo;
            _preCount = preCount;
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
            get => _preCount > 0; // NOTE: 変更操作を提供しません
        }

        public int PreCount {
            get => _preCount; // NOTE: 変更操作を提供しません
        }
    }
}
