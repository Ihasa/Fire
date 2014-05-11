using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
namespace MyGame2
{
    class Camera : GameComponent
    {
        static float aspectRatio{get;set;}
        static Game game;
        public static void Init(Game g)
        {
            game = g;
            aspectRatio = g.GraphicsDevice.Viewport.AspectRatio;
        }
        public Vector3 Position { get; set; }
        public Vector3 Target { get; set; }
        public float FieldOfView { get; set; }
        public Matrix View { get { return Matrix.CreateLookAt(Position, Target, Vector3.Up); } }
        public Matrix Projection { get { return Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(FieldOfView), aspectRatio, 0.1f, 1000.0f); } }
        public Camera(Vector3 position, Vector3 target, float fov)
            :base(game)
        {
            Position = position;
            Target = target;
            FieldOfView = fov;
        }
    }
    class ChaseCamera : Camera
    {
        Obj3D target;
        float offset;
        float height;
        float chaseHeight;
        public ChaseCamera(Obj3D target, float offset, float height, float chaseHeight,float fov)
            : base(target.Position - offset * target.DirectionXZ + Vector3.Up * height, target.Position+Vector3.Up*chaseHeight, fov)
        {
            this.target = target;
            this.offset = offset;
            this.height = height;
            this.chaseHeight = chaseHeight;
        }

        public override void Update(GameTime gameTime)
        {
            Position = target.Position + Vector3.Transform(-offset * target.DirectionXZ + Vector3.Up * height, Matrix.CreateRotationY(MathHelper.ToRadians(target.AngleVelXZ * -5)));
            Target = target.Position + Vector3.Up*chaseHeight;
            base.Update(gameTime);
        }
    }
    class FixedChaseCamera : Camera
    {
        Obj3D target;
        public FixedChaseCamera(Obj3D target, Vector3 position, float fov)
            :base(position,target.Position,fov)
        {
            this.target = target;
        }
        public override void Update(GameTime gameTime)
        {
            Target = target.Position;
            base.Update(gameTime);
        }
    }
}
