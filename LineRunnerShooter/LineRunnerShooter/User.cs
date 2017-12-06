﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LineRunnerShooter
{
    class User : ICollide 
    {
        //TODO: healt
        //TODO: Add 360 shooting requirement
        //TODO: bad guy sprite maken
        //TODO: user kan nog door platformen springen van onder naar boven, maar dan kan je niet naar links/rechts, Dit is op de moment dus niet meer mogelijk, mss wel best zo iets toelaten
        //TODO: maak users een paar pixels kleiner (zodat er geen collisieprobleem is
        //TODO: rename user to character

        protected int time; //for update 

        protected List<Texture2D> _texture; //textures
        protected Rectangle _spritePos;     //texture pos
        protected int _Action;              //actioin movement
        public Vector2 _Position;        //position
        protected Vector2 _Velocity;
        protected Vector2 _StartPos;
        protected MoveMethod _MoveMethod;   //movemethod 
        protected double gravity;
        protected int _lives;
        protected double slow;
        protected double maxSpeed;


        public bool canLeft;                //true = mag naar links
        public bool canRight;               //true = mag naar rechts
        public bool isGrounded;             //true = ik sta op de grond

        /*
        protected Rectangle _CollisionRight;   //for move right
        protected Rectangle _CollisionLeft;    //for move left
        protected Rectangle _CollisionRect; //hit box
        protected Rectangle feetCollisionRect; //bepaald isGrounded gebruik voor collisie met de grond
        */
        protected CollisionBox collisionBox;

        protected ARM arm;

        public Vector2 Location { get { return _Position; } }

        public User(Texture2D textureL, Texture2D textureR, MoveMethod move, Texture2D bullet)
        {
            _texture = new List<Texture2D>();
            _texture.Add(textureL);
            _texture.Add(textureR);
            _Action = 0;
            _Position = new Vector2(0, 500);
            time = 0;
            _MoveMethod = move;
            _spritePos = new Rectangle(100, 190, 100, 200);
            isGrounded = false;
            canLeft = true;
            canRight = true;
            _lives = 5;
            _StartPos = new Vector2(500, 300);
            slow = 30;
            collisionBox = new CollisionBox(Convert.ToInt16(_Position.X), Convert.ToInt16(_Position.Y), _spritePos.Width, _spritePos.Height);
            _Velocity = new Vector2(0,0);
            gravity = 9.81;
            maxSpeed = 15;
        }

        public virtual void Update(GameTime gameTime, KeyboardState stateKey)
        {
            double totalTime = gameTime.ElapsedGameTime.TotalMilliseconds;
            time += Convert.ToInt32(totalTime);
            if (time > 20)
            {
                time = 0;
                spritePosUpdate();
            }

            UpdateFI(totalTime);
            if (_Position.Y > 2500 && (_Position.X >300))
            {
                Reset();
            }
            collisionBox.Update(_Position.ToPoint());
        }

        protected virtual void spritePosUpdate()
        {
            _spritePos.X += 100;
            if (_spritePos.X > 700)
            {
                _spritePos.X = 0;
            }
        }

        public virtual void Update(GameTime gameTime, KeyboardState stateKey, MouseState mouseState, Vector2 camPos)
        {
            Vector2 mousePos = mouseState.Position.ToVector2();
            collisionBox.Update(_Position.ToPoint());
            Update(gameTime, stateKey);
        }

        public virtual void UpdateFI(double dt)
        {
            
            MoveHorizontal(dt);
            MoveVertical(dt);
            _Position += _Velocity;
        }

        public virtual void draw(SpriteBatch spriteBatch)
        {

            spriteBatch.Draw(_texture[_Action], _Position, _spritePos, Color.White);
            //spriteBatch.Draw(_texture[0], collisionBox.Left, _spritePos, Color.Red);
            //spriteBatch.Draw(_texture[0], collisionBox.Right, _spritePos, Color.Red);
            //spriteBatch.Draw(_texture[0], collisionBox.Feet, _spritePos, Color.Blue);
        }

        protected virtual void MoveHorizontal(double time)
        {
            double speed = Math.Abs(_Velocity.X) + time/slow;
            if(speed > maxSpeed) { speed = maxSpeed; }
            switch (_MoveMethod.Movedir)
            {
                case (0):
                    _Velocity.X = -(float)(speed);
                    _Action = 0;
                    break;
                case (1):
                    _Velocity.X = (float)(speed);
                    _Action = 1;
                    break;
                default:
                    if (Math.Abs(_Velocity.X) < 1)
                    {
                        _Velocity.X = 0;
                    }
                    else
                    {
                        _Velocity.X /= 1.2F;
                    }
                    break;
            }
            if (!isGrounded)
            {
                _Velocity.X = _Velocity.X * 0.95f;
            }
        }

        public virtual void MoveVertical()
        {
            if (!isGrounded)
            {
                _Velocity.Y += 1;
            }
        }

        public virtual void MoveVertical(double time)
        {
            if (!isGrounded)
            {
                _Velocity.Y += (float)((time / 400.0)*gravity); 
            }
            else { _Velocity.Y = 0; }
            if (_Velocity.Y > 20) { _Velocity.Y = 20; }

        }

        public Rectangle getCollisionRectagle()
        {
            return collisionBox.Body;
        }

        public virtual Rectangle getFeetCollisionRect()
        {
            return collisionBox.UnderFeet;
        }

        public Rectangle getRightCollision()
        {
            return collisionBox.Right;
        }

        public Rectangle getLeftCollision()
        {
            return collisionBox.Left;
        }

        public void PlatformUpdate(int platform)
        {
            _Position.Y += platform; 
        }

        public bool getBulletsCollision(Rectangle target)
        {
            bool isHit = false;
            foreach (Bullet b in arm.bullets)
            {
                if (target.Intersects(b.getCollisionRectagle()))
                {
                    isHit = true;
                    b.hitTarget();
                }
            }
            return isHit;
        }
        /*
        public virtual List<Rectangle> getBulletsRect()
        {
            return arm.getBulletsRect();
        }
        */

        public virtual void Reset()
        {
            _Position = _StartPos;
            _lives--;
        }

        public virtual void checkEnviroments(List<Rectangle> level)
        {

            isGrounded = false;
            canLeft = true;
            canRight = true;

            bool hitHead = false;

            foreach(Rectangle rect in level)
            {
                
                if(rect.Intersects(collisionBox.UnderFeet))
                {
                    isGrounded = true;
                }
                if (rect.Intersects(collisionBox.Feet) && isGrounded) //TODO: Lift glitcht weer
                {
                    _Position.Y -= 1;
                    _Velocity.Y = 0;
                    //isGrounded = true;
                }
                if (rect.Intersects(getLeftCollision())&&canLeft)
                {
                    canLeft = false;
                    _Position.X += Math.Abs(_Velocity.X);
                }
                if (rect.Intersects(getRightCollision())&&canRight)
                {
                    canRight = false;
                    _Position.X -= Math.Abs(_Velocity.X);
                }
                if(rect.Intersects(collisionBox.Head) && !hitHead)
                {
                    hitHead = true;
                    _Velocity.Y = Math.Abs(_Velocity.Y);
                }
                
            }
        }

        public void checkHit(List<Bullet> bullets)
        {
            for (int i = 0; i < bullets.Count; i++)
            {
                if (collisionBox.Body.Intersects(bullets[i].getCollisionRectagle()))
                {
                    takeDamage(bullets[i].hitTarget());
                }
            }

        }

        protected virtual void takeDamage(int damage)
        {
            _lives -= damage;
        }

        public List<Rectangle> getBulletsRect()
        {
            return arm.getBulletsRect();
        }
    }

    public abstract class MoveMethod
    {
        protected int movedir;
        public bool isJump;
        public bool isShooting;

        public int Movedir { get { return movedir; } protected set { movedir = value; } }

        public abstract void Update(KeyboardState keyState, MouseState mouseState, bool canLeft, bool canRight);
    }
}
