﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineRunnerShooter
{
    class ARM
    {
        private Texture2D pixel;
        private float angle;
        private Vector2 _position;
        public List<Bullet> bullets;

        public ARM(Texture2D pix, Texture2D energy)
        {
            angle = 0;
            pixel = pix;
            _position = new Vector2(200, 240);
            bullets = new List<Bullet>();
            bullets.Add(new Bullet(energy));
            bullets.Add(new Bullet(energy));
            bullets.Add(new Bullet(energy));
            bullets.Add(new Bullet(energy));
        }
        public void Update(GameTime gameTime, Vector2 position, Vector2 mouse)
        {
            _position = position;
            _position.X += 50;
            _position.Y += 65;

            float xVers =  -mouse.X + _position.X;
            float yVers =  -mouse.Y + _position.Y;
            angle = (float)Math.Atan2(xVers,yVers) + (float) (Math.PI/2);

            foreach(Bullet b in bullets)
            {
                b.Update(gameTime);
            }
        }

        public void Update(GameTime gameTime, Vector2 position, int dir)
        {
            _position = position;
            _position.X += 50;
            _position.Y += 65;
            if(dir == 0)
            {
                angle = (float)(Math.PI);
            }
            else if(dir == 1)
            {
                angle = 0;
            }
            foreach (Bullet b in bullets)
            {
                b.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, 81, 36);
            Vector2 origin = new Vector2(5, 10);

            foreach (Bullet b in bullets)
            {
                b.Draw(spriteBatch);
            }

            spriteBatch.Draw(pixel, _position, sourceRectangle, Color.White, -angle, origin, 1.0f, SpriteEffects.None, 1);
        }

        public void Fire()
        {
            //bullet.fire(angle, _position);
            Console.WriteLine("checking bullet");
            int i = 0;
            while((i != -1))
            {
                Console.WriteLine("searching");
                if (!bullets[i].isFired)
                {
                    bullets[i].fire(angle, _position);
                    Console.WriteLine("bullet Fired");
                    i = -1;
                }
                else
                {
                    i++;
                    if(bullets.Count <= i)
                    {
                        i = -1;
                        Console.WriteLine("bullet not available");
                    }
                }
            }
        }

        public List<Rectangle> getBulletsRect()
        {
            List<Rectangle> bulletsRect = new List<Rectangle>();
            foreach (Bullet b in bullets)
            {
                bulletsRect.Add(b.getCollisionRectagle());
            }
            return bulletsRect;
        }
    }

    class RobotARM
    {
        private Texture2D pixel;
        private float angle;
        private Vector2 _position;
        bool isAttacking;

        public RobotARM(Texture2D pix)
        {
            angle = 0;
            pixel = pix;
            _position = new Vector2(200, 240);
        }
        public void Update(GameTime gameTime, Vector2 position, int dir)
        {
            _position = position;
            _position.X += 50;
            _position.Y += 50;
            if (isAttacking)
            {
                angle += (float)((Math.PI / 180)*gameTime.ElapsedGameTime.TotalMilliseconds*2);
            }
            else
            {
                angle = (float)((Math.PI) * (dir-1));
            }
            isAttacking = false;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, 81, 36);
            Vector2 origin = new Vector2(5, 10);
            spriteBatch.Draw(pixel, _position, sourceRectangle, Color.White, angle, origin, 1.0f, SpriteEffects.None, 1);
            
        }

        public void Fire()
        {
            isAttacking = true;
        }

        public Rectangle attackBox()
        {
            int size = 80;
            Rectangle attack = new Rectangle(0,0,size,size);

            if(Math.Cos(angle) > 0 && Math.Sin(angle) > 0)
            {
                attack.Location = _position.ToPoint() + new Point(0, -size);
            }
            else if (Math.Cos(angle) <= 0 && Math.Sin(angle) > 0)
            {
                attack.Location = _position.ToPoint() + new Point(-size, -size);
            }
            else if (Math.Cos(angle) <= 0 && Math.Sin(angle) <= 0)
            {
                attack.Location = _position.ToPoint() + new Point(-size, 0);
            }
            else if (Math.Cos(angle) > 0 && Math.Sin(angle) <= 0)
            {
                attack.Location = _position.ToPoint();
            }

            return attack;
        }

    }
}

