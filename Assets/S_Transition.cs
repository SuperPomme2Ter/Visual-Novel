using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class S_Transition : MonoBehaviour
{
    public static S_Transition Instance;

    public bool m_doFade;
    [SerializeField]
    private Image m_bg;
    [SerializeField]
    GameObject m_dialogBox;
    private Sprite m_bgSprite;
    private bool m_fadeState;
    private Color m_LerpColor;
    void Start()
    {
        m_doFade = false;
        m_fadeState = false;
        m_LerpColor = new Color(m_bg.color.r, m_bg.color.r, m_bg.color.r, 1);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_doFade)
        {
            m_bg.color=Color.Lerp(m_bg.color,m_LerpColor,Time.deltaTime);
            if (m_bg.color.a >= 1 && !m_fadeState)
            {
                m_bg.color = m_LerpColor;
                m_LerpColor = new Color(m_bg.color.r, m_bg.color.r, m_bg.color.r,0);
                m_fadeState=true;

            }
            else if (m_bg.color.a < 0.1f && m_fadeState)
            {
                m_bg.color = new Color(m_bg.color.r, m_bg.color.r, m_bg.color.r, 0); ;
                m_doFade =false;
                m_dialogBox.SetActive(true);
                DialogueManager.Instance.enabled = true;
                DialogueManager.Instance.ContinueDialogue();
            }

        }
    }
    public void LaunchFade(Sprite bgSprite)
    {
        DialogueManager.Instance.enabled=false;
        m_bgSprite = bgSprite;
        m_dialogBox.SetActive(false);
        m_fadeState=false;
        m_LerpColor= new Color(m_bg.color.r, m_bg.color.r, m_bg.color.r, 1.5f); ;
        m_doFade = true;

    }
}
