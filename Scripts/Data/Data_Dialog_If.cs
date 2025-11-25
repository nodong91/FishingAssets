using UnityEngine;

[CreateAssetMenu(fileName = "Data_Dialog_If", menuName = "Scriptable Objects/Data_Dialog_If")]
public class Data_Dialog_If : ScriptableObject
{
    public enum IfType
    {
        Loan = 0,

    }
    public IfType ifType;
    public Data_Dialog onDataDialog;
    public Data_Dialog offDataDialog;
}
