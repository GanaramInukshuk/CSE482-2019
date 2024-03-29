﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimulatorInterfaces;

namespace PlayerControls {

    public class HealthControls : MonoBehaviour {

        // Main UI objects
        [Header("Main UI Objects and Parameters")]
        // Main increment and decrement buttons and buliding type slider
        public Button _buttonIncrement;
        public Button _buttonDecrement;

        // For displaying information back into the UI
        public Text[] _textBldgCount;
        public Text   _textTotal;
        public Text   _textBldgType;

        // Subsliders and subincrement/subdecrement buttons
        // These are the smaller silder, increment, and decrement buttons that
        // adjust the overall number of available seats
        public Button[] _subincrement;
        public Button[] _subdecrement;
        public Slider _sliderBldgType;

        // Other private members/variables
        private ICivicControls _simulator;
        private IncrementSliderControls _incrementSlider;

        private void Awake() {
            // Set up simulator
            UpdateTextLabels();

            // Set up listeners
            // I do this programatically because doing it in the inspector is... cumbersome...
            _buttonIncrement.onClick.AddListener(() => IncrementBuildings());
            _buttonDecrement.onClick.AddListener(() => DecrementBuildings());
            _sliderBldgType.onValueChanged.AddListener(UpdateBldgTypeText);

            // Set up listeners for the sub-buttons
            for (int i = 0; i < _subincrement.Length; i++) {
                int j = i;
                _subincrement[i].onClick.AddListener(() => IncrementSeats(j));
                _subdecrement[i].onClick.AddListener(() => DecrementSeats(j));
            }

            // Set up slider
            _sliderBldgType.minValue = 0;
            _sliderBldgType.maxValue = _textBldgCount.Length - 1;
            _sliderBldgType.wholeNumbers = true;

            _textBldgType.text = $"Building Type:\n{(HealthSimulator.Constants.BuildingType)0}";
        }

        public void SetSimulator(ICivicControls simulator, IncrementSliderControls incrementSlider) {
            _incrementSlider = incrementSlider;
            _simulator = simulator;
        }

        public void UpdateTextLabels() {
            // Total text
            string breakdownText = $"HEALTH: {_simulator.SeatsFilled[0]} clinic and {_simulator.SeatsFilled[1]} hospital patients.";
            _textTotal.text = breakdownText;

            for (int i = 0; i < _textBldgCount.Length; i++) {
                string updatedText = $"{_simulator.BuildingVector[i]} ({_simulator.SeatCountVector[i]}/{_simulator.SeatMaxVector[i]} patient capacity)";
                _textBldgCount[i].text = updatedText;
            }
        }

        //public void Generate(int[] persons) {
        //    _simulator.Generate(persons);
        //    UpdateTextLabels();
        //}

        // I don't want to use the increment slider for civic services, so this will
        // exclusively increment and decrement by 1
        private void IncrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            _simulator.IncrementBuildings(1, bldgType);
            UpdateTextLabels();
        }

        private void DecrementBuildings() {
            int bldgType = (int)_sliderBldgType.value;
            _simulator.IncrementBuildings(-1, bldgType);
            UpdateTextLabels();
        }

        // That said, I will be using the increment slider for adjusting the number of seats
        private void IncrementSeats(int bldgType) {
            int incrementAmt = _incrementSlider.IncrementAmount;
            _simulator.IncrementSeats(incrementAmt, bldgType);
            UpdateTextLabels();
        }

        private void DecrementSeats(int bldgType) {
            int incrementAmt = _incrementSlider.IncrementAmount;
            _simulator.IncrementSeats(-incrementAmt, bldgType);
            UpdateTextLabels();
        }

        // For slider
        private void UpdateBldgTypeText(float updatedValue) {
            int bldgType = (int)_sliderBldgType.value;
            _textBldgType.text = $"Building Type:\n {(HealthSimulator.Constants.BuildingType)bldgType}";
        }
    }
}