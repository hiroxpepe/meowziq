
using System;
using System.Collections.Generic;

namespace Meowziq {
    /// <summary>
    /// Message のインタフェース
    /// </summary>
    public interface IMessage<T1, T2> {

        /// <summary>
        /// 引数の tick の T1 のリストを返します
        /// NOTE: 引数 tick の T1 のリストが存在しなければ null を返します
        /// </summary>
        List<T1> GetBy(int tick);

        /// <summary>
        /// 引数 tick のアイテムを持つかどうかを返します
        /// </summary>
        bool Has(int tick);

        /// <summary>
        /// 引数の tick を起点にして切り替え処理を行います
        /// </summary>
        void ApplyTick(int tick, Action<int> load);

        /// <summary>
        /// プログラムNo(音色)を T1 として適用します
        /// </summary>
        void ApplyProgramChange(int midiCh, int tick, int programNum);

        /// <summary>
        /// ボリュームを T1 として適用します
        /// </summary>
        void ApplyVolume(int midiCh, int tick, int volume);

        /// <summary>
        /// PAN (パン)を T1 として適用します
        /// </summary>
        void ApplyPan(int midiCh, int tick, Pan pan);

        /// <summary>
        /// ミュートを T1 として適用します
        /// </summary>
        void ApplyMute(int midiCh, int tick, bool mute);

        /// <summary>
        /// T2 を T1 として適用します
        /// </summary>
        void ApplyNote(int midiCh, T2 note);

        /// <summary>
        /// 状態を初期化します
        /// </summary>
        void Clear();

        /// <summary>
        /// 内部フラグを反転します
        /// </summary>
        void Invert();
    }
}
