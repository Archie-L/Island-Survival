using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource effects;
    public AudioSource ambience;
    public Slider effectsSlider;
    public AudioSource music;
    public Slider musicSlider;
    public bool isMenu;

    // Start is called before the first frame update
    void Start()
    {
        effects = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
        ambience = GameObject.FindGameObjectWithTag("ambience").GetComponent<AudioSource>();
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();

        if (!isMenu)
        {
            musicSlider.value = PlayerPrefs.GetFloat("menuMusic");
            effectsSlider.value = PlayerPrefs.GetFloat("menuEffects");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMenu)
        {
            PlayerPrefs.SetFloat("menuEffects", effectsSlider.value);
            PlayerPrefs.SetFloat("menuMusic", musicSlider.value);
        }
        else
        {
            effects.volume = effectsSlider.value;
            ambience.volume = effectsSlider.value;
            music.volume = musicSlider.value;
        }
    }
}
