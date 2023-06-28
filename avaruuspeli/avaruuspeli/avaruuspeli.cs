using System;
using System.Collections.Generic;
using System.Net.Mime;
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
    private Image _vihollisaluksenKuva = LoadImage("VihollisAlus.png");
    private Image _ammus = LoadImage("Ammus.png");
    private Image _asteroidi = LoadImage("Asteroidi.png");
    private Image _liekki = LoadImage("Liekki.png");
    private Image _savu = LoadImage("Savu.png");
    
    AssaultRifle _ase;
    
    private PhysicsObject _alus;
    
    public override void Begin()
    {
        FixedTimeStep = true;
        Kentta();
        Alus();
        VihollisAlukset();
        Asteroidit();
        //AloitusNayttö();
        LuoAikalaskuri();
        Ohjaukset();
    }
    private void Kentta()
    {
        Level.Background.Color = Color.Black;
        Level.Background.Image = _avaruusTausta;
        Level.Background.TileToLevel();
    }
    
    private void Ohjaukset()
    {
        Mouse.ListenMovement(0.1, KuunteleLiiketta, null);
        Mouse.Listen(MouseButton.Left, ButtonState.Down, AmmuAseella, "Ammu", _ase);
        
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, Paussilla, "Paussittaa pelin");
        Keyboard.Listen(Key.F1, ButtonState.Pressed,ShowControlHelp, "Näytä ohjeet");
    }
    
    private void Alus()
    {
        _alus = new PhysicsObject(32, 32);

        _aluksenKuva.Scaling = ImageScaling.Nearest;
        _alus.Image = _aluksenKuva;

        Add(_alus);

        Camera.ZoomFactor = 2;
        Camera.Follow(_alus);

        _ase = new AssaultRifle(0, 0);
        _ase.FireRate = 7;
        _ase.ProjectileCollision = AmmusOsui;
        _alus.Add(_ase);
        _alus.RelativePosition = new Vector(_alus.Width / 2, 0);
        
        
        Mouse.ListenMovement(0.1, Tahtaa, "Tähtää aseella");
    }
    void Tahtaa()
    {
        Vector suunta = (Mouse.PositionOnWorld - _alus.AbsolutePosition).Normalize();
        _ase.Angle = suunta.Angle;
        _alus.Angle = suunta.Angle + Angle.FromDegrees(-135);
    }

    private void VihollisAlukset()
    {
        PhysicsObject vihollisenAlus = new PhysicsObject(32, 32);
        _vihollisaluksenKuva.Scaling = ImageScaling.Nearest;
        vihollisenAlus.Image = _vihollisaluksenKuva;
        vihollisenAlus.X = 100;
        Add(vihollisenAlus);
    }

    private void Asteroidit()
    {
        PhysicsObject asteroidi = new PhysicsObject(200, 200);
        _asteroidi.Scaling = ImageScaling.Nearest;
        asteroidi.Image = _asteroidi;
        asteroidi.X = 200;
        asteroidi.Mass = 300;
        Add(asteroidi);
    }

    

    private void Jatka()
    {
        Pause();
    }
    
    private void KuunteleLiiketta()
    {
        //Vector paikkaRuudulla = Mouse.PositionOnWorld;
        //_alus.MoveTo(paikkaRuudulla, 500);

        Vector hiiri = Mouse.PositionOnWorld - _alus.Position;
        _alus.MoveTo(hiiri * Int32.MaxValue, 100);

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
    void LuoAikalaskuri()
    {
        Timer aikalaskuri = new Timer();
        aikalaskuri.Start();

        Label aikanaytto = new Label();
        aikanaytto.TextColor = Color.White;
        aikanaytto.DecimalPlaces = 0;
        aikanaytto.BindTo(aikalaskuri.SecondCounter);
        Add(aikanaytto);

        aikanaytto.X = -400;
        aikanaytto.Y = 300;
    }
    
    private void Paussilla()
    {
        Pause();
        MultiSelectWindow paussiMenu = new MultiSelectWindow("Paussilla", "Jatka", "Lopeta");
        paussiMenu.AddItemHandler(0, Jatka);
        paussiMenu.AddItemHandler(1, Exit);
        paussiMenu.DefaultCancel = 0;
        Add(paussiMenu);
        
        paussiMenu.Color = Color.DarkGray;
        paussiMenu.SetButtonColor(Color.MidnightBlue);
    }
    
    private void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
    }
    
    void AmmuAseella(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            ammus.Size *= 2;
            ammus.Image = _ammus;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
        }
    }
}



class Vihollinen : PhysicsObject
{
    private IntMeter _elamalaskuri = new IntMeter(3, 0, 3);
    public  IntMeter Elamalaskuri { get { return _elamalaskuri; } }

    public Vihollinen(double leveys, double korkeus)
        : base(leveys, korkeus)
    {
        _elamalaskuri.LowerLimit += delegate { this.Destroy(); };
    }
}