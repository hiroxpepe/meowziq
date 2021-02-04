
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanford.Multimedia.Midi;

using Meowziq.Midi;

namespace Meowziq.Core.Tests {
    [TestClass()]
    public class MixerTests {

        /// <summary>
        /// mixer.json を使用する場合
        /// </summary>
        [TestMethod()]
        public void ApplyVauleTest() {
            var _type = "base";
            var _fader1 = new Mixer<ChannelMessage>.Fader() {
                Type = _type,
                Name = "intro",
                // TODO: Inst
                Vol = 100,
                Pan = Pan.Enum.Parse("Left"),
                Mute = false
            };
            var _fader2 = new Mixer<ChannelMessage>.Fader() {
                Type = _type,
                Name = "verse1",
                // TODO: Inst
                Vol = 85,
                Pan = Pan.Enum.Parse("Center"),
                Mute = false
            };
            // 初期設定
            Mixer<ChannelMessage>.Message = MessageFactory.CreateMessage();
            Mixer<ChannelMessage>.Clear();
            Mixer<ChannelMessage>.AddFader = _fader1;
            Mixer<ChannelMessage>.AddFader = _fader2;

            // 初回変化あり
            var _tick1 = 0;
            Mixer<ChannelMessage>.ApplyVaule(_tick1, 1, _type, "intro", 25); // TODO: inst
            Message.Invert(); // 内部フラグ反転して取得
            var _result1 = Message.GetBy(_tick1);
            Assert.IsTrue(_result1.Count > 0);
            Message.Invert(); // 内部フラグを戻す

            // パターン変化ない場合データなし
            var _tick2 = 1920;
            Mixer<ChannelMessage>.ApplyVaule(1920, 1, _type, "intro", 25); // 1小節後
            Message.Invert(); // 内部フラグ反転して取得
            var _result2 = Message.GetBy(_tick2);
            Assert.IsTrue(_result2 is null);
            Message.Invert(); // 内部フラグを戻す

            // パターンが変化して設定値も変化したらデータあり
            var _tick3 = 7680;
            Mixer<ChannelMessage>.ApplyVaule(_tick3, 1, _type, "verse1", 36); // 4小節目でパターンが変わった
            Message.Invert(); // 内部フラグ反転して取得
            var _result3 = Message.GetBy(_tick3);
            Assert.IsTrue(_result3.Count > 0);
            Message.Invert(); // 内部フラグを戻す
        }
    }
}