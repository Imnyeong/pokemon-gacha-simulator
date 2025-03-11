using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GachaManager : MonoBehaviour
{
    public Image pokemonImage;
    public Text pokemonNameText;
    public Text pokemonTypeText;
    public Button gachaButton;

    private const int TOTAL_POKEMONS = 1008;

    private void Start()
    {
        gachaButton.onClick.AddListener(OnGachaButtonClicked);
    }

    private void OnGachaButtonClicked()
    {
        FetchPokemonData();
        StartCoroutine(WaitButton());
    }
    private IEnumerator WaitButton()
    {
        gachaButton.interactable = false;
        yield return new WaitForSecondsRealtime(2.0f);
        gachaButton.interactable = true;
    }
    private void FetchPokemonData()
    {
        int randomPokemonId = Random.Range(1, TOTAL_POKEMONS);
        PokemonData pokemon = LocalDatabase.Instance.pokedex[randomPokemonId];
        UpdatePokemonUI(pokemon);
        SaveCaughtPokemon(pokemon);
    }

    private void UpdatePokemonUI(PokemonData pokemon)
    {
        pokemonNameText.text = pokemon.name;
        pokemonTypeText.text = string.Join(", ", pokemon.types);
        StartCoroutine(LoadSprite(pokemon.sprite, pokemonImage));
    }

    private IEnumerator LoadSprite(string url, Image image)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + request.error);
            }
        }
    }

    private void SaveCaughtPokemon(PokemonData pokemon)
    {
        HashSet<int> caughtPokemon = new HashSet<int>();
        string caughtData = PlayerPrefs.GetString("CaughtPokemon", "");
        if (!string.IsNullOrEmpty(caughtData))
        {
            foreach (var id in caughtData.Split(','))
            {
                if (int.TryParse(id, out int parsedId))
                    caughtPokemon.Add(parsedId);
            }
        }

        if (!caughtPokemon.Contains(pokemon.id))
        {
            caughtPokemon.Add(pokemon.id);
            PlayerPrefs.SetString("CaughtPokemon", string.Join(",", caughtPokemon));
            PlayerPrefs.Save();
            LocalDatabase.Instance.RefreshCaughtPokemon();
        }
    }
}
