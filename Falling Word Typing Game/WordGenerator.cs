using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WordGenerator : MonoBehaviour
{
    private static string[] wordList = { "sidewalk", "robin", "three", "protect", "periodic",
									"somber", "majestic", "jump", "pretty", "wound", "jazzy",
									"memory", "join", "crack", "grade", "boot", "cloudy", "sick",
									"mug", "hot", "tart", "dangerous", "mother", "rustic", "economic",
									"weird", "cut", "parallel", "wood", "encouraging", "interrupt",
									"guide", "long", "chief", "mom", "signal", "rely", "abortive",
									"hair", "representative", "earth", "grate", "proud", "feel",
									"hilarious", "addition", "silent", "play", "floor", "numerous",
									"friend", "pizzas", "building", "organic", "past", "mute", "unusual",
									"mellow", "analyse", "crate", "homely", "protest", "painstaking",
									"society", "head", "female", "eager", "heap", "dramatic", "present",
									"sin", "box", "pies", "awesome", "root", "available", "sleet", "wax",
									"boring", "smash", "anger", "tasty", "spare", "tray", "daffy", "scarce",
									"account", "spot", "thought", "distinct", "nimble", "practise", "cream",
									"ablaze", "thoughtless", "love", "verdict", "giant" };

	// 메서드에 static을 붙여 싱글턴 생성하지 않고도 호출할 수 있도록 함.
    public static string GetRandomWord()
    {
		int randomIndex = Random.Range(0, wordList.Length);
		string randomWord = wordList[randomIndex];

		return randomWord;
    }
}
