# CSharpT4Samples

Created in 2020/09



## やりたいこと

byte配列 から **固定サイズの配列を持つ構造体** に変換（デシリアライズ）したいです。



固定サイズの配列を持つ構造体 は、『他の言語またはプラットフォームのデータ ソースと相互運用する』のような場面で有用かと思います。（[MS公式](https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/unsafe-code-pointers/fixed-size-buffers) の文言を拝借）



#### データ構造

今回は以下のデータ構造を想定しています。

先頭に 4Byte の単一データがあり、以降に 3Byte のデータが隙間なく 128個 並んでいます。

![Capture](https://github.com/hsytkm/CSharpT4Samples/blob/master/DataStructure.png)




#### 対象データ構造の実装イメージ

直観的には以下のように書きたいですが、C#8.0 ではビルドできません。

``` C#
// 3Byteの構造体（ここはビルドできます）
[StructLayout(LayoutKind.Sequential, Size = 3)]
readonly struct My3Bytes
{
    public readonly byte b0, b1, b2;
}

// データ構造の実装（C#8.0ではビルドできません）
[StructLayout(LayoutKind.Sequential, Size = 4 + (3 * 128))]
readonly struct DeserializedData
{
    public readonly int Param;
    public readonly My3Bytes FixedBuffer[128];  // これで固定サイズ配列を定義できない
}
```



## 実装方法

### 実装1. 固定サイズバッファ

構造体内に固定サイズ配列を定義する手段として、固定サイズバッファ（fixed T[]）が用意されています。

これを使えば良いのですが、制約が多く扱い難い印象です…

- 配列型 に 組み込み型（byte, short, int, ...）しか指定できない（3Byteの配列を作れない）
- 当該フィールドを（構造体全体を） readonly にできない
- unsafe必須

``` C#
// 固定サイズバッファ
[StructLayout(LayoutKind.Sequential)]
unsafe struct DeserializedData          // unsafe必須 / readonly不可
{
    public readonly int Param;
    public fixed int FixedBuffer[128];  // 組み込み型限定なので3Byte指定不可 / readonly不可
}
```



### 実装2. べた書き

固定サイズの配列を分解して1行ずつべたで書けば、固定サイズバッファ を使用せずに同じデータ構造を定義できます。

固定サイズバッファの制約（組み込み型 / readonly / unsafe）がなく自由度は高いですが、想像するだけで げんなり します。（宣言だけでなくgetterも必要ですし…）

``` C#
// べた書き
[StructLayout(LayoutKind.Sequential, Size = 4 + (3 * 128))]
readonly struct DeserializedData
{
    public readonly int Param;
    public readonly My3Bytes FixedBuffer0;
    /* 宣言略（げんなり） */
    public readonly My3Bytes FixedBuffer127;
    public readonly My3Bytes GetFixedBuffer(int index) =>
        index switch
        {
            0 => FixedBuffer0,
            /* getter略（うんざり） */
            127 => FixedBuffer127,
            _ => throw new IndexOutOfRangeException()
        };
}
```



### 実装3. その他

良い方法ありましたら教えて下さい！



## どう対応したか？

T4 テキスト テンプレート を使ってべた書きしました。 **力こそパワー💪**



### T4テキストテンプレートとは？

詳細は [MS公式](https://docs.microsoft.com/ja-jp/visualstudio/modeling/code-generation-and-t4-text-templates) で確認して下さい。（私は全部読まずに雰囲気で使っちゃっています…）

ポイントは以下と思っています。

- T4 テキスト テンプレートには、"実行時" と "デザイン時" の 2 種類があり、今回使うのは "デザイン時" の方
- テンプレートテキスト（.tt）から テキストファイル （.cs や .txt）を作成できる
- テキストファイルは即時作成される（ビルド時でない。テンプレートファイルを変更しただけで保存しなくても作成される）



### テキストテンプレート

1. プロジェクトにファイルを追加します。

   追加 → 新しい項目 → テキストテンプレート(.tt) 

2. テンプレートを書きます。

   C# と同じノリで書けて、また変換テキストをすぐに確認できるので雰囲気で使えます。

   今回は対応していませんが、GetEnumerator() や インデクサ も書いておくと便利そうです。

``` C#
<#@ template debug="false" hostspecific="false" language="C#" #>
<#@ assembly name="System.Core" #>
<#@ import namespace="System.Linq" #>
<#@ import namespace="System.Text" #>
<#@ import namespace="System.Collections.Generic" #>
<#@ output extension=".cs" #>  //★出力するファイルの拡張子を指定（デフォは .txt）
<#  //★ソース生成に使用されるコード
    Func<int, int, string> CreateDeclarationFields = (start, count) =>
    {
        return string.Join(Environment.NewLine, Enumerable.Range(start, count)
            .Select(i => "public readonly My3Bytes FixedBuffer" + i.ToString() + ";"));
    };
    Func<int, int, string> CreateSwitchFields = (start, count) =>
    {
        return string.Join(Environment.NewLine, Enumerable.Range(start, count)
            .Select(i => i.ToString() + " => FixedBuffer" + i.ToString() + ","));
    };
#>  //★以降がテキストファイルとして生成される
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>

using System;
using System.Runtime.InteropServices;

namespace CSharpT4Samples
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    readonly struct My3Bytes
    {
        public readonly byte b0, b1, b2;
    }

<# {
    int count = 128;    //★固定配列のサイズ
#>
    //★<#= #> 内がソースコードに展開される
    [StructLayout(LayoutKind.Sequential, Size = 4 + (3 * <#= count #>))]
    readonly struct DeserializedData
    {
        public readonly int Param;
        <#= CreateDeclarationFields(0, count) #>     //★固定配列分フィールドを定義

        public readonly My3Bytes GetFixedBuffer(int index) =>
            index switch
            {
                <#= CreateSwitchFields(0, count) #>  //★Indexのフィールドを参照
                _ => throw new IndexOutOfRangeException()
            };
    }
<# } #>
}
```



### 生成されたテキストファイル

``` C#
// <auto-generated>
// THIS (.cs) FILE IS GENERATED BY T4. DO NOT CHANGE IT. CHANGE THE .tt FILE INSTEAD.
// </auto-generated>

using System;
using System.Runtime.InteropServices;

namespace CSharpT4Samples
{
    [StructLayout(LayoutKind.Sequential, Size = 3)]
    readonly struct My3Bytes
    {
        public readonly byte b0, b1, b2;
    }

    [StructLayout(LayoutKind.Sequential, Size = 4 + (3 * 128))]
    readonly struct DeserializedData
    {
        public readonly int Param;
        public readonly My3Bytes FixedBuffer0;
        /* 宣言略（べた書きだけど手書きじゃないのでOK） */
        public readonly My3Bytes FixedBuffer127;

        public readonly My3Bytes GetFixedBuffer(int index) =>
            index switch
            {
                0 => FixedBuffer0,
                /* getter略（べた書きだけど手書きじゃないのでOK） */
                127 => FixedBuffer127,
                _ => throw new IndexOutOfRangeException()
            };
    }
}
```



### 困ったこと

テンプレートコード内で、ローカル関数 や 文字列補間($"")  を使うと、VSにシンタックスエラーを指摘されました。（コードは生成されますが、エラーが取れないので気持ち悪い…）

今回は以下でお茶を濁しました。

- ローカル関数 → Func を使う
- 文字列補間 → 使わない



## まとめ

固定サイズの配列を持つ構造体 の実装で、T4 を使ってフィールドをべた書きしてみました。

固定サイズバッファ（fixed T[]）を使用しないことで、奇数サイズの配列 や readonly属性 に対応することができます。

よりスマートな実装がありましたらコメントで教えて頂きたいです。



##  環境

Visual Studio Community 2019 16.7.4

.NET Core 3.1

C# 8.0



## 参考にさせて頂いたページ

[固定サイズ バッファー (C# プログラミング ガイド)](https://docs.microsoft.com/ja-jp/dotnet/csharp/programming-guide/unsafe-code-pointers/fixed-size-buffers)

[コード生成と T4 テキスト テンプレート](https://docs.microsoft.com/ja-jp/visualstudio/modeling/code-generation-and-t4-text-templates)

[.NET Core時代のT4によるC#のテキストテンプレート術](http://neue.cc/2019/12/06_585.html)

[T4テキストテンプレート入門 - デザイン時T4テキストテンプレート編](https://qiita.com/Midoliy/items/6b9d7b377e61fdb525ad)