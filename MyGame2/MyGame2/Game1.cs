using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using VisualEffects;
namespace MyGame2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        float stage = 200;
        Car car;
        int frames = 0;
        Random rand;
        List<Fire> fires;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 960;
            graphics.PreferredBackBufferHeight = 720;
            VisualEffect.Init(this);
            Particle.WholeDrawOrder = 3;
            rand = new Random();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Camera.Init(this);
            Obj3D.Init(this,new Vector3(stage,stage,stage));
            fires = new List<Fire>();
            base.Initialize();
        }

        //Obj3D test;
        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Model carModel = Content.Load<Model>("car");
            car = new Car(carModel, Content.Load<Model>("taiho"),Vector3.One, 45, new XboxHandle(), new CarAbility(2, 0.01f,0.01f, 100000,0.1f));
            car.AddToGameComponents(this);
            Components.Add(new Obj3D(Content.Load<Model>("stage"),false){Scales = new Vector3(stage,1,stage),HitScales = Vector3.One*stage});
            Components.Add(new Obj3D(Content.Load<Model>("skytree"), false) { Scales = Vector3.One * stage*2, Lighting = false });
            //ÉLÉÉÉÅÉâÇÃê›íË
            //Obj3D.Camera = new Camera(new Vector3(stage, stage, 0), Vector3.One, 45);
            Obj3D.Camera = new ChaseCamera(car, 10, 10,5, 60);
            //Obj3D.Camera = new Camera(Vector3.Up * stage*2, new Vector3(0,0,0.0001f), 45);
            //Obj3D.Camera = new FixedChaseCamera(car, new Vector3(-stage,20,-stage), 45);
            Components.Add(Obj3D.Camera);
            for (int i = 0; i < 5; i++)
            {
                addFire();
            }
        }
        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            try
            {
                // Allows the game to exit
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                    this.Exit();
                //è’ìÀîªíË

                Obj3D[] list = objList();
                for (int i = 0; i < list.Length - 1; i++)
                {
                    for (int j = i + 1; j < list.Length; j++)
                    {
                        if (list[i].Hit(list[j]) || list[j].Hit(list[i]))
                        {
                            list[i].Dispatch(list[j]);
                            list[j].Dispatch(list[i]);
                        }
                    }
                    //Window.Title = i + "";
                }
                if (frames++ % 300 == 299 && fires.Count < 7)
                {
                    addFire();
                }
                if (!car.Enabled)
                {
                    if (restartFrames-- < 0)
                    {
                        restartFrames = 120;
                        car.ReStart();
                    }
                }
                for (int i = 0; i < fires.Count; i++)
                {
                    if (!fires[i].Enabled)
                    {
                        fires.RemoveAt(i);
                    }
                }
                //test.AngleXZ += 1;
                // TODO: Add your update logic here
                base.Update(gameTime);
            }
            catch (Exception e)
            {
                Window.Title = e.Message;
            }
        }

        private Obj3D[] objList()
        {
            return Components.OfType<Obj3D>().Where<Obj3D>((obj) => { return obj.Enabled; }).ToArray();
        }
        int restartFrames = 120;

        private void addFire()
        {
            Vector3 position = new Vector3(
                rand.Next((int)(stage * 0.2f),(int)(stage * 0.8f)),
                15,
                rand.Next((int)(stage * 0.2f),(int)(stage * 0.8f))
            );
            if (rand.Next(0, 2) == 0)
                position.X = -position.X;
            if (rand.Next(0, 2) == 0)
                position.Z = -position.Z;
            Fire f = new Fire(position, rand.Next(10, 20));
            fires.Add(f);
            f.AddToGameComponents(this);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            

            base.Draw(gameTime);
        }
    }
}
