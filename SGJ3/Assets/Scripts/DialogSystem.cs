using UnityEngine;
using System.Collections.Generic;
using System;

public struct DialogID
{
    public int ID;
}
[System.Serializable]
public class DialogSystem
{
    class DialogState
    {
        public DialogID ID;
        public float StartTime;
        public Dialog Dialog;
        public string[] LastDialogStrings;
        public AudioSource ActiveSound;
    }
    List<DialogState> _DialogsQueued = new List<DialogState>();
    DialogState _CurrentDialog;
    public DialogBox TreeBox;
    public DialogBox RobotBox;
    public AudioClip[] TreeVoiceClips;
    public AudioClip TreeQuestionClip;
    public AudioClip[] RobotVoiceClips;
    public AudioClip RobotQuestionClip;
    public float VoiceVolume;
    public float MinPitch;
    public float MaxPitch;

    int _DialogID;

    public DialogID PlayDialog(Dialog dialog)
    {
        DialogState state = new DialogState();
        state.Dialog = dialog;
        state.LastDialogStrings = new string[dialog.Entries.Length];
        for (int j = 0; j < state.LastDialogStrings.Length; j++)
            state.LastDialogStrings[j] = string.Empty;
        _DialogID++;

        state.ID = new DialogID() { ID = _DialogID, };
        _DialogsQueued.Add(state);
        return state.ID;
    }

    public bool IsAlive(DialogID id)
    {
        if (_CurrentDialog != null && _CurrentDialog.ID.ID == id.ID)
            return true;
        for (int i = 0; i < _DialogsQueued.Count; i++)
        {
            var state = _DialogsQueued[i];
            if (state.ID.ID == id.ID)
            {
                return true;
            }
        }
        return false;
    }

    AudioSource PlayQuestionClip(Person person)
    {
        AudioClip clip = person == Person.Tree ? TreeQuestionClip : RobotQuestionClip;
        return PlayClip(clip);
    }
    AudioSource PlayClip(Person person)
    {
        AudioClip[] clips = person == Person.Tree ? TreeVoiceClips : RobotVoiceClips;
        AudioClip clip = clips[UnityEngine.Random.Range(0, clips.Length)];
        return PlayClip(clip);
    }

    private AudioSource PlayClip(AudioClip clip)
    {
        float pitch = UnityEngine.Random.Range(MinPitch, MaxPitch);
        return Sound.Instance.PlayClipAtPoint(clip, Vector3.zero, VoiceVolume, pitch);
    }

    void SetPortraitState(Person person, string text, float alpha)
    {
        DialogBox box = null;
        switch (person)
        {
            case Person.Robot:
                box = RobotBox;
                break;
            case Person.Tree:
                box = TreeBox;
                break;
        }
        SetActive(box.gameObject, alpha > 0);
        box.Text.text = text;
        box.SetAlpha(alpha);
    }

    private void SetActive(GameObject gameObject, bool v)
    {
        if (gameObject.activeSelf != v)
            gameObject.SetActive(v);
    }

    public void Update()
    {
        float time = Time.time;
        if (_CurrentDialog == null && _DialogsQueued.Count > 0)
        {
            _CurrentDialog = _DialogsQueued[0];
            _CurrentDialog.StartTime = Time.time;
            _DialogsQueued.RemoveAt(0);
        }
        SetPortraitState(Person.Robot, string.Empty, 0f);
        SetPortraitState(Person.Tree, string.Empty, 0f);

        if (_CurrentDialog != null)
        {
            float dialogTime = time - _CurrentDialog.StartTime;
            float timeAccumulator = 0f;
            for (int i = 0; i < _CurrentDialog.Dialog.Entries.Length; i++)
            {
                var entry = _CurrentDialog.Dialog.Entries[i];
                float entryDuration = entry.StartDelay + entry.ScrollForwardTime + entry.LingerTime;
                if (dialogTime > entry.StartDelay + timeAccumulator && dialogTime <= entry.StartDelay + entryDuration + timeAccumulator)
                {
                    float entryTime = dialogTime - timeAccumulator - entry.StartDelay;
                    float scrollTime = Mathf.Clamp01(entryTime / entry.ScrollForwardTime);
                    int length = Mathf.RoundToInt(entry.Text.Length * scrollTime);
                    string text;
                    if (_CurrentDialog.LastDialogStrings[i].Length != length)
                    {
                        text = _CurrentDialog.LastDialogStrings[i] = entry.Text.Substring(0, length);
                        if (text.EndsWith(" ") == false && text.EndsWith("\n") == false)
                        {
                            if (_CurrentDialog.ActiveSound)
                                GameObject.Destroy(_CurrentDialog.ActiveSound.gameObject);

                            if (text.EndsWith("?"))
                                _CurrentDialog.ActiveSound = PlayQuestionClip(entry.Person);
                            else
                                _CurrentDialog.ActiveSound = PlayClip(entry.Person);
                        }
                    }
                    else
                    {
                        text = _CurrentDialog.LastDialogStrings[i];
                    }

                    bool shouldFade = true;
                    if (_CurrentDialog.Dialog.Entries.Length > i + 1)
                    {
                        var nextEntry = _CurrentDialog.Dialog.Entries[i + 1];
                        shouldFade = nextEntry.Person != entry.Person || nextEntry.StartDelay > 0;
                    }

                    float timeSinceLinger = (entry.LingerTime + entry.ScrollForwardTime) - entryTime;
                    float alpha = shouldFade ? Mathf.Clamp01(timeSinceLinger * 5f) : 1f;
                    SetPortraitState(entry.Person, text, alpha);
                }
                timeAccumulator += entryDuration;
            }
            if (timeAccumulator < dialogTime)
                _CurrentDialog = null;
        }
    }
}
