using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace EffectiveGuns
{
    public class ThingDef_EffectBullet : ThingDef
    {
        public float EffectChance = 0.25f; //Default value, XML overwrites it
        public DamageDef GunDamagetype;

        public override void ResolveReferences()
        {
            base.ResolveReferences();
            GunDamagetype = GunDamagetype ?? DamageDefOf.Bullet;
            Log.Message($"Set generic bullet to {GunDamagetype}");
        }
    }

    public class Projectile_EffectBullet : Bullet
    {
        public ThingDef_EffectBullet Def
        {
            get
            {
                return this?.def as ThingDef_EffectBullet;
            }
        }

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing is Pawn)
            {
                var hitPawn = hitThing as Pawn;
                if (Def != null && hitThing != null)
                {
                    var rand = Rand.Value;
                    if (rand <= Def.EffectChance)
                    {
                        var successMessage = "EffectBulletSuccess".Translate(this.launcher.Label, hitPawn.Label, Def.GunDamagetype);
                        Log.Message(successMessage);
                        var currentMap = hitPawn.Map;
                        Log.Message($"About to do damage of type {Def.GunDamagetype}");
                        GenExplosion.DoExplosion(hitPawn.PositionHeld, currentMap, 2.0f, Def.GunDamagetype, this.launcher, -1, -1f, null, null, def, hitThing);
                        var loc = hitPawn.PositionHeld.ToVector3();
                        for (int i = 0; i < 4; i++)
                        {
                            FleckMaker.ThrowSmoke(loc, currentMap, 1.5f);
                            FleckMaker.ThrowMicroSparks(loc, currentMap);
                        }
                    }
                }
                else
                {
                    MoteMaker.ThrowText(hitThing.PositionHeld.ToVector3(), hitThing.MapHeld, "EffectBulletFailure".Translate(Def.GunDamagetype), 12f);
                }
            }
        }
    }
}
