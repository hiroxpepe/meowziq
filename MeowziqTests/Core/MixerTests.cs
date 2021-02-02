
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sanford.Multimedia.Midi;

namespace Meowziq.Core.Tests {
    [TestClass()]
    public class MixerTests {

        [TestMethod()]
        public void AddFaderTest() {
            var _type = "base";
            // 初回設定なら変化有り
            Mixer<ChannelMessage>.AddFader = new Mixer<ChannelMessage>.Fader(){
                Type = _type,
                Vol = 100,
                Pan = Pan.Enum.Parse("Left"),
                Mute = false
            };
            var _result1 = new PrivateType(typeof(Mixer<ChannelMessage>)).InvokeStatic("changedVol", _type);
            Assert.AreEqual(_result1, true);

            // 同じ値を設定すると変化なし
            Mixer<ChannelMessage>.AddFader = new Mixer<ChannelMessage>.Fader() {
                Type = _type,
                Vol = 100,
                Pan = Pan.Enum.Parse("Left"),
                Mute = false
            };
            var _result2 = new PrivateType(typeof(Mixer<ChannelMessage>)).InvokeStatic("changedVol", _type);
            Assert.AreEqual(_result2, false);

            // 違う値を設定すると変化あり
            Mixer<ChannelMessage>.AddFader = new Mixer<ChannelMessage>.Fader() {
                Type = _type,
                Vol = 80,
                Pan = Pan.Enum.Parse("Left"),
                Mute = false
            };
            var _result3 = new PrivateType(typeof(Mixer<ChannelMessage>)).InvokeStatic("changedVol", _type);
            Assert.AreEqual(_result3, true);
        }
    }
}