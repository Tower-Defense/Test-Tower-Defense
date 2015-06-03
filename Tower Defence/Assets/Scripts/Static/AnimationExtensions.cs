using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public static class AnimationExtensions
{
    public static IEnumerator WhilePlaying(this Animation animation)
    {
        do
        {
            yield return null;
        } while (animation.isPlaying);
    }

    public static IEnumerator WhilePlaying(this Animation animation,
        string animationName, Action action = null)
    {
        var state = animation.PlayQueued(animationName);
        yield return new WaitForSeconds(state.clip.length);
        if (action != null)
        {
            action();
        }
    }
}

