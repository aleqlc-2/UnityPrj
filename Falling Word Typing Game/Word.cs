using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable] // MonoBehaviour 상속받지 않을때 인스펙터창에 노출시키기위함
public class Word
{
    public string word;
    private int typeIndex;

    WordDisplay display; //private

    public Word(string _word, WordDisplay _display)
    {
        word = _word;
        typeIndex = 0;

        display = _display;
        display.SetWord(word);
    }

    public char GetNextLetter() // char반환
    {
        return word[typeIndex];
    }

    public void TypeLetter()
    {
        typeIndex++;
        display.RemoveLetter(); // 한글자씩 삭제
    }

    public bool WordTyped()
    {
        bool wordTyped = (typeIndex == word.Length); // 모두 타이핑 했을때
        if (wordTyped)
        {
            display.RemoveWord(); // 단어 Destroy
        }
        return wordTyped;
    }
}
