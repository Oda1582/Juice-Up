using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIsScript : MonoBehaviour
{
    public GameObject _UIComboAnim, _UIComboInfo;

    public GameObject ComboAnchor;

    public TMP_Text _UICombo, _UITimer;

    PlayerController refplayer;

    public bool isCombo = false, IsDone = true;
    public bool StartTimer;

    // Attributs 
    public float CurrentTime;
    public float StartingTime;
    public float secondes; 
    public float centiemes;

    public Color32 Color1, Color2, Color3, Color4, Color5, Color6, Color7, Color8, Color9, Color10, ColorSec1, ColorSec2, ColorSec3;

    // Start is called before the first frame update
    void Start()
    {
        refplayer = GameObject.Find("Player").GetComponent<PlayerController>();
        StartingTime = refplayer.TimeLoose;
        CurrentTime = StartingTime; 
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    public void FixedUpdate()
    {
        
        if (StartTimer == true){
        
        // Incrémentation du chronomètre
        CurrentTime -= 1 * Time.fixedDeltaTime;
        
        // Arrondissement a 2 chiffres apres la virgule
        CurrentTime = (Mathf.Floor(CurrentTime * 100f))/100;

        // Formatage
        secondes = Mathf.Floor(CurrentTime);
        centiemes = Mathf.Floor((CurrentTime - secondes) * 100f);
        secondes.ToString();
        centiemes.ToString();
        string niceTime = string.Format("{0:0},{1:00}", secondes, centiemes);
        // Debug.Log("Current Time : " + CurrentTime);
        // Debug.Log("Secondes :" + secondes);
        // Debug.Log("Centièmes : " + centiemes);
        //Debug.Log(isCombo);

        // Affichage dans l'UI (et gestion du "0")
        _UITimer.text = niceTime;

        if (isCombo == true && CurrentTime <= 0.01f){
            // ResetTimerDecrement();
            //Ne pas mettre de reset ici car ça empêche la décrémentation quand plusieurs combo, ou sinon foutre un boolean
            //CurrentTime = 0;
            //ResetTimer();
            //DeactivateComboUI();
        } 
        else if (isCombo == false && CurrentTime <= 0f)
        {
            // ComboAnimStart("-1");
            ResetTimer();
            DeactivateComboUI();
            refplayer.ResetCombo();
        }
        ChangeComboTextColor();
        ChangeComboTimerTextColor();
        }
    }

    public void ComboAnimStart(string Text)
    {
        GameObject newComboAnim = Instantiate(_UIComboAnim);
        TMP_Text newComboAnimText = newComboAnim.GetComponent<TMPro.TextMeshProUGUI>();
        newComboAnim.transform.SetParent(ComboAnchor.transform, false);
        newComboAnim.transform.position = ComboAnchor.transform.position;
        newComboAnim.SetActive(true);
        newComboAnimText.text = (Text);
    }

    public void SetCombo(int NbrCombo)
    {
        _UICombo.text = "X" + NbrCombo.ToString(); 
    }

    public void ActivateComboUI()
    {
        _UIComboInfo.SetActive(true);
    }

        public void DeactivateComboUI()
    {
        // ComboAnimStart("-1");
        _UIComboInfo.SetActive(false);
    }

    public void StartCount()
    {
        StartingTime = refplayer.TimeLoose;
        CurrentTime = StartingTime;  
        StartTimer = true;
        IsDone = false;
    }

    public void ResetTimer()
    {
        StartingTime = refplayer.TimeLoose;
        CurrentTime = StartingTime;        
        StartTimer = false;
    }

    public void ChangeComboTextColor()
    {
        switch (refplayer.EnemyKilledCount)
        {
            case 3:
                _UICombo.color = Color1;
                refplayer.UpgradeBulletsLVL1();
                break;
            case 6:
                _UICombo.color = Color2;
                refplayer.UpgradeBulletsLVL2();
                break;
            case 11:
                _UICombo.color = Color3;
                refplayer.UpgradeBulletsLVL3();
                break;
            case 20:
                _UICombo.color = Color4;
                break;
            case 25:
                _UICombo.color = Color5;
                break;
            case 30:
                _UICombo.color = Color6;
                break;
            case 35:
                _UICombo.color = Color7;
                break;
            case 40:
                _UICombo.color = Color8;
                break;
            case 45:
                _UICombo.color = Color9;
                break;
            case 50:
                _UICombo.color = Color10;
                break;
            default:
                // Handle other cases or set a default color
                break;
        }
    }

    public void ChangeComboTimerTextColor()
    {
        if(secondes < 10 && secondes >= 6 )
        {
            _UITimer.color = ColorSec1;
        }
        else if (secondes < 6 && secondes >= 3 )
        {
            _UITimer.color = ColorSec2;
        }
        else if (secondes < 3 && secondes >= 0 )
        {
            _UITimer.color = ColorSec3;
        }
    }
}
