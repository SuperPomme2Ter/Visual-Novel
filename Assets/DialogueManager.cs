using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using Unity.VisualScripting;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public TextAsset m_inkFile;
    public GameObject m_textBox;
    public GameObject m_customButton;
    public GameObject m_optionPanel;
    public GameObject m_BG;
    public List<Sprite> m_listBG;
    public List<Sprite> m_listSprite;
    public List<GameObject> m_listCharPos;
    private GameObject m_charPosSelected=null;

    public bool isTalking = false;

    public static Story m_story;
    TextMeshProUGUI m_nametag;
    TextMeshProUGUI m_message;
    List<string> m_tags;
    static Choice m_choiceSelected;

    enum bg
    {
        test
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    void Start()
    {
        m_story = new Story(m_inkFile.text);
        m_nametag = m_textBox.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        m_message = m_textBox.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        m_tags = new List<string>();
        m_choiceSelected = null;
        m_nametag.text = "Phoenix";
    }

    public void NextDialogue()
    {
        //Is there more to the m_story?
        if (m_story.canContinue)
        {

            AdvanceDialogue();
            //Are there any choices?
            if (m_story.currentChoices.Count != 0)
            {
                StartCoroutine(ShowChoices());
            }
        }
        else
        {
            FinishDialogue();
        }
    }

    // Finished the Story (Dialogue)
    private void FinishDialogue()
    {
        Debug.Log("End of Dialogue!");
    }

    // Advance through the m_story 
    void AdvanceDialogue()
    {
        string currentSentence = m_story.Continue();
        StopAllCoroutines();
        if (ParseTags())
        {
            return;
        }
        StartCoroutine(TypeSentence(currentSentence));
    }
    public void ContinueDialogue()
    {
        string currentSentence = m_story.currentText;
        StopAllCoroutines();
        StartCoroutine(TypeSentence(currentSentence));
    }

    // Type out the sentence letter by letter and make character idle if they were talking
    public IEnumerator TypeSentence(string sentence)
    {
        m_message.text = "";
        foreach(char letter in sentence.ToCharArray())
        {
            m_message.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
        /*CharacterScript tempSpeaker = GameObject.FindObjectOfType<CharacterScript>();
        if(tempSpeaker.isTalking)
        {
            SetAnimation("idle");
        }
        yield return null;*/
    }

    // Create then show the choices on the screen until one got selected
    IEnumerator ShowChoices()
    {
        Debug.Log("There are choices need to be made here!");
        List<Choice> _choices = m_story.currentChoices;

        for (int i = 0; i < _choices.Count; i++)
        {
            GameObject temp = Instantiate(m_customButton, m_optionPanel.transform);
            temp.transform.GetChild(0).GetComponent<Text>().text = _choices[i].text;
            temp.AddComponent<Selectable>();
            temp.GetComponent<Selectable>().element = _choices[i];
            temp.GetComponent<Button>().onClick.AddListener(() => { temp.GetComponent<Selectable>().Decide(); });
        }

        m_optionPanel.SetActive(true);

        yield return new WaitUntil(() => { return m_choiceSelected != null; });

        AdvanceFromDecision();
    }

    // Tells the m_story which branch to go to
    public static void SetDecision(object element)
    {
        m_choiceSelected = (Choice)element;
        m_story.ChooseChoiceIndex(m_choiceSelected.index);
    }

    // After a choice was made, turn off the panel and advance from that choice
    void AdvanceFromDecision()
    {
        m_optionPanel.SetActive(false);
        for (int i = 0; i < m_optionPanel.transform.childCount; i++)
        {
            Destroy(m_optionPanel.transform.GetChild(i).gameObject);
        }
        m_choiceSelected = null; // Forgot to reset the m_choiceSelected. Otherwise, it would select an option without player intervention.
        AdvanceDialogue();
    }

    /*** Tag Parser ***/
    /// In Inky, you can use m_tags which can be used to cue stuff in a game.
    /// This is just one way of doing it. Not the only method on how to trigger events. 
    bool ParseTags()
    {
        bool isFading = false;
        m_tags = m_story.currentTags;
        foreach (string t in m_tags)
        {
            string prefix = t.Split(' ')[0];
            string param = t.Split(' ')[1];

            switch(prefix.ToLower())
            {
                case "anim":
                    SetAnimation(param);
                    break;
                case "color":
                    SetTextColor(param);
                    break;
                case "fade":
                    S_Transition.Instance.LaunchFade(m_listBG[int.Parse(param)]);
                    isFading=true;
                break;
                case "place":
                    m_charPosSelected =m_listCharPos[int.Parse(param)];
                break;
                case "sprite":
                    Sprite spriteExist = m_listSprite.Find(x => x.name == param);
                    if (spriteExist == null)
                    {
                        m_charPosSelected.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_charPosSelected.gameObject.SetActive(true);
                        m_charPosSelected.GetComponent<Image>().sprite = m_listSprite.Find(x => x.name == param);
                    }
                break;
                case "name":
                    m_nametag.text = param;
                break;


            }
        }
        return isFading;
    }
        void SetAnimation(string _name)
    {
        CharacterScript cs = GameObject.FindObjectOfType<CharacterScript>();
        cs.PlayAnimation(_name);
    }
    void SetTextColor(string _color)
    {
        switch(_color)
        {
            case "red":
                m_message.color = Color.red;
                break;
            case "blue":
                m_message.color = Color.cyan;
                break;
            case "green":
                m_message.color = Color.green;
                break;
            case "white":
                m_message.color = Color.white;
                break;
            default:
                Debug.Log($"{_color} is not available as a text color");
                break;
        }
    }

}
