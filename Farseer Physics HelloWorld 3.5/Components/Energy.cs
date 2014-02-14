using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Artemis;

namespace PhaseShift.Components
{
    class Energy : Component
    {
        private float energy = 0;
        private float maximumEnergy = 0;

        public Energy() { }

        public Energy(float energy)
        {
            this.energy = this.maximumEnergy = energy;
        }

        public float GetEnergy()
        {
            return energy;
        }

        public void SetEnergy(float Energy)
        {
            this.energy = this.maximumEnergy = Energy;
        }

        public float GetMaximumEnergy()
        {
            return maximumEnergy;
        }

        public double GetEnergyPercentage()
        {
            return Math.Round(energy / maximumEnergy * 100f);
        }

        public void EnergyLost(int loss)
        {
            energy -= loss;
            if (energy < 0)
                energy = 0;
        }

        public bool IsAlive()
        {
            return energy > 0;
        }
    }
}
