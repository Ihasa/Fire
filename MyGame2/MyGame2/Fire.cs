using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using VisualEffects;
using Microsoft.Xna.Framework;
namespace MyGame2
{
    class Fire : Obj3D
    {
        public static float MinScale = 2f;
        Particle fire;
        float scale;
        static ParticleParams fireParams;
        static Fire()
        {
            fireParams = Particle.CreateFromParFile("Par/fire.par").Parameters;
        }
        public Fire(Vector3 position,float scale)
            :base(null,position,0,true,null)
        {
            fire = new Particle(fireParams);
            fire.UpdateOrder = this.UpdateOrder + 1;
            HitScales = scale * new Vector3(0.1f,1,0.1f);
            this.scale = fire.Scale = scale;
        }
        public override void AddToGameComponents(Microsoft.Xna.Framework.Game game)
        {
            game.Components.Add(fire);
            base.AddToGameComponents(game);
        }
        public void Weak(float val)
        {
            scale -= val;
        }
        public void Strong(float val)
        {
            scale += val;
        }
        public override void Update(GameTime gameTime)
        {
            if (Enabled)
            {
                if (scale > MinScale)
                {
                    fire.EmitPoint = Position;
                    fire.Scale = scale;
                    fire.Emit();
                    HitScales = scale * new Vector3(0.5f, 1, 0.5f);
                }
                else
                {
                    scale = 0;
                    Enabled = false;
                }
            }
            //game.Window.Title = "" + fire.Elements.Count;
            base.Update(gameTime);
        }
        public override void Draw(GameTime gameTime)
        {
            fire.SetViewProj(Camera);
            base.Draw(gameTime);
        }
        //public override bool Hit(Obj3D obj)
        //{
        //    return base.Hit(obj);
        //    //foreach (ParticleElement e in fire.Elements)
        //    //{
        //    //    if (obj.HitBox.Contains(e.Position) != ContainmentType.Disjoint)
        //    //        return true;
        //    //}
        //    //return false;
        //}
        public override void HitAction(Car car)
        {
            base.HitAction(car);
        }
        public override void Dispatch(Obj3D obj)
        {
            obj.HitAction(this);
        }
    }
}
