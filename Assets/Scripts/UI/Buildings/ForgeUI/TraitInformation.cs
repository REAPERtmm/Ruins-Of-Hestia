using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class TraitInformation : MonoBehaviour
{
    public TMP_Text TraitName;
    public TMP_Text TraitValue;
    public bool IsLocked = false; 
    public Image LockImageTrue;
    public Image LockImageFalse; 

    public void Lock()
    {
        IsLocked = !IsLocked;

        if (IsLocked)
        {
            LockImageTrue.gameObject.SetActive(true);
            LockImageFalse.gameObject.SetActive(false);
        }
        else
        {
            LockImageFalse.gameObject.SetActive(true);
            LockImageTrue.gameObject.SetActive(false);
        }
    }
}
