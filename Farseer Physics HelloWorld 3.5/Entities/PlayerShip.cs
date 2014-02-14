using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhaseShift.Components;
using Microsoft.Xna.Framework;
using FarseerPhysics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Artemis;

namespace PhaseShift.Entities
{
    public class PlayerShip
    {
        private static Texture2D ship = null;
        private Health health = new Health();
        private Energy energy = new Energy();
        private Body shipBody;
        private Vector2 shipOrigin;
        string EntityInfo;
        private float invisibility;
        List<Bullet> bullets = new List<Bullet>();
        int fired = 0;
        World world;
        ContentManager contentManager;
        SpriteBatch spriteBatch;
        SpriteFont font;
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


        public PlayerShip(World world, ContentManager contentManager)
        {
            this.contentManager = contentManager;
            this.world = world;
            health.SetHealth(30f);
            energy.SetEnergy(30f);
            if (ship == null)
            {
                ship = contentManager.Load<Texture2D>("ShipSprite");
            }
            Vector2 shipOrigin = new Vector2(ship.Width / 2f, ship.Height / 2f);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(64f);
            Vector2 shipPosition = new Vector2(50f, 50f);
            shipBody = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(ship.Width / 2), ConvertUnits.ToSimUnits(ship.Height), 1);
            shipBody.Position = shipPosition;
            shipBody.BodyType = BodyType.Dynamic;
            shipBody.Mass = 2;
            shipBody.AngularDamping = 0;

            // _shipBody.Rotation = (float) Math.PI / 2;
            shipBody.Restitution = 0.01f;
            shipBody.Friction = 0.1f;
            shipBody.SleepingAllowed = false;
        }
        public void Draw(SpriteBatch spriteBatch, float invisibility)
        {
            this.spriteBatch = spriteBatch;
            spriteBatch.Draw(ship, shipBody.Position, null, Color.White * invisibility, shipBody.Rotation, shipOrigin, 1f, SpriteEffects.None, 0f);
            foreach (Bullet b in bullets)
            {
                b.Draw(spriteBatch, font);
            }
        }

        public Vector2 getPosition()
        {
            return shipBody.Position;
        }

        public float getRotation()
        {
            return shipBody.Rotation;
        }

        public void accelerate()
        {
            Vector2 vel = shipBody.LinearVelocity;
            int desiredVelocity = 1;
            float velChange = desiredVelocity;
            float impulse = shipBody.Mass * velChange;
            shipBody.ApplyLinearImpulse(CreateVector2(shipBody.Rotation, impulse), shipBody.WorldCenter);
        }

        public void decelerate()
        {
            Vector2 vel = shipBody.LinearVelocity;
            int desiredVelocity = 1;
            float velChange = -desiredVelocity;
            float impulse = shipBody.Mass * velChange;
            shipBody.ApplyLinearImpulse(CreateVector2(shipBody.Rotation, impulse), shipBody.WorldCenter);
        }

        public void stop()
        {
            shipBody.LinearVelocity = Vector2.Zero;
        }

        public void rotateLeft()
        {
            shipBody.Rotation -= .05f;
        }

        public void rotateRight()
        {
            shipBody.Rotation += .05f;
        }

        public void displayEntityInfo(SpriteBatch spriteBatch, SpriteFont font)
        {
            this.font = font;
            EntityInfo = "Current Ship Rotation: " + shipBody.Rotation + "\nCurrent Ship Linear Velocity: " + shipBody.LinearVelocity + 
            "\nCurrent Ship Position: " + shipBody.Position + "\nInvisibility: " + invisibility + "\nFired: " + fired;
            spriteBatch.DrawString(font, EntityInfo, this.getPosition(), Color.White);
        }

        public void shoot()
        {
            fired++;
            bullets.Add(new Bullet(world, contentManager, this));
        }
    }
}
