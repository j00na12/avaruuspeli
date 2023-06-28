using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;

namespace avaruuspeli;

public class avaruuspeli : PhysicsGame
{
    private Image _avaruusTausta = LoadImage("AvaruusTausta.png");
    private Image _aluksenKuva = LoadImage("Alus.png");

    private PhysicsObject _alus;
    //private ListenContext ohjaimet;
    
    public override void Begin()
    {
        //AloitusNayttö();
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
        
        _aluksenKuva.Scaling = ImageScaling.Nearest;
        _alus.Image = _aluksenKuva;
        
        Add(_alus);

        Camera.ZoomFactor = 2;
        Camera.Follow(_alus);

        Angle aluksenKulma = _alus.Angle;
        
    }

    private void Ohjaukset()
    {
        //ohjaimet = ControlContext.CreateSubcontext();
        
        //Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Eteenpain, "Liiku hiiren suuntaan.");
        //Mouse.Listen(MouseButton.Left, ButtonState.Released, Pysaytys, null);
        Mouse.Listen(MouseButton.Right, ButtonState.Down, Ammu, "Ammu hiiren suuntaan.");
        Mouse.ListenMovement(0.1, KuunteleLiiketta, null); //.InContext(ohjaimet);
        
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, Paussilla, "Paussittaa pelin");
    }

    private void Kentta()
    {
        Level.Background.Color = Color.Black;
        Level.Background.Image = _avaruusTausta;
        Level.Background.TileToLevel();
    }

    /* private void Eteenpain()
    {
        ohjaimet.Enable();
    }

    private void Pysaytys()
    {
        ohjaimet.Disable();
    } */
    private void Paussilla()
    {
        Pause();
        MultiSelectWindow paussiMenu = new MultiSelectWindow("Paussilla", "Jatka", "Parhaat pisteet", "Lopeta");
        paussiMenu.AddItemHandler(0, Jatka);
        paussiMenu.AddItemHandler(2, Exit);
        paussiMenu.DefaultCancel = 0;
        Add(paussiMenu);
        
        paussiMenu.Color = Color.DarkGray;
        paussiMenu.SetButtonColor(Color.MidnightBlue);
    }

    private void Jatka()
    {
        Pause();
    }
    private void Ammu()
    {
        
    }
    
    private void KuunteleLiiketta()
    {
        //Vector paikkaRuudulla = Mouse.PositionOnWorld;
        //_alus.MoveTo(paikkaRuudulla, 500);

        Vector hiiri = Mouse.PositionOnWorld - _alus.Position;
        _alus.MoveTo(hiiri * Int32.MaxValue, 200);

    }

    /*private void AloitusNayttö()
    {
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Avaruus Peli!", "Aloita peli", "Parhaat pisteet", "Lopeta");
        Add(alkuvalikko);
        
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        alkuvalikko.AddItemHandler(1, ParhaatPisteet);
        alkuvalikko.AddItemHandler(2, Exit);
        alkuvalikko.DefaultCancel = -1;
        
        alkuvalikko.Color = Color.DarkGray;
        alkuvalikko.SetButtonColor(Color.MidnightBlue);
    }

    private void AloitaPeli()
    {
        
    }

    private void ParhaatPisteet()
    {
        
    }*/
}