using System.Collections;
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

    private void OnEnable()
    {
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
        StartCoroutine(LoadSprite(card.transform, pokemon.sprite, pokemon.id));
    }

    IEnumerator LoadSprite(Transform cardTransform, string spriteUrl, int id)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(spriteUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Image image = cardTransform.GetComponentsInChildren<Image>()[1];
            image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            image.color = LocalDatabase.Instance.caughtPokemon.Contains(id) ? Color.white : Color.black;
        }
        else
        {
            Debug.LogError("❌ 스프라이트 로딩 실패: " + request.error);
        }
    }
}
