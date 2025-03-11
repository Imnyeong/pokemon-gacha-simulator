using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GachaView : MonoBehaviour
{
    public Image pokemonImage;
    public Text pokemonNameText;
    public Text pokemonTypeText;
    public Button gachaButton;
    private GachaController controller;

    public void SetController(GachaController _controller)
    {
        controller = _controller;
        gachaButton.onClick.AddListener(controller.OnGachaButtonClicked);
    }
    public void UpdatePokemonUI(PokemonData pokemon)
    {
        pokemonNameText.text = pokemon.name;
        pokemonTypeText.text = string.Join(", ", pokemon.types);
        StartCoroutine(LoadSprite(pokemon.sprite, pokemonImage));
    }
    private IEnumerator LoadSprite(string url, Image image)
    {
        using (UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityEngine.Networking.UnityWebRequest.Result.Success)
            {
                Texture2D texture = ((UnityEngine.Networking.DownloadHandlerTexture)request.downloadHandler).texture;
                image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
            else
            {
                Debug.LogError("Failed to load sprite: " + request.error);
            }
        }
    }
}
