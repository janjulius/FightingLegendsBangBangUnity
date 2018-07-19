using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new character", menuName = "Infos/Character info", order = 1)]
public class CharacterInfo : ScriptableObject
{
    public string CharacterName;
    public Sprite CharacterHead;
    public string CharacterPrefab;
    public bool CharacterDisabled;

}
