using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class DropdownController<T> : IInterfaceController
    {
        private readonly T[] allValues;
        private readonly GameObject dropdownGameObject;
        private readonly Dropdown dropdown;
        private readonly Func<T> valueGetter;
        private readonly Action<T> valueSetter;
        private readonly Func<T, bool> valueVisibilityFilter;
        private readonly List<IInterfaceController> dependantControllers;

        private readonly List<T> visibleValues = new List<T>();
        private readonly List<Dropdown.OptionData> visibleOptions = new List<Dropdown.OptionData>();
        private int selectedIndex = -1;


        public DropdownController(
            T[] allValues,
            GameObject dropdownGameObject,
            Dropdown dropdown,
            Func<T> valueGetter,
            Action<T> valueSetter,
            Func<T, bool> valueVisibilityFilter)
        {
            this.allValues = allValues;
            this.dropdownGameObject = dropdownGameObject;
            this.dropdown = dropdown;
            this.valueGetter = valueGetter;
            this.valueSetter = valueSetter;
            this.valueVisibilityFilter = valueVisibilityFilter;
            this.dependantControllers = new List<IInterfaceController>();
        }

        private void OnDropdownIndexChange(int newIndex)
        {
            var newValue = this.visibleValues[newIndex];
            this.valueSetter(newValue);

            if (this.dependantControllers != null)
            {
                for (var i = 0; i < this.dependantControllers.Count; i++)
                {
                    this.dependantControllers[i].Refresh();
                }
            }
        }

        public void Setup()
        {
            this.dropdown.onValueChanged.RemoveAllListeners();
            this.dropdown.onValueChanged.AddListener(this.OnDropdownIndexChange);
        }
        
        public void Refresh()
        {
            var currentValue = this.valueGetter();
            this.visibleValues.Clear();
            this.visibleOptions.Clear();

            for (var index = 0; index < this.allValues.Length; index++)
            {
                var value = this.allValues[index];
                if (this.valueVisibilityFilter == null || this.valueVisibilityFilter(value))
                {
                    this.visibleValues.Add(value);
                    this.visibleOptions.Add(new Dropdown.OptionData(value.ToString()));
                }

                if (currentValue.Equals(value))
                {
                    selectedIndex = index;
                }
            }
        
            this.dropdown.ClearOptions();
            if (this.visibleOptions.Count > 0)
            {
                this.dropdownGameObject.SetActive(true);
                this.dropdown.AddOptions(this.visibleOptions);
                if (selectedIndex < 0)
                {
                    this.OnDropdownIndexChange(0);
                }
                else
                {
                    this.dropdown.value = selectedIndex;
                }
            }
            else
            {
                this.dropdownGameObject.SetActive(false);
            }
        }

        public void AddDependantController(IInterfaceController dependantInterfaceController)
        {
            if (!this.dependantControllers.Contains(dependantInterfaceController))
            {
                this.dependantControllers.Add(dependantInterfaceController);
            }
        }
    }
}