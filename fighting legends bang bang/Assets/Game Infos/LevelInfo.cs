using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new level", menuName = "Infos/Level info", order = 1)]
public class LevelInfo : ScriptableObject
{
    public string LevelName;
    public string LevelFileName;
    public Sprite LevelImage;
    public bool LevelDisabled;

}
