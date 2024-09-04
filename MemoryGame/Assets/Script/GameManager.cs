using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    private const string SaveFileName = "game_save.json";
    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Text scoreLabel;  
    private int score;  
    public static GameManager Instance;
    public static int gridSize;
    
    [SerializeField]
    private GameObject prefab;
   
    [SerializeField]
    private GameObject Card_List;
   
    [SerializeField]
    private Sprite Card_Back;
   
    [SerializeField]
    private Sprite[] sprites;
   
    private Card[] cards;

  
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private GameObject info;
   
    [SerializeField]
    private Card LoadedSprites;
  
    
    //private Text sizeLabel;
   /* [SerializeField]
    private Slider sizeSlider;*/
    [SerializeField]
    private Text Time_Label;
    private float time;

    private int Selected_Sprites;
    private int Selected_Card;
    private int Cards_Left;
    private bool IsGameStart;
    
    private int consecutiveGuesses = 0;
    

    void Awake()
    {
        Instance = this;
        ResetSaveFile();
    }
    void Start()
    {
        IsGameStart = false;
        panel.SetActive(false);
        score = 0;
        UpdateScoreLabel();
        CheckForSavedGame();
    }

    public void SetGameSize3x3()
    {
        gridSize = 3;
        StartNewGame();
    }

    public void SetGameSize4x4()
    {
        gridSize = 4;
        StartNewGame();
    }

    public void SetGameSize5x5()
    {
        gridSize = 5;
        StartNewGame();
    }


    public void StartNewGame()
    {
        IsGameStart = false;
        time = 0;
        StartCardGame();
        Debug.Log(gridSize + "x" + gridSize + " Game Started!");
    }


    private void CheckForSavedGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        
        if (File.Exists(filePath))
        {
          
            try
            {
                string json = File.ReadAllText(filePath);
                GameState gameState = JsonUtility.FromJson<GameState>(json);

               
                if (gameState == null || gameState.cards == null || gameState.cards.Count == 0)
                {
                    DeactivateResumeButton();
                }
                else
                {
                    ActivateResumeButton();
                }
            }
            catch
            {
               
                DeactivateResumeButton();
            }
        }
        else
        {
           
            DeactivateResumeButton();
        }
    }
    
    private void ActivateResumeButton()
    {
        if (resumeButton != null)
        {
            resumeButton.interactable = true;
        }
    }

   
    private void DeactivateResumeButton()
    {
        if (resumeButton != null)
        {
            resumeButton.interactable = false;
        }
    }
    

    private void UpdateScoreLabel()
    {
        scoreLabel.text = "Score: " + score;
    }


    private void PreloadCardImage()
    {
        for (int i = 0; i < sprites.Length; i++)
            LoadedSprites.SpriteID = i;
        LoadedSprites.gameObject.SetActive(false);
    }
   
    public void StartCardGame()
    {

        if (IsGameStart) return;
        IsGameStart = true;

        panel.SetActive(true);
        info.SetActive(false);

        SetGamePanel();

        Selected_Card = Selected_Sprites = -1;
        Cards_Left = cards.Length;

        SpriteCardAllocation();
        StartCoroutine(HideFace());
        time = 0;
        consecutiveGuesses = 0;
    }

    
    private void SetGamePanel(){
      
        int isOdd = gridSize % 2 ;

        cards = new Card[gridSize * gridSize - isOdd];
      
        foreach (Transform child in Card_List.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
       
        RectTransform panelsize = panel.transform.GetComponent(typeof(RectTransform)) as RectTransform;
        float row_size = panelsize.sizeDelta.x;
        float col_size = panelsize.sizeDelta.y;
        float scale = 1.0f/gridSize;
        float xInc = row_size/gridSize;
        float yInc = col_size/gridSize;
        float curX = -xInc * (float)(gridSize / 2);
        float curY = -yInc * (float)(gridSize / 2);

        if(isOdd == 0) {
            curX += xInc / 2;
            curY += yInc / 2;
        }
        float initialX = curX;
       
        for (int i = 0; i < gridSize; i++)
        {
            curX = initialX;
           
            for (int j = 0; j < gridSize; j++)
            {
                GameObject c;
              
                if (isOdd == 1 && i == (gridSize - 1) && j == (gridSize - 1))
                {
                    int index = gridSize / 2 * gridSize + gridSize / 2;
                    c = cards[index].gameObject;
                }
                else
                {
                   
                    c = Instantiate(prefab);
                   
                    c.transform.SetParent(Card_List.transform);

                    int index = i * gridSize + j;
                    cards[index] = c.GetComponent<Card>();
                    cards[index].ID = index;
                  
                    c.transform.localScale = new Vector3(scale, scale);
                }
                
                c.transform.localPosition = new Vector3(curX, curY, 0);
                curX += xInc;

            }
            curY += yInc;
        }

    }
    
    void ResetFace()
    {
        for (int i = 0; i < gridSize; i++)
            cards[i].ResetRotation();
    }
   
    IEnumerator HideFace()
    {
      
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < cards.Length; i++)
            cards[i].Flip();
        yield return new WaitForSeconds(0.5f);
    }
   
    private void SpriteCardAllocation()
    {
        int i, j;
        int[] selectedID = new int[cards.Length / 2];
      
        for (i = 0; i < cards.Length/2; i++)
        {
           
            int value = Random.Range(0, sprites.Length - 1);
           
            for (j = i; j > 0; j--)
            {
                if (selectedID[j - 1] == value)
                    value = (value + 1) % sprites.Length;
            }
            selectedID[i] = value;
        }

      
        for (i = 0; i < cards.Length; i++)
        {
            cards[i].Active();
            cards[i].SpriteID = -1;
            cards[i].ResetRotation();
        }
     
        for (i = 0; i < cards.Length / 2; i++)
            for (j = 0; j < 2; j++)
            {
                int value = Random.Range(0, cards.Length - 1);
                while (cards[value].SpriteID != -1)
                    value = (value + 1) % cards.Length;

                cards[value].SpriteID = selectedID[i];
            }

    }
   
   /* public void SetGameSize() {
        gridSize = (int)sizeSlider.value;
        sizeLabel.text = gridSize + " X " + gridSize;
    }*/
   
    public Sprite GetSprite(int spriteId)
    {
        return sprites[spriteId];
    }
   
    public Sprite CardBack()
    {
        return Card_Back;
    }
   
    public bool canClick()
    {
        if (!IsGameStart)
            return false;
        return true;
    }
   
    public void cardClicked(int spriteId, int cardId)
    {
       
        if (Selected_Sprites == -1)
        {
            Selected_Sprites = spriteId;
            Selected_Card = cardId;
        }
        else
        {
            if (Selected_Sprites == spriteId)
            {
                
                cards[Selected_Card].Inactive();
                cards[cardId].Inactive();
                Cards_Left -= 2;
                consecutiveGuesses++;
                if (consecutiveGuesses >= 1)
                {
                    
                    AwardBonusPoints();
                }
                AddScore(10); 
                CheckGameWin();
            }
            else
            {
               
                cards[Selected_Card].Flip();
                cards[cardId].Flip();
               AddScore(-5);
                consecutiveGuesses = 0;
            }
            Selected_Card = Selected_Sprites = -1;
        }
    }
    private void AddScore(int points)
    {
        score += points;
        UpdateScoreLabel();
    }
    
    private void CheckGameWin()
    {
       
        if (Cards_Left == 0)
        {
            EndGame();
            AudioPlayer.Instance.Play(1);
        }
    }
   
    private void EndGame()
    {
        IsGameStart = false;
        panel.SetActive(false);
    }
   
    public void DisplayInfo(bool i)
    {
        info.SetActive(i);
    }
   
    private void Update(){
        if (IsGameStart) {
            time += Time.deltaTime;
            Time_Label.text = "Time: " + time + "s";
        }
    }
    
    public void SaveGame()
    {
        GameState gameState = new GameState
        {
            gameSize = gridSize,
            time = time,
            score = score,
            cards = new List<CardData>()
        };

        foreach (Card card in cards)
        {
            CardData cardData = new CardData
            {
                spriteID = card.SpriteID,
                flipped = card.GetComponent<Card>().Isflipped,
                inactive = card.GetComponent<Card>().img.color.a == 0 
            };
            gameState.cards.Add(cardData);
        }

        string json = JsonUtility.ToJson(gameState, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SaveFileName), json);
        Debug.Log("Game Saved!");
    }

    public void LoadGame()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            GameState gameState = JsonUtility.FromJson<GameState>(json);

            gridSize = gameState.gameSize;
            time = gameState.time;
            score = gameState.score;

            StartCardGame(); // Set up the card game (this includes resetting cards)

            for (int i = 0; i < cards.Length; i++)
            {
                CardData cardData = gameState.cards[i];
                cards[i].SpriteID = cardData.spriteID;

                // Reset card's visibility and interaction
                cards[i].ResetRotation();
                cards[i].Active(); // Ensure card is set to its active state for possible interaction

                if (cardData.inactive)
                {
                    // If the card was inactive, make it unclickable and invisible
                    cards[i].Inactive();
                }
                else if (cardData.flipped)
                {
                    // If the card was Isflipped but not inactive, flip it to show the front
                    cards[i].Flip();
                }
                else
                {
                    // If the card was neither Isflipped nor inactive, ensure it shows the card back
                    cards[i].ResetRotation();
                }
            }

            UpdateScoreLabel(); // Update the score display
            Debug.Log("Game Loaded!");
        }
        else
        {
            Debug.LogWarning("No save file found!");
        }
    }



    public void NewGame()
    {
        IsGameStart = false;
        time = 0;
        StartCardGame();
        Debug.Log("New Game Started!");
    }
    
    public void QuitGame()
    {
        SaveGame();
        Debug.Log("Game Saved on Quit");
        Application.Quit(); 
        
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    public void ResetSaveFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, SaveFileName);

        if (File.Exists(filePath))
        {
            try
            {
                
                File.Delete(filePath);
                Debug.Log("Save file reset successfully.");
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error resetting save file: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("No save file to reset.");
        }

        
        DeactivateResumeButton();
    }
    
    private void AwardBonusPoints()
    {
        int bonusPoints = 10; 
        score += bonusPoints;
        UpdateScoreLabel();
    }
    
}
