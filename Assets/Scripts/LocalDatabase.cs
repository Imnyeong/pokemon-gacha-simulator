using UnityEngine;
using System.Collections.Generic;

public class LocalDatabase : MonoBehaviour
{
    public static LocalDatabase Instance;

    public List<PokemonData> pokedex = new List<PokemonData>();

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
}
