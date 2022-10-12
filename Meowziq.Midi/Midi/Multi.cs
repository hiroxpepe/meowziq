
using System.Collections.Generic;
using System.Linq;
using Sanford.Multimedia.Midi;

namespace Meowziq.Midi {
    /// <summary>
    /// class that holds Sanford.Multimedia.Midi.Track objects for SMF output.
    /// </summary>
    public static class Multi {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Fields

        static Map<int, Track> _track_map;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Constructor

        static Multi() {
            _track_map = new();
            Enumerable.Range(0, 16).ToList().ForEach(x => _track_map.Add(x, new Track()));
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // static Properties [noun, adjective]

        public static List<Track> List {
            get => _track_map.Select(x => x.Value).ToList();
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public static Methods [verb]

        public static Track Get(int index) {
            return _track_map[index];
        }
    }
}
