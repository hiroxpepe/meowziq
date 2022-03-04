
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// SMF 出力用 Track オブジェクトを保持するクラス
    /// </summary>
    public static class Multi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Map<int, Track> _trackMap;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Multi() {
            _trackMap = new Map<int, Track>();
            Enumerable.Range(0, 16).ToList().ForEach(x => _trackMap.Add(x, new Track()));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective]

        public static List<Track> List {
            get => _trackMap.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static Track Get(int index) {
            return _trackMap[index];
        }
    }
}
