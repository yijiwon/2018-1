using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource musicSource;
    public static SoundManager instance = null;

    public float lowPitchRange = .95f;
    public float highPitchRange = 1.05f;

	// Use this for initialization
	void Awake () {
        if (instance == null)
            instance = this;
        else if (instance != null)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
	}

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;
        efxSource.Play();
    }

    // params = ,로 구분된 같은 타입의 여러 입력들을 한번에 받는 게 가능.
    public void RandomizeSfx(params AudioClip [] clips)
    {
        // 랜덤하게 클립을 선택해 플레이하고,
        // 피치를 랜덤하게 해서 여러 바리에이션을 만듬.
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(lowPitchRange, highPitchRange);

        efxSource.pitch = randomPitch;
        efxSource.clip = clips[randomIndex];
        efxSource.Play();
    }
}
