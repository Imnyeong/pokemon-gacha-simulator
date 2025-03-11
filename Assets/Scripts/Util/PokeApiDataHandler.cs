using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class PokeApiDataHandler : MonoBehaviour
{
    private string localDataFilePath = "pokedexData.json";
    private const int TOTAL_POKEMONS = 1008;
    private const int MAX_CONCURRENT_REQUESTS = 10;

    private int currentData = 0;
    public Slider progressBar;
    public Text progressText;

    private static readonly Dictionary<string, string> TypeTranslations = new Dictionary<string, string>
    {
        { "normal", "노말" }, { "fire", "불꽃" }, { "water", "물" }, { "electric", "전기" },
        { "grass", "풀" }, { "ice", "얼음" }, { "fighting", "격투" }, { "poison", "독" },
        { "ground", "땅" }, { "flying", "비행" }, { "psychic", "에스퍼" }, { "bug", "벌레" },
        { "rock", "바위" }, { "ghost", "고스트" }, { "dragon", "드래곤" }, { "dark", "악" },
        { "steel", "강철" }, { "fairy", "페어리" }
    };

    async void Start()
    {
        if (File.Exists(localDataFilePath))
        {
            string jsonData = File.ReadAllText(localDataFilePath);
            LocalDatabase.Instance.pokedex = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData);
            SceneManager.LoadScene("InGame");
        }
        else
        {
            await DownloadAndSavePokedexDataAsync();
        }
    }
    private async Task DownloadAndSavePokedexDataAsync()
    {
        List<PokemonData> pokemonList = new List<PokemonData>();
        List<Task<PokemonData>> tasks = new List<Task<PokemonData>>();

        for (int i = 1; i <= TOTAL_POKEMONS; i++)
        {
            tasks.Add(GetPokemonDataAsync(i));

            if (tasks.Count >= MAX_CONCURRENT_REQUESTS)
            {
                PokemonData[] results = await Task.WhenAll(tasks);
                pokemonList.AddRange(results);
                tasks.Clear();
            }
        }

        if (tasks.Count > 0)
        {
            PokemonData[] results = await Task.WhenAll(tasks);
            pokemonList.AddRange(results);
        }

        pokemonList.Sort((a, b) => a.id.CompareTo(b.id));

        string saveJson = JsonConvert.SerializeObject(pokemonList, Formatting.Indented);
        File.WriteAllText(localDataFilePath, saveJson);

        LocalDatabase.Instance.pokedex = pokemonList;
        SceneManager.LoadScene("InGame");
    }
    private async Task<PokemonData> GetPokemonDataAsync(int id)
    {
        string url = $"https://pokeapi.co/api/v2/pokemon/{id}";
        string speciesUrl = $"https://pokeapi.co/api/v2/pokemon-species/{id}";

        string name = "";
        List<string> translatedTypes = new List<string>();
        string sprite = "";

        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PokemonApiResponse apiResponse = JsonConvert.DeserializeObject<PokemonApiResponse>(json);

                sprite = apiResponse.sprites.front_default;

                foreach (var type in apiResponse.types)
                {
                    if (TypeTranslations.TryGetValue(type.type.name, out string translatedType))
                    {
                        translatedTypes.Add(translatedType);
                    }
                    else
                    {
                        translatedTypes.Add(type.type.name);
                    }
                }
            }
            else
            {
                Debug.LogError($"포켓몬 {id} 데이터 다운로드 실패: {request.error}");
                return null;
            }
        }

        using (UnityWebRequest request = UnityWebRequest.Get(speciesUrl))
        {
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                PokemonSpeciesResponse speciesResponse = JsonConvert.DeserializeObject<PokemonSpeciesResponse>(json);

                foreach (var nameEntry in speciesResponse.names)
                {
                    if (nameEntry.language.name == "ko")
                    {
                        name = nameEntry.name;
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError($"포켓몬 {id} 한글 이름 다운로드 실패: {request.error}");
            }
        }
        currentData++;
        progressText.text = $"{((float)currentData / TOTAL_POKEMONS * 100).ToString("F2")}%({currentData}/{TOTAL_POKEMONS})";
        progressBar.value = (float)currentData / TOTAL_POKEMONS;

        return new PokemonData
        {
            id = id,
            name = name,
            types = translatedTypes,
            sprite = sprite
        };
    }
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
public class PokemonSpeciesResponse
{
    public List<PokemonName> names;
}

[System.Serializable]
public class PokemonName
{
    public string name;
    public Language language;
}

[System.Serializable]
public class Language
{
    public string name;
}