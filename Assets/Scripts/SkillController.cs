using UnityEngine;
using System.Collections;

public enum Effect { none, poison, curse, insanity }

public class SkillController : MonoBehaviour {

    public enum Type {offensive, recover}
    public enum Range {one, all, allParty, allAgressive}

    public Type skillType = Type.offensive; 

    public Sprite skillSprite;

    public string description;

    public int skillLevel = 1;

    public float damageTarget = 0;
    public float recoverTarget = 0;
    public Effect effectTarget = Effect.none;

    public float damageCaster = 0;
    public float recoverCaster = 0;
    public Effect effectCaster = Effect.none;

    public float missRate = 0; // from 0 to 1
    public float critRate = 0; // from 0 to 1

    public Range skillRange = Range.one;

    public void SetTargets(InteractiveObject caster, InteractiveObject target)
    {
        print(caster.name);
        if (skillRange == Range.one)
        {
            float targetRandom = Random.Range(0f, 1f);
            if (targetRandom >= missRate)
            {
                target.Damage(damageTarget, caster);
                target.Recover(recoverTarget);

                if (effectTarget != Effect.none)
                {
                    bool alreadyHasEffect = false;
                    foreach (Effect j in target.unitEffect)
                    {
                        if (j == effectTarget)
                            alreadyHasEffect = true;
                    }
                    if (!alreadyHasEffect)
                        target.unitEffect.Add(effectCaster);
                }
            }

            float casterRandom = Random.Range(0f, 1f);
            if (casterRandom >= missRate)
            {
                caster.Damage(damageCaster, caster);
                caster.Recover(recoverCaster);

                if (effectCaster != Effect.none)
                {
                    bool alreadyHasEffect = false;
                    foreach (Effect j in caster.unitEffect)
                    {
                        if (j == effectCaster)
                            alreadyHasEffect = true;
                    }
                    if (!alreadyHasEffect)
                        caster.unitEffect.Add(effectCaster);
                }
            }
        }
    }

    void Start()
    {
        Destroy(gameObject, 1f);
    }
}