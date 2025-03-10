using System.Collections.Generic;
using UnityEngine;

public class GachaController : MonoBehaviour
{
    public GachaView view;
    private const int TOTAL_POKEMONS = 1008;

    private void Start()
    {
        view.SetController(this);
    }

    public void OnGachaButtonClicked()
    {
        FetchPokemonData();
    }

    private void FetchPokemonData()
    {
        int randomPokemonId = Random.Range(1, TOTAL_POKEMONS);

        PokemonData pokemon = LocalDatabase.Instance.pokedex[randomPokemonId];
        view.UpdatePokemonUI(pokemon);
        SaveCaughtPokemon(pokemon);
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