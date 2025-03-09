using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;

public class PokeAPIDownloader : MonoBehaviour
{
    private string localDataFilePath = "pokedexData.json";
    private const int TOTAL_POKEMONS = 1008;

    public Slider progressBar;
    public Text progressText;

    void Start()
    {
        if (File.Exists(localDataFilePath))
        {
            string jsonData = File.ReadAllText(localDataFilePath);
            LocalDatabase.Instance.pokedex = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData);
        }
        else
        {
            StartCoroutine(DownloadAndSavePokedexData());
        }
    }

    IEnumerator DownloadAndSavePokedexData()
    {
        List<PokemonData> pokemonList = new List<PokemonData>();

        for (int i = 1; i <= TOTAL_POKEMONS; i++)
        {
            string url = $"https://pokeapi.co/api/v2/pokemon/{i}";
            using (UnityWebRequest request = UnityWebRequest.Get(url))
            {
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    string json = request.downloadHandler.text;
                    PokemonApiResponse apiResponse = JsonConvert.DeserializeObject<PokemonApiResponse>(json);

                    List<string> types = new List<string>();
                    foreach (var type in apiResponse.types)
                    {
                        types.Add(type.type.name);
                    }

                    PokemonData pokemon = new PokemonData
                    {
                        id = i,
                        name = apiResponse.name,
                        types = types,
                        sprite = apiResponse.sprites.front_default
                    };

                    pokemonList.Add(pokemon);
                    progressBar.value = (float)i / TOTAL_POKEMONS;
                    progressText.text = $"{((float)i / TOTAL_POKEMONS * 100).ToString("F2")}%({i}/{TOTAL_POKEMONS})";
                }
                else
                {
                    Debug.LogError($"❌ 포켓몬 {i} 데이터 다운로드 실패: {request.error}");
                }
                yield return new WaitForSeconds(0.5f);
            }
        }
        pokemonList.Sort((a, b) => a.id.CompareTo(b.id));
        string saveJson = JsonConvert.SerializeObject(pokemonList, Formatting.Indented);
        File.WriteAllText(localDataFilePath, saveJson);

        LocalDatabase.Instance.pokedex = pokemonList;
    }
}

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
