#if UNITY_EDITOR
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AssignSFXToMixer : MonoBehaviour
{
    public string sfxTag = "SFX";
    public AudioMixerGroup sfxGroup;

    [Button(ButtonSizes.Large)]private void TagAllAudioSources(string tag = "SFX")
    {
        // 获取场景中所有的 AudioSource 组件
        AudioSource[] audioSources = GameObject.FindObjectsOfType<AudioSource>(includeInactive: true);

        foreach (AudioSource source in audioSources)
        {
            if (source.gameObject.tag == "BGM")
            {
                Debug.Log($"Skipping[{source.gameObject.name}] AudioSource on BGM",source);
                continue;
            }
            source.gameObject.tag = tag;
            Debug.Log($"Tagged[{source.gameObject.name}] AudioSource as {tag}",source);
        }

        Debug.Log($"Tagged {audioSources.Length} AudioSources as 'SFX'");
    }
    [Button(ButtonSizes.Large)]public void AssignAllSFXToMixerGroup()
    {
        if (!sfxGroup)
        {
            Debug.LogError("No SFX Mixer Group assigned!");
            return;
        }
        var sfxSources = GameObject.FindGameObjectsWithTag(sfxTag);
        foreach (var src in sfxSources)
        {
            var audioSource = src.GetComponent<AudioSource>();
            if (audioSource)
            {
                audioSource.outputAudioMixerGroup = sfxGroup;
                Debug.Log($"Assigned {audioSource.gameObject.name} to SFX Mixer Group", audioSource);
            }
        }
        Debug.Log($"Assigned {sfxSources.Length} AudioSources to SFX Mixer Group");
    }
}
#endif