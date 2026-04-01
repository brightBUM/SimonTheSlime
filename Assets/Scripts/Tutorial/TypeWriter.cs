using System.Collections;
using TMPro;
using UnityEngine;

public class TypeWriter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI textField;
    [SerializeField] GameObject canvas;

    [Header("Typing Settings")]
    [TextArea(0,100)]
    [SerializeField] string message;
    [SerializeField] private float charactersPerSecond = 30f;

    private Coroutine typingCoroutine;
    private string fullText;
    private bool isTyping;

    private void Start()
    {
        StartTyping(message);
    }
    public void StartTyping(string newText)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        fullText = newText;
        typingCoroutine = StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        textField.text = "";

        float delay = 1f / charactersPerSecond;

        foreach (char letter in fullText)
        {
            textField.text += letter;
            yield return new WaitForSeconds(delay);
        }

        isTyping = false;

       
    }

    public void SkipTyping()
    {
        if (!isTyping) return;

        StopCoroutine(typingCoroutine);
        textField.text = fullText;
        isTyping = false;
    }

    public void SetSpeed(float newSpeed)
    {
        charactersPerSecond = Mathf.Max(1f, newSpeed);
    }

    public bool IsTyping()
    {
        return isTyping;
    }
}
