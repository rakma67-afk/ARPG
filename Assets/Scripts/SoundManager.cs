using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    AudioSource bgm;
    AudioSource sfx;

    public AudioClip[] bgms;
    public AudioClip[] sfxs;
    private List<AudioSource> sfxSources = new List<AudioSource>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < bgms.Length; i++)
        {
            if (i == 0)
            {
                bgm = gameObject.AddComponent<AudioSource>();
                bgm.clip = bgms[i];
                bgm.loop = true;
                bgm.volume = 0.20f;
                bgm.Play();
            }
        }

        foreach (AudioClip clip in sfxs)
        {
            sfx = gameObject.AddComponent<AudioSource>();
            sfx.clip = clip;
            sfx.loop = false;
            sfx.playOnAwake = false;
            sfxSources.Add(sfx);
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playerSlash()
    {
        sfxSources[0].Play();
        sfxSources[9].Play();
        
    }
    public void playerWalk(bool Play)
    {
        if (Play)
        {
            sfxSources[1].Play();
        }
        else
        {
            sfxSources[1].Stop();
        }
    }
    public void playerHurt()
    {
        sfxSources[2].Play();
    }
    public void playerRoll()
    {
        sfxSources[3].Play();
    }
    public void playerSkillA()
    {
        sfxSources[4].Play();
    }
    public void playerSkillS()
    {
        sfxSources[5].Play();
        sfxSources[9].Play();
    }
    public void playerSkillD()
    {
        sfxSources[6].Play();
        sfxSources[9].Play();
    }
    public void playerDead()
    {
        sfxSources[7].Play();
        sfxSources[10].Play();
    }
    public void playerLevelUp()
    {
        sfxSources[8].Play();
    }
    public void enemyHurt()
    {
        sfxSources[11].Play();
    }
    public void enemyAttack()
    {
        sfxSources[12].Play();
    }
    public void enemygrowl()
    {
        sfxSources[13].Play();
    }
    public void bossAggro()
    {
        sfxSources[14].Play();
    }
    public void enemyDie()
    {
        sfxSources[15].Play();
    }


    
}
