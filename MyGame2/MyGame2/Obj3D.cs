using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using VisualEffects;
namespace MyGame2
{
    struct MoveParams
    {
        public Vector3 Position;
        public float Speed;
        public float AngleXZ;
        public MoveParams(Vector3 position, float speed,float angleXZ)
        {
            Position = position;
            Speed = speed;
            AngleXZ = angleXZ;
        }
    }
    class Obj3D : DrawableGameComponent 
    {
        protected static Game game;
        static Vector3 worldScale;
        int frames = 0;
        public Func<int,MoveParams> Move;
        
        public static void Init(Game g,Vector3 stageScale)
        {
            game = g;
            worldScale = stageScale;
        }
        Model model;
        MoveParams moveParams;
        protected float vy;
        Particle explode;
        float gravity;
        public bool Lighting { get; set; }
        public Vector3 Scales { get; set; }
        public Vector3 HitScales { get; set; }
        public Vector3 Position { get { return moveParams.Position; } set { moveParams.Position = value; } }
        public float Speed { get { return moveParams.Speed; } set { moveParams.Speed = value; } }
        public float SpeedMPF { get { return Speed / 3600.0f / 60; } }
        public float AngleXZ { get { return moveParams.AngleXZ; } set { moveParams.AngleXZ = value; } }
        public float AngleYZ { get; set; }
        public float AngleVelXZ { get; set; }
        public Vector3 DirectionXZ { get { float rad = MathHelper.ToRadians(moveParams.AngleXZ); return Vector3.Normalize(new Vector3((float)Math.Sin(rad), 0, (float)Math.Cos(rad))); } }
        public static Camera Camera { get; set; }
        public Obj3D(Model m,/*Camera camera,*/Vector3 position,float angleXZ,bool fall, Func<int,MoveParams> moveMethod)
            : base(game)
        {
            model = m;
            //Camera = camera;
            Position = position;
            AngleXZ = angleXZ;
            Move = moveMethod;
            Scales = Vector3.One;
            HitScales = Vector3.Zero;
            vy = 0;
            explode = Particle.CreateFromParFile("Par/explosion2.par");
            explode.UpdateOrder = this.UpdateOrder + 1;
            explode.InitialColor = new Vector3(255, 128, 0);
            gravity = fall ? 9.8f / 60 : 0;
            Lighting = true;
        }
        public Obj3D(Model m,bool fall)//, Camera camera)
            : this(m, /*camera,*/ Vector3.Zero, 0,fall,null)
        {
        }
        public Obj3D(bool fall)
            : this(null, Vector3.Zero, 0, fall, null)
        {
        }
        public override void Update(GameTime gameTime)
        {
            if (Move != null)
            {
                moveParams = Move(frames);
            }
            Position += SpeedMPF * DirectionXZ + Vector3.Up * vy/60;
            vy -= gravity;
            if (Position.Y < 0)
            {
                moveParams.Position.Y = 0;
                vy = 0;
            }
            else if (Position.Y > worldScale.Y)
            {
                moveParams.Position.Y = worldScale.Y;
            }
            if (Position.X > worldScale.X)
            {
                moveParams.Position.X = worldScale.X;
            }
            else if (Position.X < -worldScale.X)
            {
                moveParams.Position.X = -worldScale.X;
            }
            if (Math.Abs(Position.Z) > worldScale.Z)
            {
                moveParams.Position.Z = moveParams.Position.Z / Math.Abs(moveParams.Position.Z) * worldScale.Z;
            }
            AngleXZ += AngleVelXZ;
            AngleXZ = AngleXZ % 360;
            frames++;
            base.Update(gameTime);
        }
        public virtual void AddToGameComponents(Game game)
        {
            game.Components.Add(explode);
            game.Components.Add(this);
        }
        public override void Draw(GameTime gameTime)
        {
            if (model != null)
            {
                foreach (ModelMesh m in model.Meshes)
                {
                    foreach (BasicEffect e in m.Effects)
                    {
                        if(Lighting)
                            e.EnableDefaultLighting();
                        e.World = Matrix.CreateScale(Scales) * Matrix.CreateRotationX(MathHelper.ToRadians(AngleYZ)) * Matrix.CreateRotationY(MathHelper.ToRadians(AngleXZ)) * Matrix.CreateTranslation(Position);
                        e.View = Camera.View;
                        e.Projection = Camera.Projection;
                    }
                    m.Draw();
                }
            }
            explode.SetViewProj(Camera);
            base.Draw(gameTime);
        }
        public void Death()
        {
            Visible = Enabled = false;
            Speed = 0;
            explode.Scale = HitScales.Length();
            explode.EmitPoint = Position + Vector3.Up * 1.5f;
            explode.Emit();
        }

        //衝突判定
        public BoundingBox HitBox 
        { 
            get 
            {
                Vector3 offsetMin = new Vector3(-HitScales.X / 2, 0, -HitScales.Z / 2);
                Vector3 offsetMax = new Vector3(HitScales.X / 2, HitScales.Y, HitScales.Z / 2);
                BoundingBox b = new BoundingBox(Position + offsetMin, Position + offsetMax);
                return b;
            } 
        }
        public virtual bool Hit(Obj3D obj)
        {
            return obj.HitBox.Intersects(HitBox);
        }
        //衝突時のアクション
        public virtual void Dispatch(Obj3D obj)
        {
            obj.HitAction(this);
        }
        public virtual void HitAction(Obj3D obj) { }
        public virtual void HitAction(Car car) { }
        public virtual void HitAction(Fire fire) { }
        public virtual void HitAction(Water water) { }
    }
}
