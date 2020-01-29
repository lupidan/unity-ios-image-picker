using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class ToggleGroupController<T> : IInterfaceController
    {
        private readonly T[] values;
        private readonly Text[] labels;
        private readonly Toggle[] toggles;
        private readonly Func<T[]> valuesGetter;
        private readonly Action<T[]> valuesSetter;
        private readonly Func<T, bool> valueEnabled;
        private readonly List<IInterfaceController> dependantInterfaceControllers;

        public ToggleGroupController(
            T[] values,
            Text[] labels,
            Toggle[] toggles,
            Func<T[]> valuesGetter,
            Action<T[]> valuesSetter,
            Func<T, bool> valueEnabled)
        {
            this.values = values;
            this.labels = labels;
            this.toggles = toggles;
            this.valuesGetter = valuesGetter;
            this.valuesSetter = valuesSetter;
            this.valueEnabled = valueEnabled;
            this.dependantInterfaceControllers = new List<IInterfaceController>();
        }

        public void Setup()
        {
            // Setup initial UI status and listeners
            var currentValues = valuesGetter();
            for (var i = 0; i < this.toggles.Length; i++)
            {
                var value = this.values[i];
                var shouldBeOn = currentValues != null && Array.IndexOf(currentValues, value) > -1;
                this.toggles[i].isOn = shouldBeOn;
                
                this.toggles[i].onValueChanged.RemoveAllListeners();
                this.toggles[i].onValueChanged.AddListener(_ => this.Refresh());
            }
        }

        public void Refresh()
        {
            var displayedValues = new List<T>();
            
            // Refresh UI and get value from UI state
            for (var i = 0; i < this.values.Length; i++)
            {
                var value = this.values[i];
                var isOn = this.toggles[i].isOn;
                var isEnabled = valueEnabled == null || valueEnabled(value);
                var shouldBeOn = isOn && isEnabled;

                this.labels[i].text = value.ToString();
                this.toggles[i].enabled = isEnabled;
                this.toggles[i].isOn = shouldBeOn;

                if (shouldBeOn)
                {
                    displayedValues.Add(value);
                }
            }

            // Set value, update dependant controllers
            valuesSetter(displayedValues.ToArray());
            if (this.dependantInterfaceControllers != null)
            {
                for (var i = 0; i < this.dependantInterfaceControllers.Count; i++)
                {
                    this.dependantInterfaceControllers[i].Refresh();
                }
            }
        }

        public void AddDependantController(IInterfaceController dependantInterfaceController)
        {
            if (!this.dependantInterfaceControllers.Contains(dependantInterfaceController))
            {
                this.dependantInterfaceControllers.Add(dependantInterfaceController);
            }
        }
    }
}