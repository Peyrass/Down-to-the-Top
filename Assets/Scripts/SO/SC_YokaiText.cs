using UnityEngine;

[CreateAssetMenu (fileName = "SO_Dialog", menuName = "SO/YokaiTexts")]
public class SC_YokaiText : ScriptableObject
{
   [TextArea(4, 6)] public string[] dialog;
}