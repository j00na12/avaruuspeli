using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace avaruuspeli;

public class avaruuspeli : PhysicsGame
{
    private PhysicsObject _alus;
    public override void Begin()
    {
        Alus();
        Kentta();
        Ohjaukset();

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }

    private void Alus()
    {
        _alus = new PhysicsObject(50, 50);
        _alus.Shape = Jypeli.Shape.Circle;
        _alus.Color = Color.Red;
        Add(_alus);
    }

    private void Ohjaukset()
    {
        Mouse.Listen(MouseButton.Right, ButtonState.Down, Eteenpain, "Liiku hiiren suuntaan.");
        Mouse.Listen(MouseButton.Right, ButtonState.Up, Pysaytys, null);
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Ammu, "Ammu hiiren suuntaan.");
        
    }

    private void Kentta()
    {
        Level.Background.Color = Color.Black;
    }

    private void Eteenpain()
    {
        Mouse.ListenMovement(0.1, KuunteleLiiketta, null);
    }

    private void Pysaytys()
    {
        _alus.X = _alus.X;
        _alus.Y = _alus.Y;
    }

    private void Ammu()
    {
        
    }
    void KuunteleLiiketta()
    {        
        _alus.X = Mouse.PositionOnWorld.X;
        _alus.Y = Mouse.PositionOnWorld.Y;

        Vector hiirenLiike = Mouse.MovementOnWorld; // tai Mouse.MovementOnScreen
        // tähän hiiren liikevektorin hyödyntäminen
    }
}