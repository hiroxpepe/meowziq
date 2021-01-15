
namespace Meowziq.Core {
    /// <summary>
    /// ChannelMessage に変換される音情報保持クラス
    /// </summary>
    public class Note {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        int tick; // 4分音符 480の分解能のシーケンサーの tick 値 ※絶対値

        int head; // この Note の1小節(4拍)の頭の tick 値 ※絶対値 

        int num; // MIDI ノート番号

        int gate; // MIDI ノートON -> ノートOFF までの長さ

        int velo; // MIDI ノート強さ

        bool stopPre; // 優先発音フラグ

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        /// <summary>
        /// NOTE: 作成したら状態は変更できません
        /// </summary>
        public Note(int tick, int num, int gate, int velo, bool stopPre = false) {
            this.tick = tick;
            this.num = num;
            this.gate = gate;
            this.velo = velo;
            this.stopPre = stopPre;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public int Tick {
            get => tick; // NOTE: 変更操作を提供しません
        }

        public int Num {
            get => num; // NOTE: 変更操作を提供しません
        }

        public int Gate {
            get => gate; // NOTE: 変更操作を提供しません
        }

        public int Velo {
            get => velo; // NOTE: 変更操作を提供しません
        }

        public bool StopPre {
            get => stopPre; // NOTE: 変更操作を提供しません
        }
    }
}
