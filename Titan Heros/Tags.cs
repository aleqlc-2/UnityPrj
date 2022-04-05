using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagManager
{
    public const string PLYAER_TAG = "Player";
    public const string ENEMY_TAG = "Enemy";
    public const string BOSS_TAG = "Boss";
}

public class AxisManager
{
    public const string HORIZONTAL_AXIS = "Horizontal";
    public const string VERTICAL_AXIS = "Vertical";
}

public class AnimationTags
{
    public const string WALK_PARAMETER = "Walk";
    public const string RUN_PARAMETER = "Run";

    public const string NORMAL_ATTACK_1_TRIGGER = "NormalAttack1";
    public const string NORMAL_ATTACK_2_TRIGGER = "NormalAttack2";

    public const string SPECIAL_ATTACK_1_TRIGGER = "SpecialAttack1";
    public const string SPECIAL_ATTACK_2_TRIGGER = "SpecialAttack2";
    public const string SPECIAL_ATTACK_3_TRIGGER = "SpecialAttack3";

    public const string HIT_TRIGGER = "Hit";

    public const string DEAD_TRIGGER = "Dead";

    public const string SLIDE_IN_ANIMATION = "SlideIn";
    public const string CAMERA_2_ANIMATION = "Camera2";

    public const string COUNTDOWN_1_ANIMATION = "Countdown1";
    public const string COUNTDOWN_2_ANIMATION = "Countdown2";
    public const string COUNTDOWN_3_ANIMATION = "Countdown3";

    public const string IDLE_ANIMATION = "Idle";
}

public class SceneNames
{
    public const string MAIN_MENU = "MainMenu";
    public const string GAMEPLAY = "Gameplay";
}