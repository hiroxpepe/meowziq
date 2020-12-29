
using System.Collections.Generic;

namespace Meowzic.Core {
    /// <summary>
    /// PAさんクラス
    ///     + ボリューム調整、パン、ミュートとか
    ///     + 全体のエフェクトとか
    ///     + 曲の演奏者について責任を持つ
    /// + MIDI側にメッセージを渡すのをどうするか
    ///     + 必要なデータをラップするクラス
    ///     + こちらのタイミングでイベントを呼ぶ？
    ///         + 初期実装ではイニシャルの処理で良い
    /// </summary>
    public class Mixer {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        protected List<Player> playerList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void Add(Player player) {
            playerList.Add(player);
        }
    }
}
