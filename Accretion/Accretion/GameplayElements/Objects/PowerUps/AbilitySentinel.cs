using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Accretion.GameplayElements.Objects.PowerUps
{
    public class AbilitySentinel
    {
        private Queue<fireEvent> fireEvents = new Queue<fireEvent>();
        private TimeSpan abilityDuration;
        private int expirations = 0;
        private static readonly object lockObject = new object();
        private DateTime lastUsed;

        public AbilitySentinel(TimeSpan abilityDuration)
        {
            this.abilityDuration = abilityDuration;
        }

        public DateTime getLastUsedTimeUTC()
        {
            return this.lastUsed;
        }

        public void fireAbility()
        {
            lock (lockObject)
            {
                this.fireEvents.Enqueue(new fireEvent(DateTime.UtcNow));
                this.lastUsed = DateTime.UtcNow;
            }
        }

        public bool isInUse()
        {
            this.discardExpiredAbilityInstances();

            return this.fireEvents.Count > 0;
        }

        public int numberOfActiveAbilityInstances()
        {
            this.discardExpiredAbilityInstances();
            return this.fireEvents.Count;
        }

        public int numberOfExpirationsSinceLastCheck()
        {
            this.discardExpiredAbilityInstances();
            int expirationsSinceLastCheck;
            lock (lockObject)
            {
                expirationsSinceLastCheck = expirations;
                expirations = 0;
            }

            return expirationsSinceLastCheck;
        }

        private void discardExpiredAbilityInstances()
        {
            lock (lockObject)
            {
                while (this.fireEvents.Count > 0 && DateTime.UtcNow > this.fireEvents.Peek().startTime + this.abilityDuration)
                {              
                    this.fireEvents.Dequeue();
                    expirations++;
                }
            }
        }
    }

    internal class fireEvent
    {
        internal DateTime startTime;

        public fireEvent(DateTime startTime)
        {
            this.startTime = startTime;
        }
    }
}
