using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordManager : MonoBehaviour
{
    public List<Word> words;

    public WordSpawner wordSpawner;

    private bool hasActiveWord;
    private Word activeWord;

    public void AddWord()
    {
        Word word = new Word(WordGenerator.GetRandomWord(), wordSpawner.SpawnWord());
        Debug.Log(word.word);

        words.Add(word);
    }

    public void TypeLetter(char letter)
    {
        if (hasActiveWord)
        {
            if (activeWord.GetNextLetter() == letter)
            {
                activeWord.TypeLetter();
            }
        }
        else
        {
            foreach (Word word in words) // 스폰된 모든 word를 가져와서
            {
                if (word.GetNextLetter() == letter) // 첫 글자가 타이핑 친 글자이면
                {
                    activeWord = word; // 그 단어를 activeWord에 넣고
                    hasActiveWord = true; // 다음 타이핑부터는 위의 if구문에 들어갈 수 있도록
                    word.TypeLetter(); // 첫 글자 삭제하고 빨갛게
                    break; // 한 단어가 활성화되는 순간 foreach에서 빠져나가 다른 단어가 활성화 되지않도록
                }
            }
        }

        // 활성화된 단어를 모두 타이핑 했을 때
        if (hasActiveWord && activeWord.WordTyped())
        {
            hasActiveWord = false; // 다음 단어부터 다시 위의 else구문부터 들어갈 수 있도록
            words.Remove(activeWord); // 모두 타이핑한 단어는 리스트에서 삭제
        }
    }
}
