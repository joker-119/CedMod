﻿// Decompiled with JetBrains decompiler
// Type: Waits.AnimatorWait
// Assembly: Assembly-CSharp, Version=1.2.3.4, Culture=neutral, PublicKeyToken=null
// MVID: 4FF70443-CA06-4035-B3D1-98CFA9EE67BF
// Assembly location: D:\steamgames\steamapps\common\SCP Secret Laboratory Dedicated Server\SCPSL_Data\Managed\Assembly-CSharp.dll

using MEC;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Waits
{
  public class AnimatorWait : Wait
  {
    public Animator animator;

    public override IEnumerator<float> _Run()
    {
      // ISSUE: reference to a compiler-generated field
      int num = this.<1__>state;
      AnimatorWait animatorWait = this;
      if (num != 0)
      {
        if (num != 1)
          return false;
        // ISSUE: reference to a compiler-generated field
        this.<>1__state = -1;
        return false;
      }
      // ISSUE: reference to a compiler-generated field
      this.<1__>state = -1;
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated method
      this.<2__>current = Timing.WaitUntilFalse(new Func<bool>(animatorWait.<_Run>b__1_0));
      // ISSUE: reference to a compiler-generated field
      this.<1__>state = 1;
      return true;
    }
  }
}
