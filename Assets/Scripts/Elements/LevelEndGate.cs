using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEndGate : MonoBehaviour
{
    [SerializeField] SpriteRenderer resetLight;
    [SerializeField] List<SpriteRenderer> lights;
    [SerializeField] List<ButtonPlatform> buttons;
    [SerializeField] float duration = 0.5f;
    [SerializeField] Transform gateTransform;

    int[] code = { 3, 4, 2, 1, 5 };
    List<int> currentCode = new List<int>();

    bool gateLocked = true;
    Coroutine puzzleCoroutine;
    private void OnEnable()
    {
        foreach (ButtonPlatform buttonPlatform in buttons)
        {
            buttonPlatform.ButtonPress += OnButtonPound;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    IEnumerator StartLightBlinkOrder()
    {

        while(true)
        {
            resetLight.color = Color.white;

            yield return new WaitForSeconds(duration);

            resetLight.color = Color.black;

            for (int i = 0; i < code.Length; i++)
            {
                lights[code[i] - 1].color = Color.green;
                SoundManager.Instance.PlaylightBlinkSFX();

                yield return new WaitForSeconds(duration);

                lights[code[i] - 1].color = Color.black;
            }
        }
        
    }
    private void ResetPuzzle()
    {
        StopCoroutine(puzzleCoroutine);
        //show -ve feedback red color

        foreach(SpriteRenderer light in lights)
        {
            light.color = Color.black;
        }

        currentCode.Clear();

        foreach(ButtonPlatform button in buttons)
        {
            button.Reset();
        }

        SoundManager.Instance.PlayResetPuzzleSFX();
        resetLight.DOColor(Color.red, 1f).SetLoops(1).OnComplete(() =>
        {
            puzzleCoroutine = StartCoroutine(StartLightBlinkOrder());

        });


    }
    private void OnButtonPound(int value)
    {
        if (!gateLocked)
            return;

        StopCoroutine(puzzleCoroutine);

        if (currentCode.Count == 0)
        {
            foreach (SpriteRenderer light in lights)
            {
                light.color = Color.black;
            }

            resetLight.color = Color.black;
        }


        lights[value - 1].color = Color.green;

        currentCode.Add(value);

        for (int i = 0; i < currentCode.Count; i++)
        {
            if (currentCode[i] != code[i])
            {
                //reset puzzle
                ResetPuzzle();
                return;
            }
        }

        //puzzle solved ,unlock gate

        UnlockGate();

    }

    private void UnlockGate()
    {
        if (code.Length == currentCode.Count)
        {
            resetLight.color = Color.green;

            foreach (SpriteRenderer light in lights)
            {
                light.color = Color.green;
            }

            Debug.Log("unlock gate");

            foreach (ButtonPlatform button in buttons)
            {
                button.Reset();
            }

            SoundManager.Instance.PlayGateUnlockSFx(true);
            gateTransform.DOMoveY(15f, 2f).OnComplete(() =>
            {
                SoundManager.Instance.PlayGateUnlockSFx(false);
            });
            gateLocked = false;
        }
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
           puzzleCoroutine = StartCoroutine(StartLightBlinkOrder());
        }
    }

    private void OnDisable()
    {
        foreach (ButtonPlatform buttonPlatform in buttons)
        {
            buttonPlatform.ButtonPress -= OnButtonPound;
        }
    }
}
