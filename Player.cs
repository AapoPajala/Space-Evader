using Godot;
using System;
using System.Text;

public class Player : RigidBody2D
{
    public ProgressBar progressBar;
    Button restart;
    public Label timeLabel, ammoCounter, hs;
    AudioStreamPlayer2D loop;
    Control getUI() {
        return GetParent().GetNode("UI").GetChild(0) as Control;
    }
    public static bool started;
    AudioPlayer audio;
    public override void _Ready()
    {
        audio = GetParent().GetNode("AudioPlayer") as AudioPlayer;
        ContactMonitor = true;
        ContactsReported = 4;
        progressBar = getUI().GetChild(0) as ProgressBar;
        restart = getUI().GetChild(1) as Button;
        timeLabel = getUI().GetChild(2) as Label;
        ammoCounter = getUI().GetChild(3) as Label;
        hs = getUI().GetChild(4) as Label;
        progressBar.PercentVisible = false;
        progressBar.Modulate = Colors.Green;
        Connect("body_entered", this, nameof(onHit));
        restart.Connect("pressed", this, nameof(restartGame));
        if(!started) {
            reset();
            started = true;
        }
        loop = new AudioStreamPlayer2D();
        loop.Stream = GD.Load<AudioStream>("res://sounds/engine.wav");
        loop.Connect("finished", this, nameof(play));
        AddChild(loop);

        rocket = GD.Load<PackedScene>("res://Objects/Rocket.tscn");

        ammo = 3;
        ammoCounter.Text = "" + ammo;

        
        file = new File();

        if(file.FileExists("user://game.dat")) {
            file.OpenEncryptedWithPass("user://game.dat", File.ModeFlags.Read, "101TheGamesBeHacky101");
            highscore = file.GetFloat();
            file.Close();
            hs.Text = "" + highscore;
            hs.Visible = true;
        }
        else hs.Visible = false;

        
    }
    private byte[] k;
    private float highscore;
    float msp;
    public bool tap, start;
    Vector2 last;
    PackedScene rocket;
    File file;
    void play() {
        loop.Play(0);
    }

    public int ammo;
    public float dtime;

    public override void _Process(float delta)
    {
        Vector2 mouse = GetViewport().GetMousePosition() - GetViewport().Size / 2;
        
        if(!dead && !start) {
            audio.play(audio.start);
            loop.Play(0);
            start = true;
        }

        if(dtime > 0) dtime -= delta;

        if(!tap && mb == 1) {
            last = mouse;
            tap = true;

            if(dtime > 0 && ammo > 0 && !dead) {
                Rocket shot = rocket.Instance() as Rocket;
                GetParent().AddChild(shot);
                shot.GlobalPosition = GlobalPosition - new Vector2(-(float)Math.Sin(Rotation) * 100f, (float)Math.Cos(Rotation) * 100f);
                shot.AddCollisionExceptionWith(this);
                shot.LinearVelocity = new Vector2(-(float)Math.Sin(Rotation) * -1500f, (float)Math.Cos(Rotation) * -1500f);
                shot.Rotation = Rotation;
                audio.play(audio.rocket);
                ammo--;
                ammoCounter.Text = "" + ammo;
            }

            if(dtime <= 0)
                dtime = 0.2f;
        }

        

        if(mb == -1) tap = false;

        if(mb == 1) {
            msp = (mouse.x - last.x) / 20f;
            last = last.MoveToward(mouse, 1f);
            loop.VolumeDb = Math.Min(6.5f, Math.Abs(msp / 6f));
        } else {
            last = mouse;
            msp *= 0.95f;
            loop.VolumeDb = -1;
        }

        Position += new Vector2(msp, 0);

        if(!dead) time += delta;
        timeLabel.Text = "" + Math.Round(time, 2);

        float posx = Math.Max(-318, Position.x);
        posx = Math.Min(318, posx);

        Position = new Vector2(posx, Position.y);
        
        LookAt(mouse);
        Rotation += 1.571f;
    }
    float time;
    public int mb;
    public override void _Input(InputEvent e) {
        if(e is InputEventMouseButton m) {
            if(e.IsPressed())
                mb = m.ButtonIndex;
            else mb = -1;
        }
    }

    public static bool dead;

    public void restartGame() {
        GetTree().ChangeScene("res://Main.tscn");
        dead = false;
    }

    void reset() {
        dead = true;
        restart.Disabled = false;
        restart.Visible = true;
        if(time > 0)
            timeLabel.RectPosition += new Vector2(0, 350);
        Visible = false;
    }

    public void onHit(Godot.Object body) {
        

        RigidBody2D hit = body as RigidBody2D;
        if(hit != null && !dead && hit.GetType() != typeof(Health) && hit.GetType() != typeof(Ammo)) {

            progressBar.Value += 20;
            if(progressBar.Value > 30)
                progressBar.Modulate = Colors.Orange;
            if(progressBar.Value > 70)
                progressBar.Modulate = Colors.Red;
            progressBar.Update();

            audio.play(audio.explosion);

            CPUParticles2D particles = new CPUParticles2D();
            particles.OneShot = true;
            particles.Amount = 48;
            particles.Spread = 180;
            particles.Color = Colors.Red;
            particles.Direction = Vector2.Zero;
            particles.InitialVelocity = 500;
            particles.ScaleAmount = 6;
            particles.Explosiveness = 1;
            particles.Randomness = 0.4f;
            particles.InitialVelocityRandom = 0.5f;
            GetParent().AddChild(particles);
            particles.Position = hit.Position;

            hit.CallDeferred("free");

            if(progressBar.Value == 100) {

                Background bg = GetParent().GetNode("Background") as Background;
                bg.fade = true;
                restart.Icon = GD.Load<Texture>("res://Textures/restartbutton.png");
                reset();
                loop.Stop();
                if(time > highscore) {
                    highscore = time;
                    file.OpenEncryptedWithPass("user://game.dat", File.ModeFlags.Write, "101TheGamesBeHacky101");
                    file.Seek(0);
                    file.StoreFloat((float)Math.Round(highscore, 2));
                    file.Close();
                    Sprite sprite = new Sprite();
                    sprite.Texture = GD.Load<Texture>("res://Textures/medal.png");
                    timeLabel.AddChild(sprite);
                    sprite.Scale *= 0.15f;
                    sprite.Position = new Vector2(50, 12);
                    audio.play(audio.newhs);
                } else audio.play(audio.gameover);
            }
        }
    }

}
