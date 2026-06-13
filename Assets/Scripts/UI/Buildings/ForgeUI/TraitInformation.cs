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
    public Image LockImageTrue;
    public Image LockImageFalse;

    private TraitInstance trait;

    public void Initialize(TraitInstance traitInstance)
    {
        trait = traitInstance;

        TraitName.text = trait.Stat.ToString();
        TraitValue.text = trait.Value.ToString("F2");
        
        switch(traitInstance.Type)
        {
            case TraitType.Flat:
                TraitValue.text = trait.Value.ToString("F2");
                break;
            case TraitType.Percent:
                TraitValue.text = (trait.Value).ToString("F2") + "%";
                break;
        }

        UpdateLockVisual();
    }

    public void Lock()
    {
        trait.IsLocked = !trait.IsLocked;
        UpdateLockVisual();
    }

    private void UpdateLockVisual()
    {
        LockImageTrue.gameObject.SetActive(trait.IsLocked);
        LockImageFalse.gameObject.SetActive(!trait.IsLocked);
    }
}
