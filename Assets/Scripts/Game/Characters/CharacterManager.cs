using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// キャラクター管理クラス
/// </summary>
public class CharacterManager
{
    private Dictionary<string, Character> characters;
    private Dictionary<string, RivalCharacter> rivalCharacters;
    private Dictionary<string, GeneralCharacter> generalCharacters;

    /// <summary>
    /// すべてのキャラクター
    /// </summary>
    public IEnumerable<Character> AllCharacters => characters.Values;

    /// <summary>
    /// すべてのライバルキャラクター
    /// </summary>
    public IEnumerable<RivalCharacter> AllRivalCharacters => rivalCharacters.Values;

    /// <summary>
    /// すべての一般キャラクター
    /// </summary>
    public IEnumerable<GeneralCharacter> AllGeneralCharacters => generalCharacters.Values;

    /// <summary>
    /// いずれかのキャラクターの好感度が変わった時のイベント
    /// </summary>
    public event Action OnAnyCharacterRomanceChanged;

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public CharacterManager()
    {
        characters = new Dictionary<string, Character>();
        rivalCharacters = new Dictionary<string, RivalCharacter>();
        generalCharacters = new Dictionary<string, GeneralCharacter>();
        InitializeCharacters();
        InitializeRivalCharacters();
        InitializeGeneralCharacters();
    }

    /// <summary>
    /// キャラクターを初期化
    /// </summary>
    private void InitializeCharacters()
    {
        // キャラクター1 - 職人（攻略可能）
        Character char1 = new Character(
            "char_01",
            "職人",
            "家や道具を作る職人。科学者が作った素材で釣竿やフライパンを作ってくれる。",
            ShopType.Craftsman
        );
        char1.AddFavoriteItem("wood", "stone", "vine");
        char1.AddDislikedItem("berry");
        AddCharacter(char1);

        // キャラクター2 - 料理人（攻略可能）
        Character char2 = new Character(
            "char_02",
            "料理人",
            "料理が得意。レシピを販売している。",
            ShopType.Cook
        );
        char2.AddFavoriteItem("fish", "coconut");
        char2.AddDislikedItem("stone");
        AddCharacter(char2);

        // キャラクター3 - 医者（攻略可能）
        Character char3 = new Character(
            "char_03",
            "医者",
            "体力とスタミナを回復してくれる。",
            ShopType.Doctor
        );
        char3.AddFavoriteItem("berry");
        char3.AddDislikedItem("wood");
        AddCharacter(char3);

        Debug.Log($"CharacterManager初期化: {characters.Count}人のキャラクター登録");
    }

    /// <summary>
    /// ライバルキャラクターを初期化
    /// </summary>
    private void InitializeRivalCharacters()
    {
        // ライバル1 - 職人のライバル
        AddRivalCharacter(new RivalCharacter("rival_01", "ライバル職人"));

        // ライバル2 - 料理人のライバル
        AddRivalCharacter(new RivalCharacter("rival_02", "ライバル料理人"));

        // ライバル3 - 医者のライバル
        AddRivalCharacter(new RivalCharacter("rival_03", "ライバル医者"));

        Debug.Log($"CharacterManager初期化: {rivalCharacters.Count}人のライバルキャラクター登録");
    }

    /// <summary>
    /// 一般キャラクターを初期化
    /// </summary>
    private void InitializeGeneralCharacters()
    {
        // 科学者 - 店あり、友好度のみ、恋愛相談イベントあり
        GeneralCharacter scientist = new GeneralCharacter(
            "char_04",
            "科学者",
            "釣竿に必要な糸やガラスなどの素材を販売している。プレイヤーの恋愛相談に乗ってくれる。",
            ShopType.Scientist
        );
        AddGeneralCharacter(scientist);

        Debug.Log($"CharacterManager初期化: {generalCharacters.Count}人の一般キャラクター登録");
    }

    /// <summary>
    /// キャラクターを追加
    /// </summary>
    private void AddCharacter(Character character)
    {
        if (characters.ContainsKey(character.id))
        {
            Debug.LogWarning($"キャラクターID '{character.id}' は既に登録されています。");
            return;
        }

        characters.Add(character.id, character);

        // 好感度変更イベントを購読
        character.OnRomanceChanged += (current, change) => OnAnyCharacterRomanceChanged?.Invoke();
    }

    /// <summary>
    /// ライバルキャラクターを追加
    /// </summary>
    private void AddRivalCharacter(RivalCharacter rivalCharacter)
    {
        if (rivalCharacters.ContainsKey(rivalCharacter.id))
        {
            Debug.LogWarning($"ライバルキャラクターID '{rivalCharacter.id}' は既に登録されています。");
            return;
        }

        rivalCharacters.Add(rivalCharacter.id, rivalCharacter);
    }

    /// <summary>
    /// 一般キャラクターを追加
    /// </summary>
    private void AddGeneralCharacter(GeneralCharacter generalCharacter)
    {
        if (generalCharacters.ContainsKey(generalCharacter.id))
        {
            Debug.LogWarning($"一般キャラクターID '{generalCharacter.id}' は既に登録されています。");
            return;
        }

        generalCharacters.Add(generalCharacter.id, generalCharacter);
    }

    /// <summary>
    /// キャラクターIDからキャラクターを取得
    /// </summary>
    public Character GetCharacter(string characterId)
    {
        if (characters.TryGetValue(characterId, out Character character))
        {
            return character;
        }

        Debug.LogWarning($"キャラクターID '{characterId}' が見つかりません。");
        return null;
    }

    /// <summary>
    /// ライバルキャラクターIDからライバルキャラクターを取得
    /// </summary>
    public RivalCharacter GetRivalCharacter(string rivalCharacterId)
    {
        if (rivalCharacters.TryGetValue(rivalCharacterId, out RivalCharacter rivalCharacter))
        {
            return rivalCharacter;
        }

        Debug.LogWarning($"ライバルキャラクターID '{rivalCharacterId}' が見つかりません。");
        return null;
    }

    /// <summary>
    /// 一般キャラクターIDから一般キャラクターを取得
    /// </summary>
    public GeneralCharacter GetGeneralCharacter(string generalCharacterId)
    {
        if (generalCharacters.TryGetValue(generalCharacterId, out GeneralCharacter generalCharacter))
        {
            return generalCharacter;
        }

        Debug.LogWarning($"一般キャラクターID '{generalCharacterId}' が見つかりません。");
        return null;
    }

    /// <summary>
    /// すべてのキャラクターのサマリー
    /// </summary>
    public string GetSummary()
    {
        string summary = "【攻略キャラクター】\n";
        foreach (var character in characters.Values)
        {
            summary += $"  {character.GetSummary()}\n";
        }

        summary += "\n【ライバルキャラクター】\n";
        foreach (var rivalCharacter in rivalCharacters.Values)
        {
            summary += $"  {rivalCharacter.GetSummary()}\n";
        }

        summary += "\n【一般キャラクター】\n";
        foreach (var generalCharacter in generalCharacters.Values)
        {
            summary += $"  {generalCharacter.GetSummary()}\n";
        }

        return summary;
    }

    /// <summary>
    /// 友好度が最も高いキャラクターを取得
    /// </summary>
    public Character GetHighestFriendshipCharacter()
    {
        return characters.Values.OrderByDescending(c => c.Friendship).FirstOrDefault();
    }

    /// <summary>
    /// 恋愛度が最も高いキャラクターを取得
    /// </summary>
    public Character GetHighestRomanceCharacter()
    {
        return characters.Values.OrderByDescending(c => c.Romance).FirstOrDefault();
    }
}
