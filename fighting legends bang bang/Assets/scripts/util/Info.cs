using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info : MonoBehaviour
{

    [Header("The character string are private check the script")]
    private string[] CharacterInformation =
    {
        "No information available for this character.",
        "the Yeti",
        "the Topless Tree",
        "the Nazi Goblin Leader",
        "the Panda",
        "",
        "the Raccoon",
        "the Snowman",
        "",
        "the Rugby God",
        ""
    };
    private string[] SpecialAttackInformation =
    {
        "This character currently has no special attack or this text is not updated.",

        "Berend gets furious, for 4 seconds Berend will be more vulnerable but deal more damage, attack faster, jump higher and take no knockback.",

        "Boom Stronk gets heavy, slamming down onto the ground if Boom Stronk " +
        "hits an enemy he will deal damage based on distance dropped, if not and he hits the ground he will regain some of his health.",

        "Fred remember his stolen banana's and slams the ground dealing 75 damage to all enemies in an area around him blasting them away.",

        "Jens summons a cannonball on a random player " +
        "(including himself) the cannonball will fall down until it reaches the destination creating an explosion which deals " +
        "100 damage in an area additionally the cannonball will hit anyone on its path dealing 50 damage.",

        "",

        "Rocky vanishes into the shadows jumping onto his closest enemy dealing three swift slashes and then blasting them away dealing a total of 20 * 3 + 25 damage.",

        "Willem turns into a big snowball for 2.5 seconds, rolling forward destroying everything on its path dealing 150 damage you can reactivate the special to get out of the snowball early",

        "",

        "Tjeerd can mark enemies for 2.5 seconds by walking on them and being enable to jump for the duration, after this time is over all marked enemies will take damage",
        "",
        ""
    };

    public string GetCharacterInformation(int i)
    {
        if (i < CharacterInformation.Length)
        {
            if (CharacterInformation[i] == "")
            {
                return CharacterInformation[0];
            }
            return CharacterInformation[i];
        }
        return CharacterInformation[0];
    }

    public string GetSpecialAttackInformation(int i)
    {
        if (i < SpecialAttackInformation.Length)
        {
            if (SpecialAttackInformation[i] == "")
            {
                return SpecialAttackInformation[0];
            }
            return SpecialAttackInformation[i];
        }
        return SpecialAttackInformation[0];
    }

}
