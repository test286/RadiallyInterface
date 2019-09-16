﻿using System.Collections;
using System.Collections.Generic;
//using System.Windows.Forms;
using UnityEngine;
using UnityEngine.XR;

public class centralSystemC : MonoBehaviour {

    private int churingNumber = -100;//各ボタンから得られるカーソル位置情報
    protected int stage = 0;//インターフェースの処理段階       -100=Error  -2=システムアイコン入力済み  -1=外縁  0=Nutral  1=第一段階選択  2=第二段階選択時  3=入力決定
    protected string InputText = "";
    protected string setText = "";
    private int baseNumber;
    private int consonant;  //子音

    /* システムの形決定 */
    protected int poleSum = 5;            //キーの数
    private int trapezoidDivisionNum = 5;     //キー当たりのメッシュ数　1以上
    protected float radiusOut = 1f;       //システムの外縁の半径
    protected float radiusIn = 0.7f;        //ニュートラルエリアの半径
    protected float poleHeight = 1f;      //システムの厚み
    private TextMesh textMesh;

    /* キーを取得済みフラグ */
    private bool isGetKeyObjects = false;
    /* キーオブジェクト */                          //中身はキーを再表示する度に再設定
    private GameObject[] keyObjects;

    /*[親のpointNum,set]*/
    protected string[,] textSet;

    /* か行見出し　か行あ段　か行い段  …
     * あ
     * さ行見出し　さ行あ段　さ行い段  …
     * た行見出し　た行あ段　た行い段  …
     * い
     * な行見出し　な行あ段　な行い段  …
     * ：
     * ：
     */
    //デバッグ用
    protected readonly string[,] textSetDebug = new string[15, 7] {    { "k", "ka"   , "ki", "ku"  , "ke"   , "ko"   , "Error"},
                                                                       { "a", "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "s", "sa"   , "si", "su"  , "se"   , "so"   , "Error"},
                                                                       { "t", "ta"   , "ti", "tu"  , "te"   , "to"   , "Error"},
                                                                       { "i", "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "n", "na"   , "ni", "nu"  , "ne"   , "no"   , "Error"},
                                                                       { "h", "ha"   , "hi", "hu"  , "he"   , "ho"   , "Error"},
                                                                       { "u", "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "m", "ma"   , "mi", "mu"  , "me"   , "mo"   , "Error"},
                                                                       { "y", "ya"   , "゛", "yu"  , "゜"   , "yo"   , "Error"},
                                                                       { "e", "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "r", "ra"   , "ri", "ru"  , "re"   , "ro"   , "Error"},
                                                                       { "w", "wa"   , "wo", "nn"  , "改/確", "空/変", "Error"},
                                                                       { "o", "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "-", "記/数", "BS", "かな", "カナ" , "小"   , "Error"} };
    //ひらがな
    protected readonly string[,] textSetHiragana = new string[15, 7] { { "か", "か"   , "き", "く", "け"   , "こ"   , "Error"},
                                                                       { "あ", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "さ", "さ"   , "し", "す", "せ"   , "そ"   , "Error"},
                                                                       { "た", "た"   , "ち", "つ", "て"   , "と"   , "Error"},
                                                                       { "い", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "な", "な"   , "に", "ぬ", "ね"   , "の"   , "Error"},
                                                                       { "は", "は"   , "ひ", "ふ", "へ"   , "ほ"   , "Error"},
                                                                       { "う", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "ま", "ま"   , "み", "む", "め"   , "も"   , "Error"},
                                                                       { "や", "や"   , "゛", "ゆ", "゜"   , "よ"   , "Error"},
                                                                       { "え", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "ら", "ら"   , "り", "る", "れ"   , "ろ"   , "Error"},
                                                                       { "わ", "わ"   , "を", "ん", "改/確", "空/変", "Error"},
                                                                       { "お", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "--", "記/数", "BS", "英", "カナ" , "小"   , "Error"} };
    //カタカナ
    protected readonly string[,] textSetKatakana = new string[15, 7] { { "カ", "カ"   , "キ", "ク", "ケ"   , "コ"   , "Error"},
                                                                       { "ア", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "サ", "サ"   , "シ", "ス", "セ"   , "ソ"   , "Error"},
                                                                       { "タ", "タ"   , "チ", "ツ", "テ"   , "ト"   , "Error"},
                                                                       { "イ", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "ナ", "ナ"   , "ニ", "ヌ", "ネ"   , "ノ"   , "Error"},
                                                                       { "ハ", "ハ"   , "ヒ", "フ", "ヘ"   , "ホ"   , "Error"},
                                                                       { "ウ", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "マ", "マ"   , "ミ", "ム", "メ"   , "モ"   , "Error"},
                                                                       { "ヤ", "ヤ"   , "゛", "ユ", "゜"   , "ヨ"   , "Error"},
                                                                       { "エ", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "ラ", "ラ"   , "リ", "ル", "レ"   , "ロ"   , "Error"},
                                                                       { "ワ", "ワ"   , "ヲ", "ン", "改/確", "空/変", "Error"},
                                                                       { "オ", "--"   , "--", "--", "--"   , "--"   , "Error"},
                                                                       { "--", "記/数", "BS", "英", "かな" , "小"   , "Error"} };
    //小文字アルファベット
    protected readonly string[,] textSetAlphabet = new string[15, 7] { { "abc" , "a"    , "b" , "c"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "def" , "d"    , "e" , "f"   , "--"   , "--"   , "Error"},
                                                                       { "ghi" , "g"    , "h" , "i"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "jkl" , "j"    , "k" , "l"   , "--"   , "--"   , "Error"},
                                                                       { "mno" , "m"    , "n" , "o"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "pqrs", "p"    , "q" , "r"   , "s"    , "--"   , "Error"},
                                                                       { "tuv" , "t"    , "u" , "v"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "wxyz", "w"    , "x" , "y"   , "z"    , "--"   , "Error"},
                                                                       { "--"  , "--"   , "--", "--"  , "改/確", "空/変", "Error"},
                                                                       { "--"  , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "--"  , "記/数", "BS", "かな", "カナ" , "Ａ"   , "Error"} };
    //大文字アルファベット
    protected readonly string[,] textSetALPHABET = new string[15, 7] { { "ABC" , "A"    , "B" , "C"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "DEF" , "D"    , "E" , "F"   , "--"   , "--"   , "Error"},
                                                                       { "GHI" , "G"    , "H" , "I"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "JKL" , "J"    , "K" , "L"   , "--"   , "--"   , "Error"},
                                                                       { "MNO" , "M"    , "N" , "O"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "PQRS", "P"    , "Q" , "R"   , "S"    , "--"   , "Error"},
                                                                       { "TUV" , "T"    , "U" , "V"   , "--"   , "--"   , "Error"},
                                                                       { ""    , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "WXYZ", "W"    , "X" , "Y"   , "Z"    , "--"   , "Error"},
                                                                       { "--"  , "--"   , "--", "--"  , "改/確", "空/変", "Error"},
                                                                       { "--"  , "--"   , "--", "--"  , "--"   , "--"   , "Error"},
                                                                       { "--"  , "記/数", "BS", "かな", "カナ" , "ａ"   , "Error"} };
    //数字と記号
    protected readonly string[,] textSetSignNum = new string[15, 7] {  { "1-/:;" , "1" , "-"   , "/" , ":"    , ";"    , "Error"},
                                                                       { ""      , "--", "--"  , "--", "--"   , "--"   , "Error"},
                                                                       { "2()\\&", "2" , "("   , ")" , "\\"   , "&"    , "Error"},
                                                                       { "3@\".,", "3" , "@"   , "\"", "."    , ","    , "Error"},
                                                                       { ""      , "--", "--"  , "--", "--"   , "--"   , "Error"},
                                                                       { "4?!'"  , "4" , "?"   , "!" , "'"    , "--"   , "Error"},
                                                                       { "5[]{}" , "5" , "["   , "]" , "{"    , "}"    , "Error"},
                                                                       { ""      , "--", "--"  , "--", "--"   , "--"   , "Error"},
                                                                       { "6#%^*" , "6" , "#"   , "%" , "^"    , "*"    , "Error"},
                                                                       { "7+=_|" , "7" , "+"   , "=" , "_"    , "|"    , "Error"},
                                                                       { ""      , ""  , "--"  , "--", "--"   , "--"   , "Error"},
                                                                       { "8~<>$" , "8" , "~"   , "<" , ">"    , "$"    , "Error"},
                                                                       { "9"     , "9" , "--"  , "--", "改/確", "空/変", "Error"},
                                                                       { ""      , "--", "--"  , "--", "--"   , "--"   , "Error"},
                                                                       { "0"     , "0" , "かな", "BS", "英"   , "カナ" , "Error"} };

    //濁点、半濁点、小文字対応
    /* あ行の検索index   あ   い   う   …
     * あ行の濁点文字    あ゛ い゛ う゛ …
     * あ行の半濁点文字　あ゜ い゜ う゜ …
     * あ行の小文字      ぁ   ぃ   ぅ   …
     * か行の検索index   か   き   く   …
     *      :
     *      :
     */
    protected readonly string[,] RetranslationSet = new string[48, 6] { { "あ", "い", "う", "え", "お", "Error" },
                                                                       { ""  , ""  , "ゔ", ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { "ぁ", "ぃ", "ぅ", "ぇ", "ぉ", "Error" },

                                                                       { "か", "き", "く", "け", "こ", "Error" },
                                                                       { "が", "ぎ", "ぐ", "げ", "ご", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },

                                                                       { "さ", "し", "す", "せ", "そ", "Error" },
                                                                       { "ざ", "じ", "ず", "ぜ", "ぞ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },

                                                                       { "た", "ち", "つ", "て", "と", "Error" },
                                                                       { "だ", "ぢ", "づ", "で", "ど", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , "っ", ""  , ""  , "Error" },

                                                                       { "は", "ひ", "ふ", "へ", "ほ", "Error" },
                                                                       { "ば", "び", "ぶ", "べ", "ぼ", "Error" },
                                                                       { "ぱ", "ぴ", "ぷ", "ぺ", "ぽ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },

                                                                       { "や", ""  , "ゆ", ""  , "よ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { "ゃ", ""  , "ゅ", ""  , "ょ", "Error" },

                                                                       { "ア", "イ", "ウ", "エ", "オ", "Error" },
                                                                       { ""  , ""  , "ヴ", ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { "ァ", "ィ", "ゥ", "ェ", "ォ", "Error" },

                                                                       { "カ", "キ", "ク", "ケ", "コ", "Error" },
                                                                       { "ガ", "ギ", "グ", "ゲ", "ゴ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { "ヵ", ""  , ""  , "ヶ", ""  , "Error" },

                                                                       { "サ", "シ", "ス", "セ", "ソ", "Error" },
                                                                       { "ザ", "ジ", "ズ", "ゼ", "ゾ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },

                                                                       { "タ", "チ", "ツ", "テ", "ト", "Error" },
                                                                       { "ダ", "ヂ", "ヅ", "デ", "ド", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , "ッ", ""  , ""  , "Error" },

                                                                       { "ハ", "ヒ", "フ", "ヘ", "ホ", "Error" },
                                                                       { "バ", "ビ", "ブ", "ベ", "ボ", "Error" },
                                                                       { "パ", "ピ", "プ", "ペ", "ポ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },

                                                                       { "ヤ", ""  , "ユ", ""  , "ヨ", "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { ""  , ""  , ""  , ""  , ""  , "Error" },
                                                                       { "ャ", ""  , "ュ", ""  , "ョ", "Error" } };

    private bool[] useSystemCommand = new bool[10] { true, true, true, true, true, true, true, true, true, true };
    protected readonly string[] SystemCommandName = new string[10] { "改/確", "空/変", "記/数", "BS", "カナ", "小", "英", "かな", "Ａ", "ａ" };
    private bool[] dispSystemCommand = new bool[10] { true, true, true, true, true, true, true, true, true, true };
    //protected readonly Vector3[] SystemCommandVector = new Vector3[10] { new Vector3(), Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };

    private void Awake() {
        //variablesCの初期化
        //variablesC.poleSum = this.poleSum;
        //variablesC.trapezoidDivisionNum = this.trapezoidDivisionNum;
        //variablesC.radiusOut = this.radiusOut;
        //variablesC.radiusIn = this.radiusIn;
        //variablesC.poleHeight = this.poleHeight;
        variablesC.systemCommandNum = 10;
        //variablesC.systemCommandRadius = 5;
        variablesC.useSystemCommand = this.useSystemCommand;
        variablesC.systemCommandName = this.SystemCommandName;
        variablesC.displaySystemCommand = this.dispSystemCommand;
        //variablesC.SystemCommandVector = this.SystemCommandVector;
        //文字セット初期化
        textSet = textSetHiragana;
        //XRであるかどうか
        variablesC.isOnXR = XRSettings.enabled;
        variablesC.createSourcePosition = transform.position;
    }

    void Start() {
        textMesh = GameObject.Find("InputText").GetComponent<TextMesh>();
    }

    void Update() {
        textMesh.text = InputText;
        if (!isGetKeyObjects && GameObject.Find(0.ToString())) {
            GetKeyObjects();
            SetKeytext();
        }
    }

    //外部から座標値を取得
    public void UpdateChuringNum(int nextNum) {
        churingNumber = nextNum;
        ChuringSystem();
    }

    private void ChuringSystem() {
        if (churingNumber < 0) {
            //churingNumberがマイナス＝システムキーに触れたとき
            //数値を反転し、システムキーの名前を参照する
            setText = SystemCommandName[-churingNumber];
            stage = 3;
        } else if (stage == 1 && baseNumber != churingNumber && churingNumber != 0) {
            //主輪選択後で、最初のキー値と入力キー値が違い、中心に戻ったわけではない場合
            //子音が決定するので計算
            if (baseNumber == poleSum && churingNumber == 1) {
                //最後のキーから1キーへの入力の際
                consonant = textSet.GetLength(0) - 1;
            } else if (baseNumber == 1 && churingNumber == poleSum) {
                //１キーから最後のキーへの入力の際
                consonant = 0;
            } else {
                //それ以外の隣り合うキー値が同じ場合の計算
                consonant = ( ( baseNumber - 1 ) * 3 + 1 ) + ( churingNumber - baseNumber );
            }
            //子音と母音から再計算
            setText = textSet[consonant, churingNumber - 1 + 1];
            //次の状態へ
            stage = 2;
            if (isGetKeyObjects)
                SetKeytext();
        } else if (stage == 2 && 1 <= churingNumber && churingNumber <= poleSum + 1) {
            //子音決定済み母音選択状態で、入力キー値が1～キー数の間の場合実行
            setText = textSet[consonant, churingNumber - 1 + 1];
            if (isGetKeyObjects)
                SetKeytext();
        } else if (stage == 0 && 0 < churingNumber && churingNumber <= poleSum + 1) {
            //ニュートラル状態で、入力キー値が1～キー数の間の場合実行
            //最初のキー値を決定
            baseNumber = churingNumber;
            //とりあえず母音を保存
            setText = textSet[( baseNumber - 1 ) * 3 + 1, 0];
            //次の状態へ
            stage = 1;
            /* ここで副輪を呼ぶ */
            GameObject subCircle = new GameObject("subCircle");
            variables.createSourcePosition = keyObjects[churingNumber].transform.Find("text").transform.position;
            subCircle.transform.position = keyObjects[churingNumber].transform.Find("text").transform.position;
            subCircle.AddComponent<createTrapezoidPoleC>();
            //if (isGetKeyObjects)
            //    SetKeytext();
        } else if (( stage == 1 || stage == 2 || stage == 3 ) && churingNumber == 0) {
            //入力状態で、中心へ戻った場合
            //まず、特殊なコマンドは実行する
            SystemCommandChuring();
            //表示用にわかりやすい名前に書き換える
            ConvertToSystemCommand();
            //表示
            InputText = setText;
            //準備用の変数を初期化
            setText = "";
            //中心へ戻った
            stage = 0;
            //各テキストの初期化
            if (isGetKeyObjects)
                SetKeytext();
        } else if (stage == 3) {
            //stage3で、システムキー以外の接触のとき
            //なにもしない
        } else {
            Debug.LogWarning("Error. stage = " + stage + " ." +
                             " churingNumber = " + churingNumber + " ." +
                             " baseNumber = " + baseNumber);
        }
    }

    //他文字セットなどのキーの解釈
    private void SystemCommandChuring() {
        if (setText == "英" || setText == "ａ") {
            textSet = textSetAlphabet;
        } else if (setText == "Ａ") {
            textSet = textSetALPHABET;
        } else if (setText == "かな") {
            textSet = textSetHiragana;
        } else if (setText == "カナ") {
            textSet = textSetKatakana;
        } else if (setText == "記/数") {
            textSet = textSetSignNum;
        } else if (setText == "゛") {
            int i = 0, j = 0;
            if (HaveRetranslationText(ref i, ref j)) {
                setText = RetranslationSet[i * 4 + 1, j];
            }
        } else if (setText == "゜") {
            int i = 0, j = 0;
            if (HaveRetranslationText(ref i, ref j)) {
                setText = RetranslationSet[i * 4 + 2, j];
            }
        } else if (setText == "小") {
            int i = 0, j = 0;
            if (HaveRetranslationText(ref i, ref j)) {
                setText = RetranslationSet[i * 4 + 3, j];
            }
        }
    }

    //直前の入力と再解釈用二次配列を照らし合わせていき、対象か否か判定。対象ならその配列番号を保存
    private bool HaveRetranslationText(ref int i, ref int j) {
        for (i = 0; i < RetranslationSet.GetLength(0) / 4; i++) {
            for (j = 0; j < RetranslationSet.GetLength(1) - 1; j++) {
                if (InputText == RetranslationSet[i * 4, j]) {
                    return true;
                }

            }
        }
        return false;
    }

    //入力された内容をシステムコマンドに変換する(setText内で完結させる)
    private void ConvertToSystemCommand() {
        if (setText == "--") {
            setText = "入力なし";
        } else if (setText == "改/確") {
            setText = "改行/確定";
        } else if (setText == "空/変") {
            setText = "空白/変換";
        } else if (setText == "記/数") {
            setText = "記号など";
        } else if (setText == "BS") {
            setText = "BackSpace";
        } else if (setText == "英") {
            setText = "英語";
        } else if (setText == "かな") {
            setText = "ひらがな";
        } else if (setText == "カナ") {
            setText = "カタカナ";
        } else if (setText == "小") {
            setText = "小文字";
        } else if (setText == "゛") {
            setText = "濁点";
        } else if (setText == "゜") {
            setText = "半濁点";
        } else if (setText == "ａ") {
            setText = "ａｂｃ";
        } else if (setText == "Ａ") {
            setText = "ＡＢＣ";
        }
    }

    //キーオブジェクトの取得
    private void GetKeyObjects() {
        keyObjects = new GameObject[poleSum + 1];
        for (int i = 0; i <= poleSum; i++) {
            if (keyObjects[i] == null) {
                if (GameObject.Find(i.ToString()))
                    keyObjects[i] = GameObject.Find(i.ToString());
                else
                    break;
            }
            if (i == poleSum) {
                isGetKeyObjects = true;
            }
        }

    }

    //キーに文字を割り当てる
    private void SetKeytext() {
        for (int i = 1; i <= poleSum; i++) {
            //TrapezoidPoleC keyObjectITrapezoid = keyObjects[i].GetComponent<TrapezoidPoleC>();
            MultipleTrapezoidPoleC keyObjectITrapezoid = keyObjects[i].GetComponent<MultipleTrapezoidPoleC>();
            if (stage == 0) {
                keyObjectITrapezoid.MyText = textSet[i * 3 - 3, 0] + textSet[i * 3 - 2, 0] + textSet[i * 3 - 1, 0];
            } else if (stage == 1) {
                if (churingNumber - 1 == i) {
                    //左隣の値
                    keyObjectITrapezoid.MyText = textSet[( churingNumber - 1 ) * 3, 0];
                } else if (churingNumber == i) {
                    //現在座標の値
                    keyObjectITrapezoid.MyText = textSet[( churingNumber - 1 ) * 3 + 1, 0];
                } else if (churingNumber + 1 == i) {
                    //右隣の値
                    keyObjectITrapezoid.MyText = textSet[( churingNumber - 1 ) * 3 + 2, 0];
                } else if (churingNumber - ( poleSum - 1 ) == i) {
                    //現在地(churingNumber)が端(最大値)の場合の右隣の値
                    keyObjectITrapezoid.MyText = textSet[( churingNumber - 1 ) * 3 + 2, 0];
                } else if (churingNumber + ( poleSum - 1 ) == i) {
                    //現在地(churingNumber)が端(1)の場合の左隣の値
                    keyObjectITrapezoid.MyText = textSet[0, 0];
                } else {
                    keyObjectITrapezoid.MyText = "--";
                }
            } else if (stage == 2) {
                keyObjectITrapezoid.MyText = textSet[consonant, i];
            }
        }
    }
}
