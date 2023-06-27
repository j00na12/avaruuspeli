using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace avaruuspeli;

public class avaruuspeli : PhysicsGame
{
    private Image avaruusTausta = LoadImage("AvaruusTausta.png");
    private Image alusKuva = LoadImage("Alus.png");

    private PhysicsObject _alus;
    private ListenContext ohjaimet;
    public override void Begin()
    {
        Alus();
        Kentta();
        Ohjaukset();

        PhysicsObject kuutio = new PhysicsObject(32, 32);
        kuutio.Shape = Shape.Diamond;
        kuutio.X = 100;
        Add(kuutio);
    }

    private void Alus()
    {
        _alus = new PhysicsObject(32, 32);
        _alus.Shape = Shape.FromImage(alusKuva);
        
        Add(_alus);

        Camera.ZoomFactor = 2;
        Camera.Follow(_alus);
    }

    private void Ohjaukset()
    {
        ohjaimet = ControlContext.CreateSubcontext();
        
        //Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Eteenpain, "Liiku hiiren suuntaan.");
        //Mouse.Listen(MouseButton.Left, ButtonState.Released, Pysaytys, null);
        Mouse.Listen(MouseButton.Right, ButtonState.Down, Ammu, "Ammu hiiren suuntaan.");
        Mouse.ListenMovement(0.1, KuunteleLiiketta, null); //.InContext(ohjaimet);

    }

    private void Kentta()
    {
        Level.Background.Color = Color.Black;
    }

    private void Eteenpain()
    {
        ohjaimet.Enable();
    }

    private void Pysaytys()
    {
        ohjaimet.Disable();
    }

    private void Ammu()
    {
        
    }
    private void KuunteleLiiketta()
    {
        //Vector paikkaRuudulla = Mouse.PositionOnWorld;
        //_alus.MoveTo(paikkaRuudulla, 500);

        Vector hiiri = Mouse.PositionOnWorld - _alus.Position;
        _alus.MoveTo(hiiri * Int32.MaxValue, 300);

    }
}