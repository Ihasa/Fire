using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VisualEffects;
using Microsoft.Xna.Framework;
namespace MyGame2
{
    class Water : Obj3D
    {
        Particle water;
        Particle smoke;
        float force;
        static ParticleParams smokeParams;
        static Water()
        {
            smokeParams = Particle.CreateFromParFile("Par/smoke1.par").Parameters;
        }
        public Water(Vector3 position)
            : base(null, position, 0,false, null)
        {
            water = Particle.CreateFromParFile("Par/water2.par");
            water.UpdateOrder = this.UpdateOrder + 1;
            water.DrawOrder = Particle.WholeDrawOrder + 1;
            water.Scale = 5;
            water.Script = (p) =>
            {
                float minY = p.Size;
                if (p.BoundingSphere.Intersects(new Plane(Vector3.Up,0)) == PlaneIntersectionType.Intersecting)
                {
                    p.Position = new Vector3(p.Position.X, 0.1f, p.Position.Z);
                    p.Speed = Vector3.Zero;
                    p.Normal = Vector3.Normalize(new Vector3(0, 1, 0.000000001f));
                    
                    //p.EndLife = true;
                }
            };
            force = water.InitialVelocity;
            smoke = new Particle(smokeParams);
            smoke.Direction = Vector3.Up;
            smoke.Scale = 5;
            smoke.UpdateOrder = this.UpdateOrder + 1;
        }

        public void Shot(Vector3 direction, float angle,float speed,float force)
        {
            water.Direction = direction;
            water.Radian = MathHelper.ToRadians(2 + angle * 43);
            water.InitialVelocity = this.force * force + speed;
            water.EmitPoint = Position;
            water.Emit();
        }
        public override void Dispatch(Obj3D obj)
        {
            obj.HitAction(this);
            base.Dispatch(obj);
        }
        public override bool Hit(Obj3D obj)
        {
            foreach (ParticleElement e in water.Elements)
            {
                if (obj.HitBox.Contains(e.Position) != ContainmentType.Disjoint)
                    return true;
            }
            return false;
        }
        public override void HitAction(Fire fire)
        {
            foreach (ParticleElement e in water.Elements)
            {
                if (e.BoundingSphere.Intersects(fire.HitBox))
                {
                    smoke.EmitPoint = e.Position;
                    smoke.Emit();
                    fire.Weak(0.01f);
                    e.EndLife = true;
                }
            }
            base.HitAction(fire);
        }
        public override void AddToGameComponents(Game game)
        {
            game.Components.Add(water);
            game.Components.Add(smoke);
            base.AddToGameComponents(game);
        }
        public override void Update(GameTime gameTime)
        {
            game.Window.Title = "" + water.Elements.Count;
            //game.Window.Title = "" + water.Elements.Count;
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            water.SetViewProj(Camera);
            smoke.SetViewProj(Camera);
            base.Draw(gameTime);
        }

    }
}
