### POKEMON GACHA SIMULATOR

## 개요 📝
Open API인 poke API의 데이터를 활용하여 뽑기 시스템과 도감 수집 기능을 구현한 시뮬레이터

## Tech Stack ✏️
- Unity
- C#
- Visual Studio
- Sourcetree

## 기술 🔎
- UnityWebRequest로 API호출
- API에서 받아온 데이터를 json 파일로 변환
- Task를 활용하여 API 병렬 호출
- Dictionary를 활용한 번역 시스템 (영어 Key와 한글 Value)
- PlayerPrefs로 단말기에 정보 저장, 불러오기 구현


## Script로 보는 핵심 기능

### Json 파일로 프로젝트에 저장
```ruby
if (File.Exists(localDataFilePath))
{
    string jsonData = File.ReadAllText(localDataFilePath);
    LocalDatabase.Instance.pokedex = JsonConvert.DeserializeObject<List<PokemonData>>(jsonData);
}
else
{
    await DownloadAndSavePokedexDataAsync();
}
```
한 번 내려받은 데이터는 Json 파일로 저장 되어있기 때문에 API 호출 없이 로컬 정보로 불러오고, 최초 실행 시에는 경로에 Json 파일이 없기 때문에 API를 호출합니다.

### API 병렬 처리
```ruby
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
```

MAX_CONCURRENT_REQUESTS 만큼 Task를 추가하고 한 번에 병렬로 Task를 처리합니다. API 응답 대기시간을 줄일 수 있어 데이터를 불러오는 시간이 줄어듭니다.

### Dictionary를 활용한 번역 기능
```ruby
private static readonly Dictionary<string, string> TypeTranslations = new Dictionary<string, string>
{
    { "normal", "노말" }, { "fire", "불꽃" }, { "water", "물" }, { "electric", "전기" },
    ...
    { "steel", "강철" }, { "fairy", "페어리" }
};

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
```

pokeAPI에서 Sprite 정보와 한국어 이름 정보를 둘 다 가져오려면 한 포켓몬 당 API를 2번 호출해야하고 타입 정보의 경우 고정된 정보이기 때문에 위와 같은 방법으로 구현했습니다.