#region File Description
//-----------------------------------------------------------------------------
// TripleLaserPowerUp.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace VectorRumble
{
    /// <summary>
    /// A power-up that gives a player a triple-laser-shooting weapon.
    /// </summary>
    class TripleLaserPowerUp : PowerUp
    {
        #region Initialization
        /// <summary>
        /// Constructs a new triple-laser power-up.
        /// </summary>
        /// <param name="world">The world that this power-up belongs to.</param>
        public TripleLaserPowerUp(World world)
            : base(world) 
        {
            this.color = Color.CornflowerBlue;
        }
        #endregion

        #region Interaction
        /// <summary>
        /// Defines the interaction between this power-up and a target actor
        /// when they touch.
        /// </summary>
        /// <param name="target">The actor that is touching this object.</param>
        /// <returns>True if the objects meaningfully interacted.</returns>
        public override bool Touch(Actor target)
        {
            // if we hit a ship, give it the weapon
            Ship ship = target as Ship;
            if (ship != null)
            {
                ship.SetWeapon(new TripleLaserWeapon(ship));
            }

            return base.Touch(target);
        }
        #endregion
    }
}
