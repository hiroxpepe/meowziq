
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// SMF 出力用 Track オブジェクト達を保持するクラス
    /// </summary>
    public static class Multi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Dictionary<int, Track> trackDict;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Multi() {
            trackDict = new Dictionary<int, Track>();
            Enumerable.Range(0, 15).ToList().ForEach(x => trackDict.Add(x, new Track()));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective]

        public static List<Track> List {
            get => trackDict.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static Track Get(int index) {
            return trackDict[index];
        }
    }
}
