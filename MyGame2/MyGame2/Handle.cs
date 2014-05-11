using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
namespace MyGame2
{
    struct HandleState
    {
        public float Accel;
        public float Break;
        public float Handle;
        public float Shot;
        public float ShotAngle;
        public HandleState(float accel, float bre,float handle,float shot,float shotAngle)
        {
            Accel = accel;
            Break = bre;
            Handle = handle;
            Shot = shot;
            ShotAngle = shotAngle;
        }
    }
    abstract class Handle
    {
        public abstract HandleState GetState();
    }
    class XboxHandle:Handle
    {
        public override HandleState GetState()
        {
            HandleState res = new HandleState();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            res.Handle = state.ThumbSticks.Left.X;
            res.Accel = state.ThumbSticks.Left.Y > 0 || state.IsButtonDown(Buttons.A) ? 1 : 0;
            res.Break = state.ThumbSticks.Left.Y < 0 || state.IsButtonDown(Buttons.X) ? 1 : 0;
            res.Shot = state.Triggers.Right;
            res.ShotAngle = state.ThumbSticks.Right.Y;
            return res;
        }
    }
}
