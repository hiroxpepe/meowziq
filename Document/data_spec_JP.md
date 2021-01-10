# MeowziQ 楽曲データ記述仕様 ver 1.0.0 日本語版

---
## 楽曲データについて
+ 楽曲を演奏する際に以下の4種類のテキストデータファイルを必要とします
+ テキストデータは既存のテキストエディタ等を使用して作成します
    |No|概念|ファイル名|内容|
    |:--|:--|:--|:--|
    |1|Pattern|pattern.json|楽曲のパターン、コード進行を定義します|
    |2|Song|song.json|楽曲のキー、旋法、構成を定義します|
    |3|Phrase|phrase.json|楽曲のフレーズを定義します|
    |4|Player|player.json|楽曲の演奏者を定義します|
+ 楽曲データは JSON ファイルとして作成します
+ データの記述について
    + '[ ]' で区切られた範囲を1小節とみなします 
    + '|' で区切られた範囲を1拍とみなします
    + 現時点では4/4拍子の楽曲にしか対応しません

---
## 記述フォーマット
+ json 形式で記述します [※参考](https://lab.syncer.jp/Tool/JSON-Viewer/)

---
## Pattern 記述仕様
+ 定義のルートを "pattern" とします
+ pattern.json の内容
    |No|項目|内容|
    |:--|:--|:--|
    |1|name|Pattern の名前を記述します、Song の pattern 項目と同一にする必要があります|
    |2|data|Pattern の内容を記述します、※記述方法については後述します|
+ Pattern の記述例
    + Song キーに対する度数をローマ数字(I,V文字を使用)で記述します
    ```json
    {
        "pattern": [
            {
                "name": "intro",
                "data": "[I | | | ][IV| | | ][I| | | ][IV| | | ]"
            },
            {
                "name": "verse1",
                "data": "[I | | | ][VI| | | ][VI| | | ][VII| | | ]"
            },
            {
                "name": "chorus1",
                "data": "[IV | | | ][IV| | | ][V| | | ][VII| | | ]"
            }
        ]
    }
    ```
+ 1小節内では度数の指定が必須です  
    ```
    NG: "[I| | | ][ | | | ][ | | | ][ | | | ]"
    OK: "[I| | | ][I| | | ][I| | | ][I| | | ]"
    ```
+ 度数指定がない場合でも | と |、] の間に半角空白が必要です  
+ 度数の記述について、**Song の旋法**設定で以下のように**自動的に変換**されます
    |記述|Lyd|Ion|Mix|Dor|Aeo|Phr|Loc|
    |:--|:--|:--|:--|:--|:--|:--|:--|
    |I  |I  |I  |I   |I   |I   |I   |I   |
    |II |II |II |II  |II  |II  |bII |bII |
    |III|III|III|III |bIII|bIII|bIII|bIII|
    |IV |#IV|IV |IV  |IV  |IV  |IV  |IV  |
    |V  |V  |V  |V   |V   |V   |V   |bV  |
    |VI |VI |VI |VI  |VI  |bVI |bVI |bVI |
    |VII|VII|VII|bVII|bVII|bVII|bVII|bVII|
    + ※現時点では曲中の転調に対応していません (TODO:)
+ 旋法について
    |旋法|記述|スケール|備考|
    |:--|:--|:--|:--|
    |リディアン      |Lyd|1, 2, 3, #4, 5, 6, 7||
    |イオニアン      |Ion|1, 2, 3, 4, 5, 6, 7|メジャースケール = 長調|
    |ミクソリディアン|Mix|1, 2, 3, 4, 5, 6, b7||
    |ドリアン        |Dor|1, 2, b3, 4, 5, 6, b7||
    |エオリアン      |Aeo|1, 2, b3, 4, 5, b6, b7|マイナースケール = 短調|
    |フリジアン      |Phr|1, b2, b3, 4, 5, b6, b7||
    |ロクリアン      |Loc|1, b2, b3, 4, b5, b6, b7||
+ 度数に対する旋法は、Song の旋法より**自動的に変換**されます
    |Song 旋法|I|II|III|IV|V|VI|VII|
    |:--|:--|:--|:--|:--|:--|:--|:--|
    |Lyd|Lyd|Mix|Aeo|Loc|Ion|Dor|Phr|
    |Ion|Ion|Dor|Phr|Lyd|Mix|Aeo|Loc|
    |Mix|Mix|Aeo|Loc|Ion|Dor|Phr|Lyd|
    |Dor|Dor|Phr|Lyd|Mix|Aeo|Loc|Ion|
    |Aeo|Aeo|Loc|Ion|Dor|Phr|Lyd|Mix|
    |Phr|Phr|Lyd|Mix|Aeo|Loc|Ion|Dor|
    |Loc|Loc|Ion|Dor|Phr|Lyd|Mix|Aeo|
+ 上記の変換により、度数記述は3和音の場合では以下のように**自動的に変換**されます
    |記述|Lyd|Ion|Mix|Dor|Aeo|Phr|Loc|
    |:--|:--|:--|:--|:--|:--|:--|:--|
    |I  |I     |I     |I     |Im   |Im   |Im   |Im-5 |
    |II |II    |IIm   |IIm   |IIm  |IIm-5|bII  |bII  |
    |III|IIIm  |IIIm  |IIIm-5|bIII |bIII |bIII |bIIIm|
    |IV |#IVm-5|IV    |IV    |IV   |IVm  |IVm  |IVm  |
    |V  |V     |V     |Vm    |Vm   |Vm   |Vm-5 |bV   |
    |VI |VIm   |VIm   |VIm   |VIm-5|bVI  |bVI  |bVI  |
    |VII|VIIm  |VIIm-5|bVII  |bVII |bVII |bVIIm|bVIIm|
+ また同様に、度数記述は4和音の場合では以下のように**自動的に変換**されます
    |記述|Lyd|Ion|Mix|Dor|Aeo|Phr|Loc|
    |:--|:--|:--|:--|:--|:--|:--|:--|
    |I  |IM7    |IM7    |I7     |Im7   |Im7   |Im7   |Im7-5 |
    |II |II7    |IIm7   |IIm7   |IIm7  |IIm7-5|bIIM7 |bIIM7 |
    |III|IIIm7  |IIIm7  |IIIm7-5|bIIIM7|bIIIM7|bIII7 |bIIIm7|
    |IV |#IVm7-5|IVM7   |IVM7   |IV7   |IVm7  |IVm7  |IVm7  |
    |V  |VM7    |V7     |Vm7    |Vm7   |Vm7   |Vm7-5 |bVM7  |
    |VI |VIm7   |VIm7   |VIm7   |VIm7-5|bVIM7 |bVIM7 |bVI7  |
    |VII|VIIm7  |VIIm7-5|bVIIM7 |bVIIM7|bVII7 |bVIIm7|bVIIm7|
+ 度数に対する旋法を直接指定する場合には、コロンの後に記述します
    ```
    "[I:Dor| | | ][VI:Mix| | | ][VI| | | ][VII| | | ]"
    ```

---
## Song 記述仕様
+ 定義のルートを "song" とします
+ song.json の内容
    |No|項目|内容|
    |:--|:--|:--|
    |1|name|Song の名前を記述します|
    |2|tempo|※Song のテンポを指定します ※未実装：今後実装予定|
    |3|key|Song のキーを指定します、※記述方法については後述します|
    |4|mode|Song の旋法を指定します|
    |5|pattern|Pattern の名前を配列として指定します|
+ Song の記述例
    ```json
    {
        "song": {
            "name": "song-demo",
            "tempo": "120",
            "key": "C",
            "mode": "Aeo",
            "pattern": [
                "intro",
                "verse1",
                "verse1",
                "chorus1",
                "chorus1",
                "outro"
            ]
        }
    }
    ```
    + ※ Pattern の name 要素と Song の pattern 要素は一致する必要があります
+ キーについて
    |No|記述|備考|
    |:--|:--|:--|
    |1 |E |    |
    |2 |F |    |
    |3 |Gb|= F#|
    |4 |G |    |
    |5 |Ab|= G#|
    |6 |A |    |
    |7 |Bb|= A#|
    |8 |B |    |
    |9 |C |    |
    |10|Db|= C#|
    |11|D |    |
    |12|Eb|= D#|
---
## Phrase 記述仕様
+ 定義のルートを "phrase" とします
+ phrase.json の内容
    |No|項目|内容|
    |:--|:--|:--|
    |1|type     |Phrase の種類を記述します、Player の type 項目と同一にする必要があります|
    |2|name     |Phrase の名前を記述します、Pattern の name 項目と同一にする必要があります|
    |3|note     |Phrase の内容を記述します ※単一の設定、オクターブは設定出来ません|
    |4|data.inst|Phrase の楽器(ドラム)を指定します、※ drum のみ、記述方法については後述します|
    |5|data.note|Phrase の内容を記述します ※複数の設定|
    |6|data.oct |Phrase のオクターブを記述します ※複数の設定|
+ Phrase の記述例
    ```json
    {
        "phrase": [
            {
                "type": "drum",
                "name": "verse1",
                "data": {
                    "inst": [
                        "Crash_Cymbal_1",
                        "Closed_Hi_hat",
                        "Open_Hi_hat",
                        "Electric_Snare",
                        "Electric_Bass_Drum"
                    ],
                    "note": [
                        "[----|----|----|----][----|----|----|----][----|----|----|----][----|----|----|--x-]",
                        "[x-x-|x-x-|x-x-|x---][x-x-|x-x-|x-x-|x---][xxxx|xxxx|xxxx|xx--][xxxx|xxxx|xxxx|xx--]",
                        "[----|----|----|--x-][----|----|----|--x-][----|----|----|--x-][----|----|----|--x-]",
                        "[----|x---|----|x---][----|x---|----|x---][----|x---|----|x---][----|x---|----|x---]",
                        "[x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----][x---|---x|x---|----]"
                    ]
                }
            },
            {
                "type": "bass",
                "name": "verse1",
                "note": "[1-1-|3-3-|5-5-|7-7-][1-1-|3-3-|5-5-|7-7-][1-11|3-33|5-55|3-33][1-11|3-33|5-55|7-77]"
            },
            {
                "type": "pad",
                "name": "verse1",
                "data": {
                    "note": [
                        "[5>>>|>>--|----|----][5555|----|----|----][5>>>|>>--|----|----][5555|----|----|----]",
                        "[3>>>|>>--|----|----][----|----|----|----][3>>>|>>--|----|----][----|----|----|----]",
                        "[1>>>|>>--|----|----][1111|----|----|----][1>>>|>>--|----|----][1111|----|----|----]"
                    ],
                    "oct": [
                        " 0",
                        "-1",
                        " 0"
                    ]
                }
            },
            {
                "type": "seque",
                "name": "verse1"
            },
        ]
    }
    ```

+ (仮)コード指定記述 ※旋法に存在するコードに制限する
    ```
    {
        "type": "pad",
        "name": "verse1",
        "data": {
            "chord": [
                "[3>>>|>>--|----|----][3333|----|----|----][3>>>|>>--|----|----][3333|----|----|----]"
            ]
        }
    }
    ```
+ (仮)コード指定記述

    |記述|内容|構成音|Lyd|Ion|Mix|Dor|Aeo|Phr|Loc|
    |:--|:--|:--|:--|:--|:--|:--|:--|:--|:--|
    |3|3和音|1, 3, 5|C|C|C|Cm|Cm|Cm|Cm(b5)|
    |4|3和音|1, 4, 5|C(#4)|Csus4|Csus4|Csus4|Csus4|Csus4|Csus4(-5)|
    |5|2和音|1, 5|C5(no3)|C5(no3)|C5(no3)|C5(no3)|C5(no3)|C5(no3)|C(-5,no3)|
    |6|4和音|1, 3, 5, 6|C6|C6|C6|Cm6|Cm(b6)|Cm(b6)|Cm(-5,b6)|
    |7|4和音|1, 3, 5, 7|CM7|CM7|C7|Cm7|Cm7|Cm7|Cm7(-5)|
    |9|4和音|1, 3, 5, 9|Cadd9|Cadd9|Cadd9|Cmadd9|Cmadd9|Cm(b9)|Cm(-5,b9)|

---
## Player 記述仕様
+ 定義のルートを "player" とします
+ player.json の内容
    |No|項目|内容|
    |:--|:--|:--|
    |1|type|Player の種類を指定します、※現在、drum、bass、pad、seque に対応します (TODO:)|
    |2|midi|Player の MIDIチャンネルを指定します、drum は9を指定します|
    |3|inst|Player の楽器を記述します、※記述方法については後述します|
+ Player の記述例
    ```json
    {
        "player": [
            {
                "type": "drum",
                "midi": "9",
                "inst": "Standard"
            },
            {
                "type": "bass",
                "midi": "0",
                "inst": "Synth_Bass_1"
            },
            {
                "type": "seque",
                "midi": "1",
                "inst": "Lead_1_square"
            },
            {
                "type": "pad",
                "midi": "2",
                "inst": "Pad_3_polysynth"
            }
        ]
    }
    ```

---
## 楽器(音色)の設定について
+ 通常 Player
    |No|記述|種別|楽器名|
    |:--|:--|:--|:--|
    |  1|Acoustic_Grand_Piano|Piano|アコースティック グランドピアノ|
    |  2|Bright_Acoustic_Piano|Piano|ブライト アコースティックピアノ|
    |  3|Electric_Grand_Piano|Piano|エレクトリック グランドピアノ|
    |  4|Honky_tonk_Piano|Piano|ホンキートンクピアノ|
    |  5|Electric_Piano_1|Piano|エレクトリックピアノ|
    |  6|Electric_Piano_2|Piano|FMエレクトリックピアノ|
    |  7|Harpsichord|Piano|ハープシコード|
    |  8|Clavi|Piano|クラビネット|
    |  9|Celesta|Chromatic Percussion|チェレスタ|
    | 10|Glockenspiel|Chromatic Percussion|グロッケンシュピール|
    | 11|Music_Box|Chromatic Percussion|オルゴール|
    | 12|Vibraphone|Chromatic Percussion|ヴィブラフォン|
    | 13|Marimba|Chromatic Percussion|マリンバ|
    | 14|Xylophone|Chromatic Percussion|シロフォン|
    | 15|Tubular_Bells|Chromatic Percussion|チューブラーベル|
    | 16|Dulcimer|Chromatic Percussion|ダルシマー|
    | 17|Drawbar_Organ|Organ|ドローバーオルガン|
    | 18|Percussive_Organ|Organ|パーカッシブオルガン|
    | 19|Rock_Organ|Organ|ロックオルガン|
    | 20|Church_Organ|Organ|チャーチオルガン|
    | 21|Reed_Organ|Organ|リードオルガン|
    | 22|Accordion|Organ|アコーディオン|
    | 23|Harmonica|Organ|ハーモニカ|
    | 24|Tango_Accordion|Organ|タンゴ アコーディオン|
    | 25|Acoustic_Guitar_nylon|Guitar|アコースティックギター (ナイロン弦)|
    | 26|Acoustic_Guitar_steel|Guitar|アコースティックギター (スチール弦)|
    | 27|Electric_Guitar_jazz|Guitar|ジャズギター|
    | 28|Electric_Guitar_clean|Guitar|クリーンギター|
    | 29|Electric_Guitar_muted|Guitar|ミュートギター|
    | 30|Overdriven_Guitar|Guitar|オーバードライブギター|
    | 31|Distortion_Guitar|Guitar|ディストーションギター|
    | 32|Guitar_Harmonics|Guitar|ギターハーモニクス|
    | 33|Acoustic_Bass|Bass|アコースティックベース|
    | 34|Electric_Bass_finger|Bass|フィンガーベース|
    | 35|Electric_Bass_pick|Bass|ピックベース|
    | 36|Fretless_Bass|Bass|フレットレスベース|
    | 37|Slap_Bass_1|Bass|スラップベース 1|
    | 38|Slap_Bass_2|Bass|スラップベース 2|
    | 39|Synth_Bass_1|Bass|シンセベース 1|
    | 40|Synth_Bass_2|Bass|シンセベース 2|
    | 41|Violin|Strings|ヴァイオリン|
    | 42|Viola|Strings|ヴィオラ|
    | 43|Cello|Strings|チェロ|
    | 44|Contrabass|Strings|コントラバス|
    | 45|Tremolo_Strings|Strings|トレモロ|
    | 46|Pizzicato_Strings|Strings|ピッチカート|
    | 47|Orchestral_Harp|Strings|ハープ|
    | 48|Timpani|Strings|ティンパニ|
    | 49|String_Ensemble_1|Ensemble|ストリングアンサンブル|
    | 50|String_Ensemble_2|Ensemble|スローストリングアンサンブル|
    | 51|Synth_Strings_1|Ensemble|シンセストリングス 1|
    | 52|Synth_Strings_2|Ensemble|シンセストリングス 2|
    | 53|Choir_Aahs|Ensemble|声 (アー)|
    | 54|Voice_Oohs|Ensemble|声 (ドゥー)|
    | 55|Synth_Voice|Ensemble|シンセヴォイス|
    | 56|Orchestra_Hit|Ensemble|オーケストラヒット|
    | 57|Trumpet|Brass|トランペット|
    | 58|Trombone|Brass|トロンボーン|
    | 59|Tuba|Brass|チューバ|
    | 60|Muted_Trumpet|Brass|ミュートトランペット|
    | 61|French_Horn|Brass|フレンチホルン|
    | 62|Brass_Section|Brass|ブラスセクション|
    | 63|Synth_Brass_1|Brass|シンセブラス 1|
    | 64|Synth_Brass_2|Brass|シンセブラス 2|
    | 65|Soprano_Sax|Reed|ソプラノサックス|
    | 66|Alto_Sax|Reed|アルトサックス|
    | 67|Tenor_Sax|Reed|テナーサックス|
    | 68|Baritone_Sax|Reed|バリトンサックス|
    | 69|Oboe|Reed|オーボエ|
    | 70|English_Horn|Reed|イングリッシュホルン|
    | 71|Bassoon|Reed|ファゴット|
    | 72|Clarinet|Reed|クラリネット|
    | 73|Piccolo|Pipe|ピッコロ|
    | 74|Flute|Pipe|フルート|
    | 75|Recorder|Pipe|リコーダー|
    | 76|Pan_Flute|Pipe|パンフルート|
    | 77|Blown_bottle|Pipe|ブロウンボトル|
    | 78|Shakuhachi|Pipe|尺八|
    | 79|Whistle|Pipe|口笛|
    | 80|Ocarina|Pipe|オカリナ|
    | 81|Lead_1_square|Synth Lead|矩形波|
    | 82|Lead_2_sawtooth|Synth Lead|ノコギリ波|
    | 83|Lead_3_calliope|Synth Lead|カリオペリード|
    | 84|Lead_4_chiff|Synth Lead|チフリード|
    | 85|Lead_5_charang|Synth Lead|チャランゴリード|
    | 86|Lead_6_voice|Synth Lead|声リード|
    | 87|Lead_7_fifths|Synth Lead|フィフスズリード|
    | 88|Lead_8_bass_and_lead|Synth Lead|ベース+リード|
    | 89|Pad_1_new_age|Synth Pad|ファンタジア|
    | 90|Pad_2_warm|Synth Pad|ウォーム|
    | 91|Pad_3_polysynth|Synth Pad|ポリシンセ|
    | 92|Pad_4_choir|Synth Pad|クワイア|
    | 93|Pad_5_bowed|Synth Pad|ボウ|
    | 94|Pad_6_metallic|Synth Pad|メタリック|
    | 95|Pad_7_halo|Synth Pad|ハロー|
    | 96|Pad_8_sweep|Synth Pad|スウィープ|
    | 97|FX_1_rain|Synth Effects|雨|
    | 98|FX_2_soundtrack|Synth Effects|サウンドトラック|
    | 99|FX_3_crystal|Synth Effects|クリスタル|
    |100|FX_4_atmosphere|Synth Effects|アトモスフィア|
    |101|FX_5_brightness|Synth Effects|ブライトネス|
    |102|FX_6_goblins|Synth Effects|ゴブリン|
    |103|FX_7_echoes|Synth Effects|エコー|
    |104|FX_8_sci_fi|Synth Effects|サイファイ|
    |105|Sitar|Ethnic|シタール|
    |106|Banjo|Ethnic|バンジョー|
    |107|Shamisen|Ethnic|三味線|
    |108|Koto|Ethnic|琴|
    |109|Kalimba|Ethnic|カリンバ|
    |110|Bag_pipe|Ethnic|バグパイプ|
    |111|Fiddle|Ethnic|フィドル|
    |112|Shanai|Ethnic|シャハナーイ|
    |113|Tinkle_Bell|Percussive|ティンクルベル|
    |114|Agogo|Percussive|アゴゴ|
    |115|Steel_Drums|Percussive|スチールドラム|
    |116|Woodblock|Percussive|ウッドブロック|
    |117|Taiko_Drum|Percussive|太鼓|
    |118|Melodic_Tom|Percussive|メロディックタム|
    |119|Synth_Drum|Percussive|シンセドラム|
    |120|Reverse_Cymbal|Percussive|逆シンバル|
    |121|Guitar_Fret_Noise|Sound effect|ギターフレットノイズ|
    |122|Breath_Noise|Sound effect|ブレスノイズ|
    |123|Seashore|Sound effect|海岸|
    |124|Bird_Tweet|Sound effect|鳥のさえずり|
    |125|Telephone_Ring|Sound effect|電話のベル|
    |126|Helicopter|Sound effect|ヘリコプター|
    |127|Applause|Sound effect|拍手|
    |128|Gunshot|Sound effect|銃声|

+ ドラム Player
    |No|記述|楽器名|
    |:--|:--|:--|
    |1 |Standard|スタンダード ドラムキット|
    |2 |Room|ルーム ドラムキット|
    |3 |Power|パワー ドラムキット|
    |4 |Electronic|エレクトリック ドラムキット|
    |5 |Analog|アナログ ドラムキット|
    |6 |Jazz|ジャズ ドラムキット|
    |7 |Brush|ブラシ ドラムキット|
    |8 |SFX|SFXキット|

+ ドラム Phrase
    |No|記述|楽器名|
    |:--|:--|:--|
    |1 |Acoustic_Bass_Drum|アコースティック バスドラム|
    |2 |Electric_Bass_Drum|エレクトリック バスドラム|
    |3 |Side_Stick|サイドスティック|
    |4 |Acoustic_Snare|アコースティック スネアドラム|
    |5 |Hand_Clap|ハンドクラップ|
    |6 |Electric_Snare|エレクトリック スネアドラム|
    |7 |Low_Floor_Tom|ローフロアタム|
    |8 |Closed_Hi_hat|クローズハイハット|
    |9 |High_Floor_Tom|ハイフロアタム|
    |10|Pedal_Hi_hat|ペダルハイハット|
    |11|Low_Tom|ロータム|
    |12|Open_Hi_hat|オープンハイハット|
    |13|Low_Mid_Tom|ローミッドタム|
    |14|Hi_Mid_Tom|ハイミッドタム|
    |15|Crash_Cymbal_1|クラッシュシンバル1|
    |16|High_Tom|ハイタム|
    |17|Ride_Cymbal_1|ライドシンバル1|
    |18|Chinese_Cymbal|チャイニーズシンバル|
    |19|Ride_Bell|ライドベル|
    |20|Tambourine|タンバリン|
    |21|Splash_Cymbal|スプラッシュシンバル|
    |22|Cowbell|カウベル|
    |23|Crash_Cymbal_2|クラッシュシンバル2|
    |24|Vibra_Slap|ビブラスラップ|
    |25|Ride_Cymbal_2|ライドシンバル2|
    |26|High_Bongo|ハイボンゴ|
    |27|Low_Bongo|ローボンゴ|
    |28|Mute_High_Conga|ミュートハイコンガ|
    |29|Open_High_Conga|オープンハイコンガ|
    |30|Low_Conga|ローコンガ|
    |31|High_Timbale|ハイティンバル|
    |32|Low_Timbale|ローティンバル|
    |33|High_Agogo|ハイアゴゴ|
    |34|Low_Agogo|ローアゴゴ|
    |35|Cabasa|カバサ|
    |36|Maracas|マラカス|
    |37|Short_Whistle|ショートホイッスル|
    |38|Long_Whistle|ロングホイッスル|
    |39|Short_Guiro|ショートギロ|
    |40|Long_Guiro|ロングギロ|
    |41|Claves|クラベス|
    |42|High_Woodblock|ハイウッドブロック|
    |43|Low_Woodblock|ローウッドブロック|
    |44|Mute_Cuica|ミュートクイーカ|
    |45|Open_Cuica|オープンクイーカ|
    |46|Mute_Triangle|ミュートトライアングル|
    |47|Open_Triangle|オープントライアングル|


+ TODO: シンコペーションの実装 and 記述方法
+ MEMO: ドラムのシンコペーションには制限有り