using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using VisualEffects;

namespace MyGame2
{
    struct CarAbility
    {
        public float HandleMax;
        public float Accel;
        public float Break;
        public float MaxSpeed;
        public float Steering;
        public CarAbility(float handleMax, float accel, float bre, float maxSpeed,float steering)
        {
            HandleMax = handleMax;
            Accel = accel;
            Break = bre;
            MaxSpeed = maxSpeed;
            Steering = steering;
        }
    }
    class Car : Obj3D
    {
        Handle handle;
        CarAbility ability;
        Particle smoke;
        Water water;
        Obj3D taiho;
        float smokeVel;
        float spinTime;
        float velocity = 0;
        float shotAngle;
        public Car(Model m, Model taiho,Vector3 iniPosition, float iniAngle, Handle handle,CarAbility ability)
            : base(m, iniPosition, iniAngle,true, null)
        {
            this.handle = handle;
            smoke = Particle.CreateFromParFile("Par/smoke1.par");
            //smoke.MaxParts = 2000;
            smoke.Scale = 1f;
            smokeVel = smoke.InitialVelocity;
            smoke.UpdateOrder = this.UpdateOrder + 1;
            HitScales = new Vector3(2.4675f, 2.4465f, 3.965f);
            this.ability = ability;
            water = new Water(Position + Vector3.Up * HitScales.Y);
            shotAngle = 30;
            this.taiho = new Obj3D(taiho, false);
        }
        public override void AddToGameComponents(Game game)
        {
            game.Components.Add(smoke);
            game.Components.Add(taiho);
            water.AddToGameComponents(game);
            base.AddToGameComponents(game);
        }
        public override void Update(GameTime gameTime)
        {
            smoking();
            HandleState handleState = handle.GetState();
            if (handleState.Break != 0)
            {
                velocity -= handleState.Break * ability.Break;
                if (velocity < -0.3f)
                {
                    velocity = -0.3f;
                }
            }
            else if (handleState.Accel != 0)
            {
                velocity += handleState.Accel * ability.Accel;
                if (velocity > 1)
                    velocity = 1;
            }
            else if(velocity != 0)
            {
                if (velocity > 0)
                {
                    velocity -= ability.Break * 0.5f;
                    if (velocity < 0)
                        velocity = 0;
                }
                else
                {
                    velocity += ability.Break * 0.5f;
                    if (velocity > 0)
                        velocity = 0;
                }
            }
            Speed = ability.MaxSpeed * velocity;

            if (spinTime == 0)
            {
                if (handleState.Handle != 0)
                {
                    int val = velocity >= 0 ? 1 : -1;
                    AngleVelXZ -= handleState.Handle * ability.Steering*val;
                    if (AngleVelXZ < Math.Abs(handleState.Handle) * -ability.HandleMax * Math.Abs(velocity))
                    {
                        AngleVelXZ = Math.Abs(handleState.Handle) * -ability.HandleMax * Math.Abs(velocity);
                    }
                    else if (AngleVelXZ > Math.Abs(handleState.Handle) * ability.HandleMax * Math.Abs(velocity))
                    {
                        AngleVelXZ = Math.Abs(handleState.Handle) * ability.HandleMax * Math.Abs(velocity);
                    }
                    //if (AngleVelXZ > -handleState.Handle * ability.HandleMax * velocity)
                    //    AngleVelXZ = -handleState.Handle * ability.HandleMax * velocity;
                    //else if (AngleVelXZ < -ability.HandleMax * handleState.Handle * velocity)
                    //    AngleVelXZ = -ability.HandleMax * handleState.Handle * velocity;
                }
                else
                {
                    AngleVelXZ *= (1-ability.Steering);
                    if (Math.Abs(AngleVelXZ) < 0.001f)
                        AngleVelXZ = 0;
                }
            }
            else
            {
                velocity *= 0.98f;
                AngleVelXZ = spinTime;
                if (spinTime-- < 0)
                    spinTime = 0;
            }

            if (handleState.Shot > 0f)
            {
                water.Position = Position + Vector3.Up * HitScales.Y;
                
                Vector3 shotDirection = Vector3.Normalize(DirectionXZ + Vector3.Up * (float)Math.Tan(MathHelper.ToRadians(shotAngle)));
                water.Shot(shotDirection, handleState.Shot, SpeedMPF/**(float)Math.Cos(MathHelper.ToRadians(shotAngle))*//5,1.0f);
            }
            if (handleState.ShotAngle != 0)
            {
                shotAngle += handleState.ShotAngle;
                if (shotAngle > 45)
                {
                    shotAngle = 45;
                }
                else if (shotAngle < -5)
                {
                    shotAngle = -5;
                }
            }
            //if (GamePad.GetState(PlayerIndex.One).IsButtonDown(Buttons.RightShoulder))
            //{
            //    Death();
            //}
            base.Update(gameTime);
            taiho.Position = Position + Vector3.Up * HitScales.Y;
            taiho.AngleXZ = this.AngleXZ;
            taiho.AngleYZ = -shotAngle;
        }

        private void smoking()
        {
            smoke.EmitPoint = Position + Vector3.Transform(new Vector3(0.6442f, 0.4706f, -1.8767f), Matrix.CreateRotationY(MathHelper.ToRadians(AngleXZ)));
            smoke.InitialVelocity = smokeVel - SpeedMPF;
            smoke.Direction = -DirectionXZ;
            smoke.Emit();
            //game.Window.Title = "" + smoke.Elements.Count;
        }
        public void Jump()
        {
            if (Position.Y == 0)
            {
                vy = 9;
            }
        }
        public void Spin()
        {
            spinTime = 60;
        }
        public void ReStart()
        {
            if (!Enabled)
            {
                taiho.Enabled = taiho.Visible = Enabled = Visible = true;
                Position = Vector3.Zero;
                velocity = 0;
            }
        }
        public override void Draw(GameTime gameTime)
        {
            setViewProj(Camera, smoke);
            base.Draw(gameTime);
        }
        void setViewProj(Camera c,params Particle[] particles)
        {
            foreach (Particle p in particles)
            {
                p.SetViewProj(Camera);
            }
        }
        public override void HitAction(Fire fire)
        {
            if (Enabled)
            {
                Death();
                taiho.Death();
            }
            base.HitAction(fire);
        }
        public override void Dispatch(Obj3D obj)
        {
            obj.HitAction(this);
        }
    }
    static class Extension
    {
        public static void SetViewProj(this Particle p, Camera c)
        {
            p.SetView(c.Position, c.Target, Vector3.Up);
            p.SetProjection(c.Projection);
        }
    }
}
