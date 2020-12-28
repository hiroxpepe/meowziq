
using System.Collections.Generic;

namespace Meowzic.Core {
    public class Song {

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Fields

        Key key; // キー ※ひとまず転調ではなく旋法の切り替えを実装する

        Mode mode; // 旋法

        List<Pattern> patternList;

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Constructor

        public Song(Key key, Mode mode) {
            this.patternList = new List<Pattern>();
            this.key = key;
            this.mode = mode;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // Properties [noun, adjectives] 

        public Key Key {
            get => key;
        }

        ///////////////////////////////////////////////////////////////////////////////////////////////
        // public Methods [verb]

        public void AddPattern(Pattern pattern) {
            if (pattern.Mode == Mode.Undefined) {
                pattern.Mode = mode; // song の旋法を設定
            }
            patternList.Add(pattern);
        }

        public List<Pattern> GetPattern() {
            return patternList;
        }

    }
}
