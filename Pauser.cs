using Godot;
using System;

public class Pauser : Node2D
{
    Player player;
    Sprite start;
    Control control;
    public override void _Ready()
    {
        time = 1;
        player = GetParent().GetNode("Player") as Player;
        control = GetParent().GetNode("UI").GetChild(0) as Control;
        start = new Sprite();
        start.Texture = GD.Load<Texture>("res://Textures/start.png");
        AddChild(start);
        start.Visible = false;

        control.Call("load_banner");
        control.Call("show_banner");
    }
    int i;
    Vector2 fu, fl, su, sl;
    float time, cooldown;
    public override void _Process(float delta) {
        Vector2 mouse = GetViewport().GetMousePosition() - GetViewport().Size / 2;

        if(cooldown > 0)
            cooldown -= delta;

        if(!Player.dead && Player.started && !GetTree().Paused && cooldown <= 0) {
            if(mb == 1) {

                if(!holding) {
                    if(i == 0)
                        fu = mouse;
                    else su = mouse;
                    holding = true;
                }

            }
            if(holding) {
                if(i == 0)
                    fl = mouse;
                else sl = mouse;

                if(mb == -1) {
                    i++;
                    time = 1;
                    holding = false;
                }
            }

            if(i == 1) {
                
                if(time <= 0) {
                    fu = fl = Vector2.Zero;
                    su = sl = Vector2.Zero;
                    i = 0;
                    holding = false;
                    mb = -1;
                }
                else if(mb == -1){
                    time -= delta;
                    if(fu.DistanceTo(fl) < 150f || Math.Abs(fu.x - fl.x) > 75) {
                        fu = fl = Vector2.Zero;
                        su = sl = Vector2.Zero;
                        i = 0;
                        holding = false;
                        mb = -1;
                        time = 1;
                    }
                }



            }

            if(i == 2) {
                if(su.DistanceTo(sl) < 150f || Math.Abs(su.x - sl.x) > 75) {
                    su = sl = Vector2.Zero;
                    fu = fl = Vector2.Zero;
                    i = 0;
                    holding = false;
                    mb = -1;
                    time = 1;
                }
                else {
                    if(su.DistanceTo(fu) < 230f && sl.DistanceTo(fl) < 230f) {
                        pause();
                    } else {
                        su = sl = Vector2.Zero;
                        fu = fl = Vector2.Zero;
                        i = 0;
                        holding = false;
                        mb = -1;
                        time = 1;
                    }

                }

            }
        }

        if(GetTree().Paused) {
            if(mb == 1) {
                GetTree().Paused = false;
                start.Visible = false;
                player.mb = 1;
                player.tap = false;
                player.dtime = 0f;
                mb = -1;
                
                control.Call("hide_banner");
            }
        }

        if(!OS.IsWindowFocused() && !GetTree().Paused) {
            pause();
        }

        Update();
    }
    bool holding;
    int mb;
    public override void _Input(InputEvent e) {
        if(e is InputEventMouseButton m) {
            if(e.IsPressed())
                mb = m.ButtonIndex;
            else mb = -1;
        }

        if(e is InputEventKey k) {
            if(Input.IsKeyPressed((int)KeyList.F11))
                OS.WindowFullscreen = !OS.WindowFullscreen;
        }
    }

    void pause() {
        GetTree().Paused = true;
        fu = fl = Vector2.Zero;
        su = sl = Vector2.Zero;
        i = 0;
        mb = -1;
        cooldown = 0.5f;
        time = 1;
        start.Visible = true;

        control.Call("show_banner");
    }

    public override void _Notification(int what) {
        if(what == MainLoop.NotificationWmFocusOut && !GetTree().Paused) {
            pause();
        }
    }

    public override void _Draw() {
        /*DrawLine(fu, fl, Colors.Red, 5);
        DrawLine(su, sl, Colors.Red, 5);*/
        if(GetTree().Paused)
            DrawRect(new Rect2(-GetViewport().Size, GetViewport().Size * 2), new Color(0, 0, 0, 0.7f));
        base._Draw();
    }

}
