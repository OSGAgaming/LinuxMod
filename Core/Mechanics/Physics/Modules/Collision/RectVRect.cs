
/*using Flipsider.Engine.Input;
using Flipsider.Engine.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace LinuxMod.Core.Mechanics
{
    public partial class Collideable : IEntityModifier
    {
        public void RectVRect(Collideable collideable1, Collideable collideable2)
        {
            if ((collideable2.isStatic && collideable2.BindableEntity.Active) || !collideable2.HasBindableEntity)
            {
                CollisionInfo CI =
                    Collision.AABBResolvePoly(
                    collideable1.collisionBox,
                    collideable1.lastCollisionBox,
                    collideable2.collisionBox);

                collideable1.collisionInfo.AABB = CI.AABB;
                collideable1.BindableEntity.position += CI.d;
                if (collideable1.BindableEntity is LivingEntity)
                {
                    var LivingEntity = (LivingEntity)collideable1.BindableEntity;
                    if (CI.AABB != Bound.None)
                    {
                        switch (collideable1.collisionInfo.AABB)
                        {
                            case (Bound.Top):
                                {
                                    LivingEntity.onGround = true;
                                    LivingEntity.velocity.Y = 0;
                                    break;
                                }
                            case (Bound.Bottom):
                                {
                                    LivingEntity.velocity.Y = 0;
                                    break;
                                }
                            case (Bound.Left):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                            case (Bound.Right):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                        }
                        LivingEntity.isColliding = true;
                    }
                }
            }
        }
        public void RectVTri(Collideable collideable1, Collideable collideable2)
        {
            if ((collideable2.isStatic && collideable2.BindableEntity.Active) || !collideable2.HasBindableEntity)
            {
                CollisionInfo CI =
                    Collision.SAT(
                    collideable1.collisionBox,
                    collideable2.collisionBox);

                collideable1.collisionInfo.AABB = CI.AABB;
                collideable1.BindableEntity.position -= CI.d;
                if (collideable1.BindableEntity is LivingEntity)
                {
                    var LivingEntity = (LivingEntity)collideable1.BindableEntity;
                    if (CI.AABB != Bound.None)
                    {
                        switch (collideable1.collisionInfo.AABB)
                        {
                            case (Bound.Top):
                                {
                                    LivingEntity.onGround = true;
                                    break;
                                }
                            case (Bound.Bottom):
                                {
                                   LivingEntity.velocity.Y = 0;
                                    break;
                                }
                            case (Bound.Left):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                            case (Bound.Right):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                        }
                        LivingEntity.isColliding = true;
                    }
                }
            }
        }
        public void RectVPoly(Collideable collideable1, Collideable collideable2)
        {
            if ((collideable2.isStatic && collideable2.BindableEntity.Active) || !collideable2.HasBindableEntity)
            {
                if (collideable1.BindableEntity is LivingEntity)
                {
                    var LivingEntity = (LivingEntity)collideable1.BindableEntity;

                CollisionInfo CI =
                    Collision.Raycast(
                    collideable1.collisionBox,
                    collideable2.collisionBox,100,Main.player.height/2);

                collideable1.collisionInfo.AABB = CI.AABB;

                
                    if (CI.AABB != Bound.None)
                    {
                        switch (collideable1.collisionInfo.AABB)
                        {
                            case (Bound.Top):
                                {
                                    LivingEntity.onGround = true;
                                    LivingEntity.onSlope = true;
                                    LivingEntity.velocity.Y = 0;
                                    break;
                                }
                            case (Bound.Bottom):
                                {
                                    LivingEntity.velocity.Y = 0;
                                    break;
                                }
                            case (Bound.Left):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                            case (Bound.Right):
                                {
                                    LivingEntity.velocity.X = 0;
                                    break;
                                }
                        }
                        collideable1.BindableEntity.position += CI.d;
                        LivingEntity.isColliding = true;
                    }
                }
            }
        }
    }
}
*/