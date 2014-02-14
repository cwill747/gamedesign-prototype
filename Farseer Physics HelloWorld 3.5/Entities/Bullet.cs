using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics;

namespace PhaseShift.Entities
{
    public class Bullet
    {
        static Texture2D bullet = null;
        private Vector2 position;
        private Body body;
        private Vector2 origin;
        private SpriteFont font;

        public static Vector2 CreateVector2(float angle, float length)
        {
            return new Vector2((float)Math.Cos(angle) * length, (float)Math.Sin(angle) * length);
        }

        public Bullet(World world, ContentManager contentManager, PlayerShip origination)
        {
            if (bullet == null)
            {
                bullet = contentManager.Load<Texture2D>("bullet");
            }
            origin = origination.getPosition();
            position = origin;
            body = BodyFactory.CreateRectangle(world, ConvertUnits.ToSimUnits(bullet.Width / 2), ConvertUnits.ToSimUnits(bullet.Height / 2), 1);
            body.Position = origin;
            body.BodyType = BodyType.Dynamic;
            body.Mass = 1;
            body.AngularDamping = 0;
            body.Restitution = 0f;
            body.ApplyLinearImpulse(CreateVector2(origination.getRotation(), 200f));
        }

        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            string EntityInfo = "Bullet Origination: " + origin + "\nBullet Current Position: " + body.Position;
            spriteBatch.Draw(bullet, body.Position, null, Color.White, 0, ConvertUnits.ToSimUnits(origin), 1f, SpriteEffects.None, 1f);
            spriteBatch.DrawString(font, EntityInfo, body.Position, Color.White);

        }
    }
}
