using System;
using System.Collections.Generic;
using System.Net.Mime;
using FarseerPhysics.Dynamics;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;

namespace avaruuspeli;

// Versio 1.0

public class avaruuspeli : PhysicsGame
{
    private Image _avaruusTausta = LoadImage("AvaruusTausta.png");
    private Image _aluksenKuva = LoadImage("Alus.png");
    private Image _vihollisaluksenKuva = LoadImage("VihollisAlus.png");
    private Image _ammus = LoadImage("Ammus.png");
    private Image _asteroidi = LoadImage("Asteroidi.png");
    private Image _liekki = LoadImage("Liekki.png");
    private Image _violettiAlus = LoadImage("HienoViolettiAlus.png");
    private EasyHighScore _topLista = new EasyHighScore();
    private AssaultRifle _ase;
    private IntMeter _pistelaskuri;
    private PhysicsObject _alus;
    private int _intervaliaNostettu = 0;
    private Timer _vihuSpawni;

    public override void Begin()
    {
        IsFullScreen = true;
        FixedTimeStep = true;
        Kentta();
        Alus();
        VihollisAlukset();
        Asteroidit();
        AloitusNayttö();
        LuoAikalaskuri();
        LuoPistelaskuri();
        Ohjaukset();
    }
    private void Kentta()
    {
        Level.Background.Color = Color.Black;
        Level.Background.Image = _avaruusTausta;
        Level.Width = 20000;
        Level.Height = 20000;
        Camera.StayInLevel = true;
        Level.Background.TileToLevel();
        
        _vihuSpawni = new Timer()
        {
            Interval = 10,
        };
        _vihuSpawni.Timeout += VihollisAlukset;
        _vihuSpawni.Start();
        
        Timer asterpodiSpawni = new Timer()
        {
            Interval = 30,
        };
        asterpodiSpawni.Timeout += Asteroidit;
        asterpodiSpawni.Start();
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
        _alus = new AlusKuolee(32, 32);
        _alus.Tag = "hyvisKuolee";
        Add(_alus);
        
        _aluksenKuva.Scaling = ImageScaling.Nearest;
        _alus.Image = _aluksenKuva;
        
        Camera.ZoomFactor = 2;
        Camera.Follow(_alus);
        
        _ase = new AssaultRifle(0, 0)
        {
            FireRate = 7,
            ProjectileCollision = AmmusOsui,
        };
        
        _alus.Add(_ase);
        _alus.RelativePosition = new Vector(_alus.Width / 2, 0);
        _alus.CanRotate = false;
        
        Mouse.ListenMovement(0.1, Tahtaa, "Tähtää aseella");
        
        _alus.Destroyed += LisaaPelaajanPisteet;
    }
    
    void Tahtaa()
    {
        Vector suunta = (Mouse.PositionOnWorld - _alus.AbsolutePosition).Normalize();
        _ase.Angle = suunta.Angle;
        _alus.Angle = suunta.Angle + Angle.FromDegrees(-135);
    }
    
    private void KuunteleLiiketta()
    {
        Vector hiiri = Mouse.PositionOnWorld - _alus.Position;
        _alus.MoveTo(hiiri * Int32.MaxValue, 150);
    }
    
    private void VihollisAlukset()
    {
        double alusX = _alus.Position.X;
        double alusY = _alus.Position.Y;

        Vector randomSijainti = new Vector(RandomGen.NextDouble(alusX + 100, alusX + 300)* RandomGen.SelectOne(new int[2]{-1, 1}),
            RandomGen.NextDouble(alusY + 100, alusY + 300)* RandomGen.SelectOne(new int[2]{-1, 1}));
        
        Vihollinen vihollisenRunko = new Vihollinen(32, 32)
        {
            Color = Color.Transparent,
            Position = randomSijainti,
            Tag = "vihu",
        };
        Add(vihollisenRunko);
        
        PhysicsObject vihollisenAlus = new PhysicsObject(32, 32)
        {
            Image = RandomGen.SelectOne(_vihollisaluksenKuva, _violettiAlus),
            Position = vihollisenRunko.Position,
            Angle = Angle.FromDegrees(-135),
        };
        
        _vihollisaluksenKuva.Scaling = ImageScaling.Nearest;
        vihollisenRunko.Add(vihollisenAlus);
        
        FollowerBrain vihuAivot = new FollowerBrain(_alus);
        vihollisenRunko.Brain = vihuAivot;
        vihuAivot.TurnWhileMoving = true;
        vihuAivot.Speed = 70;

        AssaultRifle vihollistenAse = new AssaultRifle(0, 0)
        {
            FireRate = 1,
            ProjectileCollision = AmmusOsui,
            
        };
        Timer.CreateAndStart(1, delegate { VihollinenAmpuu(vihollistenAse); });
        
        vihollistenAse.Position = vihollisenRunko.Position + new Vector(0, 0);
        vihollisenRunko.Add(vihollistenAse);
        
        vihollisenRunko.CanRotate = false;
    }
    
    private void Asteroidit()
    {
        double alusX = _alus.Position.X;
        double alusY = _alus.Position.Y;
        
        Vector randomSijainti = new Vector(RandomGen.NextDouble(alusX + 200, alusX + 250)* RandomGen.SelectOne(new int[2]{-1, 1}),
            RandomGen.NextDouble(alusY + 200, alusY + 250)* RandomGen.SelectOne(new int[2]{-1, 1}));
        
        AsteroidiTuho asteroidi = new AsteroidiTuho(230, 230)
        {
            
            Position = randomSijainti,
            Mass = 300,
            Tag = "asteroiditag",
            CanRotate = false,
        };

        Add(asteroidi);
        asteroidi.Image = _asteroidi;
        _asteroidi.Scaling = ImageScaling.Nearest;
        
        RandomMoverBrain asteroidiLiike = new RandomMoverBrain();
        asteroidi.Brain = asteroidiLiike;
        asteroidiLiike.Speed = 10;
        asteroidiLiike.ChangeMovementSeconds = 5;
    }

    private void AloitusNayttö()
    {
        Pause();
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Avaruus Peli!", "Aloita peli", "Parhaat pisteet", "Lopeta");
        Add(alkuvalikko);
        
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        alkuvalikko.AddItemHandler(1, ParhaatPisteet);
        alkuvalikko.AddItemHandler(2, Exit);
        alkuvalikko.DefaultCancel = 0;
        
        alkuvalikko.Color = Color.DarkGray;
        alkuvalikko.SetButtonColor(Color.MidnightBlue);
    }
    
    private void AloitaPeli()
    {
        Pause();
    }

    private void ParhaatPisteet()
    {
        _topLista.Show();
        _topLista.HighScoreWindow.Closed += AloitaPeli;
    }

    /*private void KuolemaRuutu()
    {
        MultiSelectWindow kuolemaMenu = new MultiSelectWindow("Kuolit", "Aloita alusta", "Parhaat pisteet", "Lopeta");
        Add(kuolemaMenu);
        
        kuolemaMenu.AddItemHandler(0, AloitaAlusta);
        kuolemaMenu.AddItemHandler(1, ParhaatPisteet);
        kuolemaMenu.AddItemHandler(2, Exit);
    }*/
    
    private void Paussilla()
    {
        Pause();
        MultiSelectWindow paussiMenu = new MultiSelectWindow("Paussilla", "Jatka", "Aloita Alusta", "Lopeta");
        paussiMenu.AddItemHandler(0, Jatka);
        paussiMenu.AddItemHandler(1, AloitaAlusta);
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

    private void AloitaAlusta()
    {
        ClearAll();
        Begin();
    }
    private void LisaaPelaajanPisteet()
    {
        _alus.Destroy();
        _topLista.EnterAndShow(_pistelaskuri.Value);
        _topLista.HighScoreWindow.Closed += AloitaPeli;
    }
    
    private void AloitaPeli(Window sender)
    {
        AloitusNayttö();
        ClearAll();
        Begin();
    }
    
    void LuoAikalaskuri()
    {
        Timer aikalaskuri = new Timer();
        aikalaskuri.Start();
        
        Label aikanaytto = new Label();
        aikanaytto.TextColor = Color.White;
        aikanaytto.DecimalPlaces = 0;
        aikanaytto.BindTo(aikalaskuri.SecondCounter);
        Add(aikanaytto);
        
        aikanaytto.X = Screen.Left + 120;
        aikanaytto.Y = Screen.Top - 75;
        
        aikanaytto.Title = "Aika: ";
    }
    
    void LuoPistelaskuri()
    {
        _pistelaskuri = new IntMeter(0);               
        
        Label pistenaytto = new Label(); 
        pistenaytto.X = Screen.Right - 120;
        pistenaytto.Y = Screen.Top - 75;
        pistenaytto.TextColor = Color.White;
        
        pistenaytto.BindTo(_pistelaskuri);
        Add(pistenaytto);
        
        pistenaytto.Title = "Pisteet: ";
        _pistelaskuri.Changed += delegate(int value, int newValue)
        {
            if (newValue == _intervaliaNostettu + 35)
            {
                _intervaliaNostettu = newValue;
                if (_intervaliaNostettu > 1)
                {
                    _vihuSpawni.Interval -= 1;
                }
            }
        };
    }
    
    void AmmuAseella(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            ammus.AddCollisionIgnoreGroup(11);
            ammus.Size *= 2;
            ammus.Image = _ammus;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
        }
    }
    
    void VihollinenAmpuu(AssaultRifle ase)
    {
        PhysicsObject ammus = ase.Shoot();

        if (ammus != null)
        {
            ammus.AddCollisionIgnoreGroup(11);
            ammus.Size *= 2;
            ammus.Image = _ammus;
            ammus.MaximumLifetime = TimeSpan.FromSeconds(2.0);
        }
    }

    private void AmmusOsui(PhysicsObject ammus, PhysicsObject kohde)
    {
        ammus.Destroy();
        
        int pMaxMaara = 200;
        ExplosionSystem rajahdys = new ExplosionSystem(_liekki, pMaxMaara);
        
        rajahdys.MinVelocity = 5; // Muodostuvien hiukkasten miniminopeus
        rajahdys.MinLifetime = 0.2; // Muodostuvien hiukkasten minimielinaika
        
        rajahdys.MaxVelocity = 20; // Muodostuvien hiukkasten maksiminopeus
        rajahdys.MaxLifetime = 1; // Muodostuvien hiukkasten maksimielinaika
        Add(rajahdys);

        int pMaara = 5;
        
        if (kohde.Tag.ToString() == "vihu")
        {
            (kohde as Vihollinen).Elamalaskuri.AddValue(-1);
            _pistelaskuri.Value += 2;
            rajahdys.AddEffect(ammus.Position, pMaara);
        }
        else if (kohde.Tag.ToString() == "asteroiditag")
        {
            (kohde as AsteroidiTuho).Elamalaskuri.AddValue(-1);
            _pistelaskuri.Value += 1;
            rajahdys.AddEffect(ammus.Position, pMaara);
        }
        else if (kohde.Tag.ToString() == "hyvisKuolee")
        {
            (kohde as AlusKuolee).Elamalaskuri.AddValue(-1);
            rajahdys.AddEffect(ammus.Position, pMaara);
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

class AlusKuolee : PhysicsObject
{
    private IntMeter _elamalaskuri = new IntMeter(5, 0, 5);
    public  IntMeter Elamalaskuri { get { return _elamalaskuri; } }

    public AlusKuolee(double leveys, double korkeus)
        : base(leveys, korkeus)
    {
        _elamalaskuri.LowerLimit += delegate { this.Destroy(); };
    }
}

class AsteroidiTuho : PhysicsObject
{
    private IntMeter _elamalaskuri = new IntMeter(15, 0, 15);
    public  IntMeter Elamalaskuri { get { return _elamalaskuri; } }

    public AsteroidiTuho(double leveys, double korkeus)
        : base(leveys, korkeus)
    {
        _elamalaskuri.LowerLimit += delegate { this.Destroy(); };
    }
}
