using UnityEngine;
using System.Collections.Generic;

public class LocalDatabase : MonoBehaviour
{
    public static LocalDatabase Instance;

    public List<PokemonData> pokedex = new List<PokemonData>();
    public HashSet<int> caughtPokemon = new HashSet<int>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public PokemonData GetPokemon(int id)
    {
        return pokedex[id];
    }

    public void RefreshCaughtPokemon()
    {
        string caughtData = PlayerPrefs.GetString("CaughtPokemon", "");
        if (!string.IsNullOrEmpty(caughtData))
        {
            foreach (var id in caughtData.Split(','))
            {
                if (int.TryParse(id, out int parsedId))
                    caughtPokemon.Add(parsedId);
            }
        }
    }
}
