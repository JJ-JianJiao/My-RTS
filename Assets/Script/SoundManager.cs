using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;


    [SerializeField]
    private AudioSource BGM_AS;
    [SerializeField]
    private AudioSource soldierAS;
    [SerializeField]
    private AudioSource pauseResumePanel_AS;
    [SerializeField]
    private AudioSource buttonClick_AS;
    //[SerializeField]
    //private AudioSource soldierWhat_AS;
    //[SerializeField]
    //private AudioSource soldierAttack_AS;
    [SerializeField]
    private List<AudioClip> soldiorWhatsClips;
    [SerializeField]
    private List<AudioClip> soldiorYesClips;
    [SerializeField]
    private List<AudioClip> soldiorAttackClips;
    [SerializeField]
    private List<AudioClip> gamePauseResumeClips;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        pauseResumePanel_AS.ignoreListenerPause = true;
        buttonClick_AS.ignoreListenerPause = true;
        BGM_AS.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SoldierYes() {
        int index = Random.Range(0, soldiorYesClips.Count);
        //soldierYes_AS.clip = soldiorYesClips[index];
        //soldierYes_AS.Play();
        soldierAS.clip = soldiorYesClips[index];
        soldierAS.Play();
    }
    public void SoldierAttack()
    {
        int index = Random.Range(0, soldiorAttackClips.Count);
        //soldierAttack_AS.clip = soldiorAttackClips[index];
        //soldierAttack_AS.Play();
        soldierAS.clip = soldiorAttackClips[index];
        soldierAS.Play();
    }

    public void SoldierWhat()
    {
        int index = Random.Range(0, soldiorWhatsClips.Count);
        //soldierWhat_AS.clip = soldiorWhatsClips[index];
        //soldierWhat_AS.Play();
        soldierAS.clip = soldiorWhatsClips[index];
        soldierAS.Play();
    }

    public void PlayGamePauseAS() {
        pauseResumePanel_AS.clip = gamePauseResumeClips[0];
        pauseResumePanel_AS.Play();
    }

    public void PlayGameResumeAS()
    {
        pauseResumePanel_AS.clip = gamePauseResumeClips[1];
        pauseResumePanel_AS.Play();
    }

    public void PlayButtonClickAS() {
        buttonClick_AS.Play();
    }
}
