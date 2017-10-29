// Origianl: Aviad & Ori
// Author: Raz 

using System;
using UnityEngine;
using System.Collections.Generic;
using StarWars.Actions;
using Infra.Utils;
using NUnit.Framework;
using StarWars.UI;
using UnityEditor;
using UnityEngine.Windows.Speech;
using Action = StarWars.Actions.Action;

namespace StarWars.Brains {
public class CyberShipBrain : SpaceshipBrain {
    public override string DefaultName {
        get {
            return "CyberShip";
        }
    }
    public override Color PrimaryColor {
        get {
            return new Color((float)0xFF, (float)0x00, (float)0x00, 0x11);
        }
    }
    public override SpaceshipBody.Type BodyType {
        get {
            return SpaceshipBody.Type.TieFighter;
        }
    }

    [SerializeField] private const float SafeDistance = 3f;
    [SerializeField] private const float ShootingDistance = 6f;
    [SerializeField] private const float ShieldUpTime = 10f;
    [SerializeField] private const float MyShotLimit = 25f;
    private float _timer;

    private Action RaiseShield()
    {
        _timer = ShieldUpTime;
        return ShieldUp.action;
    }
    
    public override Action NextAction()
    {
        var forwardVector = spaceship.Forward;
            
        // Respond to threats
        if (IsBeingShotAt()) {
            // Raise shield if got enough energy
            if (spaceship.CanRaiseShield)
            {
                return RaiseShield();
            } 
            // Otherwise RUN FOR YOUR LIFE!!!
            else {
                // Determine best direction for escape
                var nearestShot = FindNearest(Space.Shots, IsNotMyShot);
                var escapeAngle = nearestShot.Forward.GetAngle(forwardVector);
                return TurnRight.action;
            }
        }
        
        // Relax shield if back to safety
        if (spaceship.IsShieldUp) {
            --_timer;
            if (_timer <= 0 && !IsBeingShotAt()) {
                return ShieldDown.action;
            }
        }
        
        // Find a tasty pray
        var nearestShip = FindNearsetShip();
        if (nearestShip == null) {
            return DoNothing.action;
        }
        
        // Look for nearest opponent
        var toNearestVector = spaceship.ClosestRelativePosition(nearestShip);
        var targetVector = toNearestVector;
        var angle = targetVector.GetAngle(forwardVector);
        
        // Opponent found! Hit or Run?
        if (nearestShip.IsShieldUp)
        {
            // He's ready for us - better not mess with the dude!
            if (angle >= 10f) 
            {
                return TurnRight.action;
            } 
            else if (angle <= -10f)
            {
                return TurnLeft.action;
            }
        }
        
        // Hit the motherfucker!
        if (toNearestVector.magnitude <= SafeDistance && !spaceship.IsShieldUp && spaceship.CanRaiseShield)
        {
            return RaiseShield();
        } 
        // Shoot the bastard!
        else if (!nearestShip.IsShieldUp && spaceship.CanShoot && toNearestVector.magnitude < ShootingDistance) 
        {
            return Shoot.action;
        } 
        // Chase the coward!
        else if (angle >= 10f) 
        {
            return TurnLeft.action;
        } 
        else if (angle <= -10f) 
        {
            return TurnRight.action;
        } 
        // Wonder around until something interesting happens
        else 
        {
            return DoNothing.action;
        }
    }

    private Spaceship FindNearsetShip() {
        return FindNearest(Space.Spaceships, IsNotMe);
    }

    private bool IsBeingShotAt() {
        var nearestShot = FindNearest(Space.Shots, IsNotMyShot);
        if (nearestShot != null) {
            return spaceship.ClosestRelativePosition(nearestShot).magnitude <= SafeDistance;
        }
        return false;
    }


    private T FindNearest<T>(IList<T> list, System.Func<T ,bool> predicate = null) where T : SpaceObject {
        T nearest = null;
        var minDistance = float.MaxValue;
        foreach (var obj in list) {
            if (!obj.IsAlive) continue;
            var distance = spaceship.ClosestRelativePosition(obj).magnitude;
            if ((nearest == null
                    || distance < minDistance)
                    && (predicate == null || predicate(obj))) {
                nearest = obj;
                minDistance = distance;
            }
        }
        return nearest;
    }

    private bool IsNotMyShot(Shot shot) {
        var angleToShot = shot.Forward.GetAngle(spaceship.Forward);
        return -MyShotLimit > angleToShot || angleToShot > MyShotLimit;
    }

    private bool IsNotMe(Spaceship ship) {
        return ship != spaceship;
    }
}
}
