using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    [SerializeField] private Sprite bgImage;

    public Sprite[] puzzles;

    public List<Button> btns = new List<Button>();
    public List<Sprite> gamePuzzles = new List<Sprite>();

    private bool firstGuess, secondGuess;

    private int countGuesses;
    private int countCorrectGuesses;
    private int gameGuesses;
    private int firstGuessIndex, secondGuessIndex;

    private string firstGuessPuzzle, secondGuessPuzzle;

    private void Awake()
    {
        // Resources폴더의 Sprites폴더의 Candy
        // 스프라이트가 너무많아 일일이 넣기 힘들때 사용
        // 일일이 넣는게 최적화에는 좋음
        puzzles = Resources.LoadAll<Sprite>("Sprites/Candy");
    }

    // AddButtons 스크립트에서 Awake 함수에서 Instantiate했으므로 이후에 실행되는 Start에서 받아와야됨
    // 똑같이 Awake로 받으면 안받아짐
    void Start()
    {
        GetButtons();
        AddListeners();
        AddGamePuzzles();
        Shuffle(gamePuzzles);
        gameGuesses = gamePuzzles.Count / 2;
    }

    private void GetButtons()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("PuzzleButton"); // s

        for (int i = 0; i < objects.Length; i++)
        {
            btns.Add(objects[i].GetComponent<Button>());
            btns[i].image.sprite = bgImage; // 모든 버튼에 배경 이미지 할당
        }
    }

    // 같은 이미지가 2개씩 버튼에 할당되게 하기위해
    private void AddGamePuzzles()
    {
        int looper = btns.Count; // 8
        int index = 0;

        for (int i = 0; i < looper; i++)
        {
            if (index == looper / 2) // 4로 같을때
            {
                index = 0; // 다시 0번째부터
            }

            gamePuzzles.Add(puzzles[index]);
            index++;
        }
    }

    // 인스펙터창에서 일일이 버튼에 OnClick에 할당할필요없음
    private void AddListeners()
    {
        foreach (Button btn in btns)
        {
            btn.onClick.AddListener(() => PickupPuzzle());
        }
    }

    public void PickupPuzzle()
    {
        // 클릭한 버튼의 이름 출력
        string name = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(name);

        if (!firstGuess)
        {
            firstGuess = true;
            firstGuessIndex = int.Parse(name);
            firstGuessPuzzle = gamePuzzles[firstGuessIndex].name;

            // 내가 클릭한 버튼의 이미지의 sprite에 리스트에서 해당 인덱스의 sprite가져와서 할당
            btns[firstGuessIndex].image.sprite = gamePuzzles[firstGuessIndex];
        }
        else if (!secondGuess)
        {
            secondGuess = true;
            secondGuessIndex = int.Parse(name);
            secondGuessPuzzle = gamePuzzles[secondGuessIndex].name;

            // 내가 클릭한 버튼의 이미지의 sprite에 리스트에서 해당 인덱스의 sprite가져와서 할당
            btns[secondGuessIndex].image.sprite = gamePuzzles[secondGuessIndex];

            if (firstGuessPuzzle.Equals(secondGuessPuzzle)) // == 비교보다 성능좋음
            {
                Debug.Log("The Puzzles Match");
            }
            else
            {
                Debug.Log("The Puzzles Dont Match");
            }

            countGuesses++;

            StartCoroutine(CheckIfThePuzzlesMatch());
        }
    }

    private IEnumerator CheckIfThePuzzlesMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessPuzzle == secondGuessPuzzle) // 그림을 맞췄으면
        {
            yield return new WaitForSeconds(0.5f);

            btns[firstGuessIndex].interactable = false;
            btns[secondGuessIndex].interactable = false;

            // 알파가 0이므로 투명으로 안보임
            btns[firstGuessIndex].image.color = new Color(0, 0, 0, 0);
            btns[secondGuessIndex].image.color = new Color(0, 0, 0, 0);

            CheckIfTheGameIsFinished();
        }
        else // 못맞추면 이미지 리셋
        {
            yield return new WaitForSeconds(0.5f);

            btns[firstGuessIndex].image.sprite = bgImage;
            btns[secondGuessIndex].image.sprite = bgImage;
        }

        yield return new WaitForSeconds(0.5f);

        firstGuess = secondGuess = false;
    }

    private void CheckIfTheGameIsFinished()
    {
        countCorrectGuesses++;

        if (countCorrectGuesses == gameGuesses) // 4번 맞추면 클리어
        {
            Debug.Log("Game Finished");
            Debug.Log("It took you " + countGuesses + " many guess(es) to finish the game"); // 몇번만에 맞췄는지
        }
    }
    
    // 8번 랜덤으로 섞음
    private void Shuffle(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);

            Sprite temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
