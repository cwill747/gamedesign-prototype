using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Squared.Tiled;
using FarseerPhysics;
using System.IO;
using FarseerPhysics.Dynamics.Joints;
using Artemis;
using PhaseShift.Entities;

namespace PhaseShift
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _batch;
        private KeyboardState _oldKeyState;
        private GamePadState _oldPadState;
        private SpriteFont _font;

        private World _world;

        private Body _circleBody;
        private Body _groundBody;
        private Texture2D _circleSprite;
        private Texture2D _groundSprite;
        private Body eastwall;
        private Body northwall;
        private Vector2 northwallPosition;
        // Simple camera controls
        private Matrix _view;
        private Vector2 _cameraPosition;
        private Vector2 _screenCenter;
        private Vector2 _groundOrigin;
        private Vector2 _circleOrigin;
        private Texture2D _ship;
        private Body _shipBody;
        private Vector2 _shipOrigin;
        private Map map;
        private float invisibility;
        private PlayerShip ship;

#if !XBOX360
        string Text = "Use WASD for movement\n" +
                            "Press z to fire main weapons\n" +
                            "Use arrow keys to move the camera";
#else
                string Text = "Use left stick to move\n" +
                                    "Use right stick to move camera\n" +
                                    "Press A to jump\n";
#endif



        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 480;

            Content.RootDirectory = "Content";

            //Create a world with no gravity
            _world = new World(Vector2.Zero);

            #if WINDOWS || XBOX
                        _graphics.PreferredBackBufferWidth = 1280;
                        _graphics.PreferredBackBufferHeight = 720;
                        IsFixedTimeStep = true;
            #endif
        }


        protected override void Initialize()
        {

            base.Initialize();
        }
        /// <summary>
        /// Returns a new Vector2 object specified by angle and length using
        /// the standard polar coordinate system.
        /// </summary>
        /// <param name="angle">The direction of the Vector2 in radians.</param>
        /// <param name="length">The magnitude of the Vector2.</param>
        /// <returns></returns>
        public static Vector2 CreateVector2(float angle, float length)
        {
            return new Vector2((float)Math.Cos(angle) * length, (float)Math.Sin(angle) * length);
        }


        protected override void LoadContent()
        {
            ship = new PlayerShip(_world, Content);
            // Initialize camera controls
            _view = Matrix.Identity;
            //_cameraPosition = Vector2.Zero;
            _screenCenter = new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);
            _batch = new SpriteBatch(_graphics.GraphicsDevice);
            map = Map.Load(Path.Combine(Content.RootDirectory, "MapTest.tmx"), Content);
            //map.ObjectGroups["events"].Objects["playership"].Texture = Content.Load<Texture2D>("ShipSprite");
            _cameraPosition.X = ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].X);
            _cameraPosition.Y = ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].Y);
            _font = Content.Load<SpriteFont>("font");

            // Load sprites
            _circleSprite = Content.Load<Texture2D>("CircleSprite"); //  96px x 96px => 1.5m x 1.5m
            _groundSprite = Content.Load<Texture2D>("GroundSprite"); // 512px x 64px =>   8m x 1m
            

            /* We need XNA to draw the ground and circle at the center of the shapes */
            _groundOrigin = new Vector2(_groundSprite.Width / 2f, _groundSprite.Height / 2f);
            _circleOrigin = new Vector2(_circleSprite.Width / 2f, _circleSprite.Height / 2f);

            // Farseer expects objects to be scaled to MKS (meters, kilos, seconds)
            // 1 meters equals 64 pixels here
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);

            /* Circle */
            // Convert screen center from pixels to meters
            Vector2 circlePosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, -1.5f);

            // Create the circle fixture
            //_circleBody = BodyFactory.CreateCircle(_world, ConvertUnits.ToSimUnits(96 / 2f), 1f, circlePosition);
            //_circleBody.BodyType = BodyType.Dynamic;

            eastwall = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["eastwall"].Width), ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["eastwall"].Height), 1);

            eastwall.IsStatic = true;
            eastwall.Restitution = 0.3f;
                        
            northwallPosition = new Vector2(ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["northwall"].Width), ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["northwall"].Height));

            northwall = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["northwall"].Width), ConvertUnits.ToSimUnits(map.ObjectGroups["collision"].Objects["northwall"].Height), 1, 
                northwallPosition);
            northwall.Restitution = 0.3f;
            northwall.IsStatic = true;
            
            // Give it some bounce and friction
            //_circleBody.Restitution = 0.3f;
            //_circleBody.Friction = 0.5f;


            /* Ground */
            Vector2 groundPosition = ConvertUnits.ToSimUnits(_screenCenter) + new Vector2(0, 1.25f);

            // Create the ground fixture
            _groundBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(512f), ConvertUnits.ToSimUnits(64f), 1f, groundPosition);
            _groundBody.IsStatic = true;
            _groundBody.Restitution = 0.3f;
            _groundBody.Friction = 0.5f;

            _ship = Content.Load<Texture2D>("ShipSprite");
            _shipOrigin = new Vector2(_ship.Width / 2f, _ship.Height / 2f);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            Vector2 shipPosition = new Vector2(ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].X), ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].Y));
            _shipBody = BodyFactory.CreateRectangle(_world, ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].Width), ConvertUnits.ToSimUnits(map.ObjectGroups["events"].Objects["playership"].Height), 1);
            _shipBody.Position = shipPosition;
            _shipBody.BodyType = BodyType.Dynamic;
            _shipBody.Mass = 2;
            _shipBody.AngularDamping = 0;
            
            // _shipBody.Rotation = (float) Math.PI / 2;
            _shipBody.Restitution = 0.01f;
            _shipBody.Friction = 0.1f;
            _shipBody.SleepingAllowed = false;
            invisibility = 1f;
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //map.Layers["Layer 1"].Opacity = (float)(Math.Cos(Math.PI * (gameTime.TotalGameTime.Milliseconds * 4) / 10000));
            invisibility = 1f;

            HandleGamePad();
            HandleKeyboard();

            //We update the world
            _cameraPosition = _shipBody.Position + new Vector2(_graphics.GraphicsDevice.Viewport.Width / 2f, _graphics.GraphicsDevice.Viewport.Height / 2f);

            _world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);
            base.Update(gameTime);
        }

        private void HandleGamePad()
        {
            GamePadState padState = GamePad.GetState(0);

            if (padState.IsConnected)
            {
                if (padState.Buttons.Back == ButtonState.Pressed)
                    Exit();

                if (padState.Buttons.A == ButtonState.Pressed && _oldPadState.Buttons.A == ButtonState.Released)
                    _circleBody.ApplyLinearImpulse(new Vector2(0, -10));

                _circleBody.ApplyForce(padState.ThumbSticks.Left);
                _cameraPosition.X -= padState.ThumbSticks.Right.X;
                _cameraPosition.Y += padState.ThumbSticks.Right.Y;

                _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));

                _oldPadState = padState;
            }
        }

        private void HandleKeyboard()
        {
            KeyboardState state = Keyboard.GetState();

            float scrollSpeed = 8.0f;

  

            if (state.IsKeyDown(Keys.Left))
            {
                ship.rotateLeft();
            }
            if (state.IsKeyDown(Keys.Right))
            {
                ship.rotateRight();
            }
            if (state.IsKeyDown(Keys.Up))
            {
                ship.accelerate();
            }

            if (state.IsKeyDown(Keys.Down))
            {
                ship.decelerate();
            }

            if (state.IsKeyDown(Keys.LeftControl))
            {
                ship.stop();
            }

            if (state.IsKeyDown(Keys.Z))
            {
                invisibility = 0.5f;
                //_ship. = (float)(Math.Cos(Math.PI * (gameTime.TotalGameTime.Milliseconds * 4) / 10000));
            }
            //map.ObjectGroups["events"].Objects["playership"].X += (int)(scrollx * scrollSpeed);
            //map.ObjectGroups["events"].Objects["playership"].Y -= (int)(scrolly * scrollSpeed);
           // _view = Matrix.CreateTranslation(new Vector3(_cameraPosition - _screenCenter, 0f)) * Matrix.CreateTranslation(new Vector3(_screenCenter, 0f));
            // We make it possible to rotate the circle body
            if (state.IsKeyDown(Keys.A))
                _cameraPosition.X += (int)(-1 * scrollSpeed);
            if (state.IsKeyDown(Keys.D))
                _cameraPosition.X += (int)(1 * scrollSpeed);
            if (state.IsKeyDown(Keys.S))
                _cameraPosition.Y -= (int)(-1 * scrollSpeed);
            if (state.IsKeyDown(Keys.W))
                _cameraPosition.Y -= (int)(1 * scrollSpeed);

            if (state.IsKeyDown(Keys.Escape))
                Exit();

            if (state.IsKeyDown(Keys.Space) && !_oldKeyState.IsKeyDown(Keys.Space))
            {
                ship.shoot();
            }
            _oldKeyState = state;
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

  /*          //Draw circle and ground
            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
            //_batch.Draw(_circleSprite, ConvertUnits.ToDisplayUnits(_circleBody.Position), null, Color.White, _circleBody.Rotation, _circleOrigin, 1f, SpriteEffects.None, 0f);
            _batch.Draw(_groundSprite, ConvertUnits.ToDisplayUnits(_groundBody.Position), null, Color.White, 0f, _groundOrigin, 1f, SpriteEffects.None, 0f);

            _batch.End();

            // Display instructions
            _batch.Begin();

            _batch.End();
            */
            base.Draw(gameTime);
            _batch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, _view);
    
            //map.Draw(_batch, new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), _cameraPosition);
            //_batch.Draw(_ship, _shipBody.Position, null, Color.White, 0f, _shipOrigin, 1f, SpriteEffects.None, 0f);
            //_batch.Draw(_ship, _shipBody.Position, null, Color.White * invisibility, _shipBody.Rotation, _shipOrigin, 1f, SpriteEffects.None, 0f);
            ship.Draw(_batch, invisibility);
            ship.displayEntityInfo(_batch, _font);
            _batch.End();
        }
    }
}