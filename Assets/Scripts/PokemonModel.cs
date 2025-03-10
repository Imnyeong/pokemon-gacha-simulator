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
//        { "normal", "�븻" }, { "fire", "�Ҳ�" }, { "water", "��" }, { "electric", "����" }, { "grass", "Ǯ" },
//        { "ice", "����" }, { "fighting", "����" }, { "poison", "��" }, { "ground", "��" }, { "flying", "����" },
//        { "psychic", "������" }, { "bug", "����" }, { "rock", "����" }, { "ghost", "��Ʈ" }, { "dragon", "�巡��" },
//        { "dark", "��" }, { "steel", "��ö" }, { "fairy", "��" }
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
//        return "�� �� ����";
//    }
//}
