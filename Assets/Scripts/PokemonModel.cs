using System.Collections.Generic;
using Newtonsoft.Json;

[System.Serializable]
public class PokemonData
{
    public int id;
    public string name;
    public List<string> types;
    public string sprite;
}

[System.Serializable]
public class PokemonApiResponse
{
    public string name;
    public TypeSlot[] types;
    public SpriteContainer sprites;
}

[System.Serializable]
public class TypeSlot
{
    public TypeInfo type;
}

[System.Serializable]
public class TypeInfo
{
    public string name;
}

[System.Serializable]
public class SpriteContainer
{
    public string front_default;
}

[System.Serializable]
public class TypeWrapper
{
    public TypeDetail type;
}

[System.Serializable]
public class TypeDetail
{
    public string name;
}

[System.Serializable]
public class PokemonSpeciesData
{
    public List<NameEntry> names;
}

[System.Serializable]
public class NameEntry
{
    public string name;
    public Language language;
}

[System.Serializable]
public class Language
{
    public string name;
}

//public static class TypeTranslator
//{
//    public static readonly Dictionary<string, string> typeTranslations = new Dictionary<string, string>
//    {
//        { "normal", "노말" }, { "fire", "불꽃" }, { "water", "물" }, { "electric", "전기" }, { "grass", "풀" },
//        { "ice", "얼음" }, { "fighting", "격투" }, { "poison", "독" }, { "ground", "땅" }, { "flying", "비행" },
//        { "psychic", "에스퍼" }, { "bug", "벌레" }, { "rock", "바위" }, { "ghost", "고스트" }, { "dragon", "드래곤" },
//        { "dark", "악" }, { "steel", "강철" }, { "fairy", "페어리" }
//    };
//
//    public static string GetKoreanTypes(List<TypeWrapper> types)
//    {
//        List<string> translatedTypes = types.ConvertAll(t =>
//            typeTranslations.ContainsKey(t.type.name) ? typeTranslations[t.type.name] : t.type.name
//        );
//        return string.Join(", ", translatedTypes);
//    }
//
//    public static string GetKoreanName(PokemonSpeciesData species)
//    {
//        foreach (var nameEntry in species.names)
//        {
//            if (nameEntry.language.name == "ko")
//            {
//                return nameEntry.name;
//            }
//        }
//        return "알 수 없음";
//    }
//}
