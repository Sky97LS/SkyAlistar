using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using LeagueSharp.Common;
using LeagueSharp;
using SharpDX;
using System.Drawing;
using System.Runtime.Remoting.Channels;

namespace AlistarBySky97
{
    class Program
    {
        private static String championName = "Alistar";
        public static Obj_AI_Hero Player;
        private static Menu _Menu;
        private static Menu DrawsManager;
        private static Menu AbilitiesManager;
        private static Menu OrbwalkerMenu;
        private static Orbwalking.Orbwalker Orbwalker;
        private static int Eslidervalue = 0;
        private static Obj_AI_Hero CurrentTarget;
        private static HeroManager Enemies;
        

        public static Dictionary<SpellSlot, Spell> spells = new Dictionary<SpellSlot, Spell>()
        {
            { SpellSlot.Q, new Spell(SpellSlot.Q) },
            { SpellSlot.W, new Spell(SpellSlot.W, 650f) },
            { SpellSlot.E, new Spell(SpellSlot.E, 575f) },
            { SpellSlot.R, new Spell(SpellSlot.R) }
        };

        static void Main(string[] args)
        {
            CustomEvents.Game.OnGameLoad += OnLoad;
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Game_OnDraw;

        }

        private static void Game_OnDraw(EventArgs args)
        {
            if (_Menu.Item("AlistarScriptSky.DrawsManager.rangeE").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, spells[SpellSlot.E].Range, System.Drawing.Color.Chartreuse, 1, false);
            }
            if (_Menu.Item("AlistarScriptSky.DrawsManager.rangeW").GetValue<bool>())
            {
                Render.Circle.DrawCircle(Player.Position, spells[SpellSlot.W].Range, System.Drawing.Color.Red, 1, false);
            }
        }


        private static void OnLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != championName)
            {
                Console.WriteLine("mah boy, you are" + Player.ChampionName + ", this script will no work for u bitch");
                return;
            }
            _Menu = new Menu("Alistar Script by Sky97", "AlistarScriptSky", true);
            DrawsManager = new Menu("Drawings settings", "AlistarScriptSky.DrawsManager");
            _Menu.AddSubMenu(DrawsManager);
            {
                DrawsManager.AddItem(new MenuItem("AlistarScriptSky.DrawsManager.rangeE", "Display E Range").SetValue(true));
                DrawsManager.AddItem(new MenuItem("AlistarScriptSky.DrawsManager.rangeW", "Display W Range").SetValue(true));
            }
            AbilitiesManager = new Menu("Manage Abilites and Combos", "AlistarScriptSky.AbilitiesManager");
            _Menu.AddSubMenu(AbilitiesManager);
            {
                AbilitiesManager.AddItem(new MenuItem("AlistarScriptSky.AbilitiesManager.AutoHealAllies", "Auto Heal Allies nearby With Hp below x").SetValue(new Slider(0, 0, 100)));
            }
            _Menu.AddToMainMenu();
            OrbwalkerMenu = new Menu("Orbwalkermenu", "AlistarScriptSky.OrbwalkerMenu");
            _Menu.AddSubMenu(OrbwalkerMenu);
            {
                Orbwalker = new Orbwalking.Orbwalker(OrbwalkerMenu);

            }



        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }

            switch (Orbwalker.ActiveMode)
            {
                case Orbwalking.OrbwalkingMode.Combo:
                    AlistarQWCombo();
                    break;
                case Orbwalking.OrbwalkingMode.Mixed:
                    AlistarE();
                    break;
                case Orbwalking.OrbwalkingMode.LastHit:

                    break;
                case Orbwalking.OrbwalkingMode.LaneClear:

                    break;
                default:
                    return;
            }

        }



        private static void AlistarE()
        {


            if (ObjectManager.Player.GetAlliesInRange(spells[SpellSlot.E].Range).Any(hero => hero.HealthPercent < _Menu.Item("AlistarScriptSky.AbilitiesManager.AutoHealAllies").GetValue<Slider>().Value))
            {

                if (spells[SpellSlot.E].IsReady())
                {
                    spells[SpellSlot.E].Cast();

                }
            }
        }

        private static void AlistarQWCombo()
        {
            if (spells[SpellSlot.W].IsReady())
            {
                CurrentTarget = TargetSelector.GetTarget(spells[SpellSlot.W].Range, TargetSelector.DamageType.Magical);
                if (CurrentTarget.IsValidTarget())
                {
                    if (spells[SpellSlot.Q].IsReady())
                    {
                        spells[SpellSlot.W].Cast(CurrentTarget);
                    }
                   
                }
                
            }
            if (!spells[SpellSlot.W].IsReady() && spells[SpellSlot.Q].IsReady())
            {
                if (ObjectManager.Player.CountEnemiesInRange(spells[SpellSlot.Q].Range)>=1)
                {
                    spells[SpellSlot.Q].Cast();
                }
                

            }
            



        }


    }
}
