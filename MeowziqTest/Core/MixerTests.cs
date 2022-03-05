
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

using Sanford.Multimedia.Midi;
using Meowziq;
using Meowziq.Core;
using Meowziq.Midi;

namespace MeowziqTest.Core{
    [TestClass()]
    public class MixerTests {

        /// <summary>
        /// mixer.json を使用する場合
        /// </summary>
        [TestMethod()]
        public void ApplyVauleTest() {
            var type = "base";
            var fader1 = new Mixer<ChannelMessage>.Fader() {
                Type = type,
                Name = "intro",
                // TODO: Inst
                Vol = 100,
                Pan = Pan.Enum.Parse("Left"),
                Mute = false
            };
            var fader2 = new Mixer<ChannelMessage>.Fader() {
                Type = type,
                Name = "verse1",
                // TODO: Inst
                Vol = 85,
                Pan = Pan.Enum.Parse("Center"),
                Mute = false
            };
            // 初期設定
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            Mixer<ChannelMessage>.Clear();
            Mixer<ChannelMessage>.AddFader = fader1;
            Mixer<ChannelMessage>.AddFader = fader2;

            // 初回変化あり
            var tick1 = 0;
            Mixer<ChannelMessage>.ApplyVaule(tick1, 1, type, "intro", 25); // TODO: inst
            Message.Invert(); // 内部フラグ反転して取得
            var result1 = Message.GetBy(tick1);
            IsTrue(result1.Count > 0);
            Message.Invert(); // 内部フラグを戻す

            // パターン変化ない場合データなし
            var tick2 = 1920;
            Mixer<ChannelMessage>.ApplyVaule(1920, 1, type, "intro", 25); // 1小節後
            Message.Invert(); // 内部フラグ反転して取得
            var result2 = Message.GetBy(tick2);
            IsTrue(result2 is null);
            Message.Invert(); // 内部フラグを戻す

            // パターンが変化して設定値も変化したらデータあり
            var tick3 = 7680;
            Mixer<ChannelMessage>.ApplyVaule(tick3, 1, type, "verse1", 36); // 4小節目でパターンが変わった
            Message.Invert(); // 内部フラグ反転して取得
            var result3 = Message.GetBy(tick3);
            IsTrue(result3.Count > 0);
            Message.Invert(); // 内部フラグを戻す
        }
    }
}
