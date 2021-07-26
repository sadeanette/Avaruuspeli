using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Assets;


/// @author Säde Latikka
/// @version 3.12.2019
/// <summary>
/// Ohjelmointi 1 harjoitustyö: Peli
/// </summary>
public class Avaruuspeli : PhysicsGame
{
    private const double r = 100;

    private IntMeter pisteLaskuri; // En voi viedä parametrina CollisionHandler (törmäys, keräys) metodeissa
    private IntMeter elamaLaskuri;

    private readonly List<PhysicsObject> vaikutusalueet = new List<PhysicsObject>(); // En voi viedä parametrina Update funktioon
    private PhysicsObject[] objektit = new PhysicsObject[3];


    /// <summary>
    /// Peli alkaa tästä
    /// </summary>
    public override void Begin()
    {
        AlkuValikko();

        MediaPlayer.Play("slumberjack");
        MediaPlayer.IsRepeating = true;
    }


    /// <summary>
    ///Luodaan peliin alkuvalikko ja tausta
    /// </summary>
    private void AlkuValikko()
    {
        ClearAll();

        Level.Background.CreateStars(1000);
        Mouse.IsCursorVisible = true;

        Image alkuKuva = LoadImage("alkunaytto");

        PhysicsObject alku = new PhysicsObject(r * 11, r * 6, Shape.Rectangle);
        alku.Image = alkuKuva;
        alku.Y = 150;
        Add(alku);

        LuoUfo(600, -300, new Vector(-300, 500));

        Valikko("", "Aloita uusi peli", "Asetukset", "Credits", "Lopeta", 0, -50);
    }


    private void AsetusValikko(string otsikko, string näyttö, string musiikki, string paluu, Boolean pelissä)
    {

        IsPaused = true;

        MultiSelectWindow valikko;

        valikko = new MultiSelectWindow(otsikko, näyttö, musiikki, paluu);
        valikko.X = 0;
        valikko.Y = 0;
        valikko.AddItemHandler(0, delegate { NäytönVaihto(pelissä); });
        valikko.AddItemHandler(1, delegate { MusiikkiAsetus(pelissä); });

        if (paluu == "Palaa takaisin alkuvalikkoon") valikko.AddItemHandler(2, AlkuValikko);
        else valikko.AddItemHandler(2, delegate { Valikko("Paused", "Jatka peliä", "Siirry takaisin alkuvalikkoon", "Asetukset", "Lopeta"); });

        Add(valikko);
    }


    private void NäytönVaihto(Boolean pelissä)
    {
        LoadSoundEffect("valikkoaani").Play();
        if (IsFullScreen == false) IsFullScreen = true;
        else IsFullScreen = false;

        if (pelissä == true) AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin peliin", pelissä);
        else AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin alkuvalikkoon", pelissä);
    }


    private void MusiikkiAsetus(Boolean pelissä)
    {
        LoadSoundEffect("valikkoaani").Play();
        if (MediaPlayer.IsPlaying == true)
        {
            MediaPlayer.Pause();
        }
        else MediaPlayer.Resume();

        if (pelissä == true) AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin peliin", pelissä);
        else AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin alkuvalikkoon", pelissä);
    }


    /// <summary>
    /// Aliohjelma näyttää viittaukset
    /// </summary>
    private void ViittausTeksti()
    {
        string[] viittaukset = new string[] { "Grafiikat ja äänitehosteet:", "Tekijä: Säde", "Toteutus: Pixilart ja Sfxr", "2019", " ", "Musiikki:", "Nimi: Slumberjack", "Tekijä: raina", "Toteutus: Ft2 ja MilkyTracker", "2005", "Lisenssi: Mod Archive Distribution" };

        VerticalLayout asettelu = new VerticalLayout();
        asettelu.Spacing = 10;

        Widget laatikko = new Widget(asettelu);
        laatikko.Color = Color.Transparent;
        laatikko.Y = -50;
        Add(laatikko);

        for (int i = 0; i < viittaukset.Length; i++)
        {
            Label teksti = new Label(viittaukset[i]);
            teksti.TextColor = Color.White;
            laatikko.Add(teksti);
        }

        Valikko("", "Palaa takaisin", "", "", "", 0, -350);
    }


    /// <summary>
    /// Aliohjelma luo valikon ja sen painikkeet
    /// </summary>
    /// <param name="otsikko">Valikossa näytettävä otsikko</param>
    /// <param name="valintateksti">Ensimmäinen valintakohta</param>
    /// <param name="valintateksti2">Toinen valintakohta</param>
    /// <param name="valintateksti3">Kolmas valintakohta</param>
    /// <param name="x">Valikon sijainti y-akselilla</param>
    /// <param name="y">Valikon sijainti y-akselilla</param>
    private void Valikko(string otsikko, string valintateksti, string valintateksti2, string valintateksti3, string valintateksti4, int x = 0, int y = 0)
    {
        LoadSoundEffect("valikkoaani").Play();

        IsPaused = true;

        MultiSelectWindow valikko;

        if (valintateksti2 == "")
        {
            valikko = new MultiSelectWindow(otsikko,
            valintateksti);
            valikko.X = x;
            valikko.Y = y;
            valikko.AddItemHandler(0, AlkuValikko);   
            Add(valikko);
        }
        else
        {
            valikko = new MultiSelectWindow(otsikko,
            valintateksti, valintateksti2, valintateksti3, valintateksti4);
            valikko.X = x;
            valikko.Y = y;
            Add(valikko);
            valikko.AddItemHandler(3, Poistu);

            if (valintateksti3 == "Asetukset") valikko.AddItemHandler(2, delegate { LoadSoundEffect("valikkoaani").Play(); AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin peliin", true); } );

            if (valintateksti2 == "Asetukset") valikko.AddItemHandler(1, delegate { LoadSoundEffect("valikkoaani").Play(); AsetusValikko("", "Koko näyttö On/Off", "Musiikki On/Off", "Palaa takaisin alkuvalikkoon", false); });
            else valikko.AddItemHandler(1, AlkuValikko);

            if (valintateksti3 == "Credits") valikko.AddItemHandler(2, ViittausTeksti);

            if (valintateksti == "Aloita uusi peli") valikko.AddItemHandler(0, AloitaPeli);
            else valikko.AddItemHandler(0, JatkaPeliä);
        }
    }


    private void Poistu()
    {
        LoadSoundEffect("valikkoaani").Play();
        Exit();
    }


    /// <summary>
    /// Aliohjelma luo widgetin, joka toimii musiikin säätäjänä
    /// </summary>
    /// <param name="kuva">Widgetin kuva</param>
    /// <param name="vali">Widgetin välimatka vakiosijainnista</param>
    /// <returns>Widget-olion</returns>

    private Widget MusiikinPainike(Image kuva, int vali = 0)
    {
        Widget painike = new Widget(r * 0.4, r * 0.4);
        painike.X = Screen.Left + 50 + vali;
        painike.Y = Screen.Bottom + 50;
        painike.Image = kuva;
        Add(painike);
        return painike;
    }


    /// <summary>
    /// Aliohjelma suoritetaan, kun peliä halutaan jatkaa
    /// </summary>
    private void JatkaPeliä()
    {
        LoadSoundEffect("valikkoaani").Play();
        IsPaused = false;
    }


    /// <summary>
    /// Aliohjelma suoritetaan, kun halutaan aloittaa uusi peli
    /// </summary>
    private void AloitaPeli()
    {
        LoadSoundEffect("valikkoaani").Play();
        ClearAll();
        LuoKentta();
    }


    /// <summary>
    /// Aliohjelma luo pelikentän ja ohjaimet
    /// </summary>
    private void LuoKentta()
    {
        ClearAll();

        IsPaused = false;

        Vector kentanKoko = new Vector (1000, 900);

        IsFullScreen = true;
        Level.Size = kentanKoko * 3; Level.CreateBorders(false);
        Level.Background.CreateStars(1000);
        Mouse.IsCursorVisible = true;
        Camera.Zoom(0.8);

        List<PhysicsObject> planeetat = new List<PhysicsObject>();
        List<PhysicsObject> alienit = new List<PhysicsObject>();

        Image[] kuvat = LoadImages("astronautti", "astronauttiVasen", "planeetta1", "planeetta2", "kuu", "alien");

        LuoLaskuri(elamaLaskuri = new IntMeter(5), -150, -40, "Elämät: {0}/5");
        LuoLaskuri(pisteLaskuri = new IntMeter(0), -1400, -40, "Tähtiä kerätty: {0}/36");

        PhysicsObject pelaaja = LuoPelaaja();
        PhysicsObject paino = LuoPainopiste(pelaaja.X, pelaaja.Y, 1, objektit, 0);
        PhysicsObject hyppySuunta = LuoHyppySuunta(pelaaja.X, pelaaja.Y, 1);
        AxleJoint liitos = new AxleJoint(pelaaja, paino, pelaaja.Position + new Vector(0, -40));
        AxleJoint liitos2 = new AxleJoint(pelaaja, hyppySuunta, pelaaja.Position + new Vector(0, 100));
        Add(liitos); Add(liitos2);

        List<Vector> polku1 = new List<Vector>() { new Vector(650, 350), new Vector(450, 150), new Vector(250, 350), new Vector(450, 550) };
        List<Vector> polku2 = new List<Vector>() { new Vector(-450, 250), new Vector(-650, 50), new Vector(-850, 250), new Vector(-650, 450) };

        LuoAlien(450, 590, alienit, kuvat[5], objektit, polku1, 1);
        LuoAlien(-650, 480, alienit, kuvat[5], objektit, polku2, 2);

        LuoPlaneettaObjekti(0, 3, 0, 4, kuvat[2], 400, planeetat, vaikutusalueet);
        LuoPlaneettaObjekti(4, 5, 0, 4, kuvat[3], 400, planeetat, vaikutusalueet);
        LuoPlaneettaObjekti(2, 4, 300, 2, kuvat[4], 200, planeetat, vaikutusalueet);
        LuoKerattavat(planeetat);

        Timer.CreateAndStart(5, LisaaMeteoriitteja);
        Timer.CreateAndStart(20, LisaaUfoja);

        Keyboard.Listen(Key.A, ButtonState.Down, delegate () { Liikuta(paino, pelaaja, "vasen"); }, "Liiku vasemmalle");
        Keyboard.Listen(Key.D, ButtonState.Down, delegate () { Liikuta(paino, pelaaja, "oikea"); }, "Liiku oikealle");
        Keyboard.Listen(Key.Space, ButtonState.Pressed, delegate () { Timer.SingleShot(0.001, delegate { Hyppää(hyppySuunta, pelaaja); }); }, "Hyppää");


        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, delegate () { pelaaja.MoveTo(Mouse.PositionOnWorld, 800); }, "Jetpack");

        Keyboard.Listen(Key.A, ButtonState.Down, delegate () { pelaaja.Image = kuvat[1]; }, null);
        Keyboard.Listen(Key.D, ButtonState.Down, delegate () { pelaaja.Image = kuvat[0]; }, null);

        Keyboard.Listen(Key.Escape, ButtonState.Pressed, delegate () { Valikko("Paused", "Jatka peliä", "Siirry takaisin alkuvalikkoon", "Asetukset", "Lopeta"); }, null);
    }

    private void Hyppää(PhysicsObject suunta, PhysicsObject pelaaja)
    {
        pelaaja.MoveTo(suunta.Position, 800);
    }

    /// <summary>
    /// Luodaan pelaaja
    /// </summary>
    /// <returns>Pelaajan</returns>
    private PhysicsObject LuoPelaaja()
    {
        PhysicsObject pelaaja = new PhysicsObject(r, r, Shape.Circle);
        pelaaja.Image = LoadImage("astronautti");
        pelaaja.CollisionIgnoreGroup = 1;
        pelaaja.Restitution = 0;
        pelaaja.Bottom = -10;
        pelaaja.MomentOfInertia = 50;
        Add(pelaaja);
        Camera.Follow(pelaaja);
        pelaaja.Animation.Start();
        AddCollisionHandler(pelaaja, "tahti", TahdenKerays);
        AddCollisionHandler(pelaaja, "happipullo", LisaaHappea);
        AddCollisionHandler(pelaaja, "meteoriitti", Tormays);
        AddCollisionHandler(pelaaja, "alienit", Tormays);
        return pelaaja;
    }


    /// <summary>
    /// Luodaan painopiste
    /// </summary>
    /// <param name="x">Painopisteen sijainti x-akselilla</param>
    /// <param name="y">Painopisteen sijainti y-akselilla</param>
    /// <param name="ryhma">Määrittää sen ryhmän, jonka olioiden kanssa painopiste ei huomioi törmäämistä</param>
    /// <param name="objektit">Taulukko, johon painopiste lisätään</param>
    /// <param name="i">Taulukon indeksiluku</param>
    /// <returns>Painopisteen</returns>
    private PhysicsObject LuoPainopiste(double x, double y, int ryhma, PhysicsObject[] objektit, int i)
    {
        PhysicsObject paino = new PhysicsObject(r * 0.1, r * 0.1, Shape.Circle);
        paino.X = x;
        paino.Y = y - 35;
        paino.Color = Color.Transparent;
        paino.CollisionIgnoreGroup = ryhma;
        Add(paino, 1);
        objektit[i] = paino;
        return paino;
    }

    private PhysicsObject LuoHyppySuunta(double x, double y, int ryhma)
    {
        PhysicsObject pallo = new PhysicsObject(r * 0.1, r * 0.1, Shape.Circle);
        pallo.X = x;
        pallo.Y = y + 100;
        pallo.Color = Color.Transparent;
        pallo.IgnoresCollisionResponse = true;
        Add(pallo, 1);
        return pallo;
    }

    /// <summary>
    /// Luodaan alien-vihollinen
    /// </summary>
    /// <param name="x">Alienin sijainti x-akselilla</param>
    /// <param name="y">Alienin sijainti y-akselilla</param>
    /// <param name="alienit">Lista, johon alien lisätään</param>
    /// <param name="kuva">Alienin kuva</param>
    /// <param name="objektit">Taulukko, jossa painopisteet sijaitsevat</param>
    /// <param name="polku">Polku, jonka mukaan alien liikkuu</param>
    /// <param name="indeksi">Taulukon indeksin paikka</param>
    /// <returns>Alienin</returns>
    private PhysicsObject LuoAlien(double x, double y, List<PhysicsObject> alienit, Image kuva, PhysicsObject[] objektit, List<Vector> polku, int indeksi)
    {
        PhysicsObject alien = new PhysicsObject(r, r, Shape.Circle);
        alien.Image = kuva;
        alien.X = x;
        alien.CollisionIgnoreGroup = 2;
        alien.Restitution = 0;
        alien.Bottom = -10;
        alien.MomentOfInertia = 100;
        alien.Tag = "alienit";
        Add(alien, 1);
        alienit.Add(alien);

        alien.Y = y;

        AxleJoint liitos = new AxleJoint(alien, LuoPainopiste(alien.X, alien.Y, 2, objektit, indeksi), alien.Position + new Vector(0, -40));
        Add(liitos);

        LiikutaAlienia(objektit[indeksi], polku);

        return alien;
    }


    /// <summary>
    /// Aliohjelma luo alienille polkuaivon, jonka mukaan alien liikkuu
    /// </summary>
    /// <param name="painoPiste">painoPiste, joka liikuttaa alienia</param>
    /// <param name="polku">Polku, jonka mukaan alien liikkuu</param>
    private void LiikutaAlienia(PhysicsObject painoPiste, List<Vector> polku)
    {
        PathFollowerBrain polkuAivot = new PathFollowerBrain(polku);
        polkuAivot.Loop = true;
        polkuAivot.TurnWhileMoving = true;

        painoPiste.Brain = polkuAivot;
    }


    /// <summary>
    /// Luodaan planeetat, kuut, sekä määritetään tähtien sijainnit
    /// </summary>
    /// <param name="x">Sijainnin määritys</param>
    /// <param name="lopetus">Määrittää kuinka monta objektia luodaan</param>
    /// <param name="vali">Määrittää objektin etäisyyden annetusta sijainnista</param>
    /// <param name="kerroin">Määrittää objektin koon kertomalla vakiota r</param>
    /// <param name="kuva">Olion kuva</param>
    /// <param name="koko">Määrittää objektin ympärillä olevan vaikutusalueen koon</param>
    /// <param name="lista">Lista, johon objekti lisätään</param>
    /// <param name="vaikutusalueet">Lista, johon planeettojen vaikutusalueet lisätään</param>
    private void LuoPlaneettaObjekti(int x, int lopetus, int vali, int kerroin, Image kuva, int koko, List<PhysicsObject> lista, List<PhysicsObject> vaikutusalueet)
    {
        double[,] sijainnit = { { 150, -250 }, { 450, 350 }, { 1150, -450 }, { -450, -650 }, { -650, 250 }, { 950, 650 } };

        while (x <= lopetus)
        {
            int y = 0;

            PhysicsObject planeetta = PhysicsObject.CreateStaticObject(r * kerroin, r * kerroin, Shape.Circle);
            planeetta.X = sijainnit[x, y] - vali;
            y++;
            planeetta.Y = sijainnit[x, y] + vali;
            planeetta.Image = kuva;
            planeetta.Tag = "kohde";
            lista.Add(planeetta);
            Add(planeetta);
            LuoVaikutusalue(koko, planeetta.X, planeetta.Y, vaikutusalueet);

            x++;
        }
    }


    /// <summary>
    /// Luodaan painovoiman vaikutusalue planeetan ympärille
    /// </summary>
    /// <param name="koko">Vaikutusalueen koko</param>
    /// <param name="x">Vaikutusalueen sijainti x-akselilla</param>
    /// <param name="y">Vaikutusalueen sijainti y-akselilla</param>
    /// <param name="vaikutusalueet">Lista, johon planeettojen vaikutusalueet lisätään</param>
    /// <returns>Vaikutusalueen</returns>
    private PhysicsObject LuoVaikutusalue(int koko, double x, double y, List<PhysicsObject> vaikutusalueet)
    {
        PhysicsObject vaikutusalue = PhysicsObject.CreateStaticObject(koko * 2, koko * 2, Shape.Circle);
        vaikutusalue.X = x;
        vaikutusalue.Y = y;
        vaikutusalue.IgnoresCollisionResponse = true;
        vaikutusalue.Color = Color.Transparent;
        vaikutusalueet.Add(vaikutusalue);
        Add(vaikutusalue);
        return vaikutusalue;
    }


    /// <summary>
    /// Luodaan taustalla lentävä ufo
    /// </summary>
    /// <param name="x">Vaikutusalueen sijainti x-akselilla</param>
    /// <param name="y">Vaikutusalueen sijainti y-akselilla</param>
    /// <param name="suunta">Vektori, jonka mukaisesti ufo liikkuu taustalla</param>
    /// <returns>Ufon</returns>
    private PhysicsObject LuoUfo(double x, double y, Vector suunta)
    {
        PhysicsObject ufo = new PhysicsObject(r, r, Shape.Circle);
        ufo.X = x;
        ufo.Y = y;
        ufo.Image = LoadImage("ufo");
        ufo.IgnoresCollisionResponse = true;
        ufo.LifetimeLeft = TimeSpan.FromSeconds(10.0);
        Add(ufo, -1);
        ufo.Hit(suunta);
        return ufo;
    }


    /// <summary>
    /// Määritetään kerättävien objektien sijainnit ja luodaan ne
    /// </summary>
    /// <param name="planeetat">Lista planeetoista, joiden ympärille objekteja lisätään</param>
    private void LuoKerattavat(List<PhysicsObject> planeetat)
    {
        Image tahtiKuva = LoadImage("tahti");
        Image happipulloKuva = LoadImage("happipullo");

        /// <summary>
        /// Luodaan kerättävä objekti
        /// </summary>
        /// <param name="x">Objektin sijainti x-akselilla</param>
        /// <param name="y">Objektin sijainti y-akselilla</param>
        /// <param name="koko">Objektin koon kerroin</param>
        /// <param name="kuva">Objektin kuva</param>
        /// <param name="tagi">Objektin tag-ominaisuus</param>
        /// <returns>Objektin</returns>
        PhysicsObject LuoKerattava(double x, double y, double koko, Image kuva, string tagi)
        {
            PhysicsObject objekti = new PhysicsObject(r * koko, r * koko, Shape.Star);
            objekti.X = x;
            objekti.Y = y;
            objekti.Image = kuva;
            objekti.Tag = tagi;
            objekti.CollisionIgnoreGroup = 2;
            Add(objekti);
            return objekti;
        }

        // Tähtien luonti jokaisen planeetan ympärille
        foreach (PhysicsObject planeetta in planeetat)
        {
            LuoKerattava(planeetta.X, planeetta.Y + planeetta.Width /1.5, 0.5, tahtiKuva, "tahti");
            LuoKerattava(planeetta.X, planeetta.Y - planeetta.Width / 1.5, 0.5, tahtiKuva, "tahti");
            LuoKerattava(planeetta.X + planeetta.Width / 1.5, planeetta.Y, 0.5, tahtiKuva, "tahti");
            LuoKerattava(planeetta.X - planeetta.Width / 1.5, planeetta.Y, 0.5, tahtiKuva, "tahti");
        }

        // Happipullojen luonti määriteltyihin sijanteihin
        LuoKerattava(600, 600, 0.5, happipulloKuva, "happipullo");
        LuoKerattava(-500, -400, 0.5, happipulloKuva, "happipullo");
    }


    /// <summary>
    /// Luodaan meteoriitti
    /// </summary>
    /// <param name="x">Meteoriitin sijainti x-akselilla</param>
    /// <param name="y">Meteoriitin sijainti y-akselilla</param>
    /// <param name="suunta">Vektori, jonka mukaisesti meteoriitti liikkuu</param>
    /// <returns>Meteoriitin</returns>
    private PhysicsObject LuoMeteoriitti(double x, double y, Vector suunta)
    {
        PhysicsObject meteoriitti = new PhysicsObject(r * 1.5, r * 1.5, Shape.Star);
        meteoriitti.X = x;
        meteoriitti.Y = y;
        meteoriitti.Image = LoadImage("meteoriitti");
        meteoriitti.Tag = "meteoriitti";
        meteoriitti.CollisionIgnoreGroup = 2;
        meteoriitti.LifetimeLeft = TimeSpan.FromSeconds(15.0);
        Add(meteoriitti);
        meteoriitti.Hit(suunta);
        AddCollisionHandler(meteoriitti, "kohde", MeteoriittiOsui);
        return meteoriitti;
    }


    /// <summary>
    /// Asetetaan luotaville meteoriiteille satunnainen sijainti ja lentokulma
    /// </summary>
    private void LisaaMeteoriitteja()
    {
        int numero = RandomGen.NextInt(0, 3);

        switch(numero)
        {
            case 0:
                LuoMeteoriitti(RandomGen.NextDouble(-Screen.Width, Screen.Width), Screen.Height, Vector.FromLengthAndAngle(RandomGen.NextDouble(200, 400), RandomGen.NextAngle(Angle.FromDegrees(-70), Angle.FromDegrees(-120))));
                break;
            case 1:
                LuoMeteoriitti(RandomGen.NextDouble(-Screen.Width, Screen.Width), -Screen.Height, Vector.FromLengthAndAngle(RandomGen.NextDouble(200, 400), RandomGen.NextAngle(Angle.FromDegrees(70), Angle.FromDegrees(120))));
                break;
            case 2:
                LuoMeteoriitti(-Screen.Width, RandomGen.NextDouble(-Screen.Height, Screen.Height), Vector.FromLengthAndAngle(RandomGen.NextDouble(200, 400), RandomGen.NextAngle(Angle.FromDegrees(-30), Angle.FromDegrees(30))));
                break;
            case 3:
                LuoMeteoriitti(Screen.Width, RandomGen.NextDouble(-Screen.Height, Screen.Height), Vector.FromLengthAndAngle(RandomGen.NextDouble(200, 400), RandomGen.NextAngle(Angle.FromDegrees(150), Angle.FromDegrees(210))));
                break;
        }
    }


    /// <summary>
    /// Asetetaan luotavalle ufolle satunnainen sijainti ja lennon nopeus
    /// </summary>
    private void LisaaUfoja()
    {
        int numero = RandomGen.NextInt(0, 1);

        switch (numero)
        {
            case 0:
                LuoUfo(-Screen.Width, RandomGen.NextDouble(-Screen.Height, Screen.Height), new Vector(RandomGen.NextDouble(50, 200), 0));
                break;
            case 1:
                LuoUfo(Screen.Width, RandomGen.NextDouble(-Screen.Height, Screen.Height), new Vector(RandomGen.NextDouble(50, -200), 0));
                break;
        }
    }


    /// <summary>
    /// Liikutetaan pelaajaa
    /// </summary>
    /// <param name="olio">Liikutettava olio</param>
    /// <param name="pelaaja">Objekti, jonka osoittamaan suuntaan liikutetaan</param>
    /// <param name="vasenVaiOikea">Määrittää, liikutetaanko pelaajaa oikealle vai vasemmalle</param>
    private void Liikuta(PhysicsObject olio, PhysicsObject pelaaja, string vasenVaiOikea)
    {
        Vector pelaajanSuunta = Vector.FromLengthAndAngle(300.0, pelaaja.Angle);

        if (vasenVaiOikea == "vasen") olio.Hit(-pelaajanSuunta);
        else olio.Hit(pelaajanSuunta);
    }


    /// <summary>
    /// Aliohjelma luo pelaajalle vetovoiman lähintä planeettaa kohti
    /// </summary>
    protected override void Update(Time time)
    {
        PhysicsObject painovoimaAlue = null;

        foreach (PhysicsObject fysiikkaObjekti in objektit)
        {
            painovoimaAlue = EtsiLahinPlaneetta(vaikutusalueet, fysiikkaObjekti);
            fysiikkaObjekti.StopMoveTo();
            if (painovoimaAlue.IsInside(fysiikkaObjekti.Position))
            {
                fysiikkaObjekti.MoveTo(new Vector(painovoimaAlue.X, painovoimaAlue.Y), 300);
            }
        }
        base.Update(time);
    }


    /// <summary>
    /// Aliohjelma etsii pelaajaa lähimmän planeetan
    /// </summary>
    /// <param name="vaikutusalueet">Alueet, joista lähintä etsitään</param>
    /// <param name="objektit">Objektit, joihin painovoima tulee vaikuttamaan</param>
    /// <returns>Lähimmän vaikutusalueen</returns>
    private static PhysicsObject EtsiLahinPlaneetta(List<PhysicsObject> vaikutusalueet, PhysicsObject objekti)
    {
        double lyhinValimatka = double.MaxValue;
        if (vaikutusalueet.Count == 0) return null;
        PhysicsObject lahinVaikutusalue = vaikutusalueet[0];

        foreach (PhysicsObject a in vaikutusalueet)
        {
            double valimatka = Vector.Distance(objekti.Position, a.Position);

            if (valimatka < lyhinValimatka)
            {
                lyhinValimatka = valimatka;
                lahinVaikutusalue = a;
            }
        }
        return lahinVaikutusalue;
    }


    /// <summary>
    /// Luodaan pistenäyttö piste- ja elämälaskurille
    /// </summary>
    /// <param name="laskuri">Laskuri, jolle pistenäyttö luodaan</param>
    /// <param name="x">Laskurin sijainti x-akselilla</param>
    /// <param name="y">Laskurin sijainti y-akselilla</param>
    /// <param name="teksti">Pistenäytössä näytettävä teksti</param>
    private void LuoLaskuri(IntMeter laskuri, int x, int y, string teksti)
    {
        Label pisteNaytto = new Label();
        pisteNaytto.X = Screen.Right + x;
        pisteNaytto.Y = Screen.Top + y;
        pisteNaytto.TextColor = Color.White;
        pisteNaytto.Color = Color.Black;
        pisteNaytto.IntFormatString = teksti;

        pisteNaytto.BindTo(laskuri);
        Add(pisteNaytto);
    }


    /// <summary>
    /// Tapahtuma tähtien keräämiselle
    /// </summary>
    /// <param name="pelaaja">Olio, joka kerää objektin</param>
    /// <param name="tahti">Objekti, jonka olio kerää</param>
    private void TahdenKerays(PhysicsObject pelaaja, PhysicsObject tahti)
    {
        SoundEffect bling = LoadSoundEffect("tahdenkerays");
        bling.Play();

        pisteLaskuri.Value++;
        tahti.Destroy();

        if (pisteLaskuri.Value == 36) Valikko("Voitit pelin!", "Aloita uusi peli", "Siirry takaisin alkuvalikkoon", "Asetukset", "Lopeta");
    }


    /// <summary>
    /// Tapahtuma happipullon keräämiselle
    /// </summary>
    /// <param name="pelaaja">Olio, joka kerää objektin</param>
    /// <param name="happipullo">Objekti, jonka olio kerää</param>
    private void LisaaHappea(PhysicsObject pelaaja, PhysicsObject happipullo)
    {
        SoundEffect happea = LoadSoundEffect("happipullonkerays");
        happea.Play();

        elamaLaskuri.Value = 5;

        happipullo.Destroy();
    }


    /// <summary>
    /// Tapahtuma, kun meteoriitti tai vihollinen osuu pelaajaan
    /// </summary>
    /// <param name="pelaaja">Olio, johon objekti törmää</param>
    /// <param name="tormaaja">Objekti, joka törmää olioon</param>
    private void Tormays(PhysicsObject pelaaja, PhysicsObject tormaaja)
    {
        SoundEffect auts = LoadSoundEffect("auts");
        auts.Play();

        if (tormaaja.Tag.ToString() == "meteoriitti")
        {
            Rajahdys(tormaaja);
            elamaLaskuri.Value -= 2;
        }
        else elamaLaskuri.Value--;

        if (elamaLaskuri.Value == 0)
        {
            pelaaja.Destroy();
            Valikko("Hävisit pelin", "Aloita uusi peli", "Siirry takaisin alkuvalikkoon", "Asetukset", "Lopeta");
        }
    }


    /// <summary>
    /// Tapahtuma, kun meteoriitti osuu planeettaan tai kuuhun
    /// </summary>
    /// <param name="meteoriitti">Meteoriitti, joka osuu kohteeseen</param>
    /// <param name="kohde">Kohde, johon meteoriitti osuu</param>
    private void MeteoriittiOsui(PhysicsObject meteoriitti, PhysicsObject kohde)
    {
        Rajahdys(meteoriitti);
    }


    /// <summary>
    /// Räjähdysefekti meteoriitin törmätessä
    /// </summary>
    /// <param name="meteoriitti">Objekti, joka räjähtää</param>
    private void Rajahdys(PhysicsObject meteoriitti)
    {
	   Explosion rajahdys = new Explosion(meteoriitti.Width * 2);
	   rajahdys.Position = meteoriitti.Position;
	   rajahdys.UseShockWave = false;
       Add(rajahdys);
	   Remove(meteoriitti);
    }
}
