﻿using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class PokedexUI : MonoBehaviour
{
    public GameObject pokemonCardPrefab;  
    public Transform content;             
    public ScrollRect scrollRect;        

    private int currentBatchIndex = 0;    
    private const int batchSize = 50;   

    void Start()
    {
        scrollRect.onValueChanged.AddListener(OnScrollValueChanged);
        StartCoroutine(LoadPokedexUI());
    }
    public void OnClickButton()
    {
        gameObject.SetActive(!gameObject.activeSelf);
        LocalDatabase.Instance.RefreshCaughtPokemon();
    }
    IEnumerator LoadPokedexUI()
    {
        while (LocalDatabase.Instance.pokedex.Count == 0)
        {
            yield return null;
        }
        LoadPokemonBatch(currentBatchIndex, batchSize);
    }
    void OnScrollValueChanged(Vector2 scrollPos)
    {
        if (scrollPos.y <= 0.1f) 
        {
            currentBatchIndex += batchSize; 
            LoadPokemonBatch(currentBatchIndex, batchSize);
        }
    }
    void LoadPokemonBatch(int startIndex, int count)
    {
        int endIndex = Mathf.Min(startIndex + count, LocalDatabase.Instance.pokedex.Count);
        for (int i = startIndex; i < endIndex; i++)
        {
            CreatePokemonCard(LocalDatabase.Instance.pokedex[i]);
        }
    }
    void CreatePokemonCard(PokemonData pokemon)
    {
        GameObject card = Instantiate(pokemonCardPrefab, content);
        LoadSpriteAsync(card.transform, pokemon.sprite, pokemon.id);
    }
    async void LoadSpriteAsync(Transform cardTransform, string spriteUrl, int id)
    {
        Texture2D texture = await DownloadTextureAsync(spriteUrl);
        if (texture != null)
        {
            Image image = cardTransform.GetComponentsInChildren<Image>()[1];
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.color = LocalDatabase.Instance.caughtPokemon.Contains(id) ? Color.white : Color.black;
            Text text = cardTransform.GetComponentInChildren<Text>();
            text.text = LocalDatabase.Instance.caughtPokemon.Contains(id) ? LocalDatabase.Instance.GetPokemon(id - 1).name : "???";
        }
        else
        {
            Debug.LogError($"스프라이트 로딩 실패: {spriteUrl}");
        }
    }
    async Task<Texture2D> DownloadTextureAsync(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.result == UnityWebRequest.Result.Success)
            {
                return ((DownloadHandlerTexture)request.downloadHandler).texture;
            }
            else
            {
                Debug.LogError($"텍스처 다운로드 실패: {request.error}");
                return null;
            }
        }
    }
}
