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

    // Start is called before the first frame update
    void Start()
    {
        effects = GameObject.FindGameObjectWithTag("SFX").GetComponent<AudioSource>();
        ambience = GameObject.FindGameObjectWithTag("ambience").GetComponent<AudioSource>();
        music = GameObject.FindGameObjectWithTag("Music").GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        effects.volume = effectsSlider.value;
        ambience.volume = effectsSlider.value;
        music.volume = musicSlider.value;
    }
}
