using UnityEngine;

namespace Spotlights
{
    public class GuardSpotlight : SpotlightBase
    {
        protected override void Awake()
        {
            base.Awake();
            Spotlight.color = Color.softRed;
        }

        public override void OnTriggerEnter2D(Collider2D other)
        {
            
        }
    }
}
