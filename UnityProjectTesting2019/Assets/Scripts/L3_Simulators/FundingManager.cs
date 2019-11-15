﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// If I'm to do this all over again, I'd write an event system

public class FundingManager {

    // Costs for everything
    // - BaseZoningCost is a base cost for a building of an occupancy of 1
    // - BaseCivicCeatCost is how much a seat or opening in a civic building costs per week
    // - BaseCivicMultiplier is used to calculate the cost of building a new civic
    //   structure; that cost is the max number of seats in the new building times the base seat cost
    //   times the multiplier; so an elementary school with 600 seats will have a cost of 2400
    // - Building sizes greater than 1 incur a discount, incentivizing the construction of high density structures
    // - Demolition costs are the construction cost times some demolition multiplier; note that
    //   the multiplier is a float that ideally should be a percentage of the construction cost, and
    //   that percentage may be negative so that the player can recoup construction costs
    // - BaseTaxRevenue is how much a zoning occupant gives in tax revenue
    // NOTE: Revenues are to be added, costs are to be subtracted
    public static class Constants {
        public static int   BaseZoningCost    = 200;
        public static int   BaseCivicSeatCost =  10;
        public static int   BaseCivicMultiplier   = 4;
        public static float HighDensityMultiplier = 0.875f;
        public static float DemolitionMultiplier  = -0.5f;
        public static int   BaseTaxRevenue = 20;
    }

    // Alternative getters for constants
    public int   ConstBaseZoningCost        => Constants.BaseZoningCost       ;
    public int   ConstBaseCivicSeatCost     => Constants.BaseCivicSeatCost    ;
    public int   ConstBaseCivicMultiplier   => Constants.BaseCivicMultiplier  ;
    public float ConstHighDensityMultiplier => Constants.HighDensityMultiplier;
    public float ConstDemolitionMultiplier  => Constants.DemolitionMultiplier ;
    public int   ConstBaseTaxRevenue        => Constants.BaseTaxRevenue       ;

    // Setter-getter for funds
    public int Funds { set; get; } = 0;

    // References to text labels
    private Text _textFunds;
    private Text _textIncome;

    // Constructor; accepts initial funds and the text labels
    public FundingManager(int initialFunds, Text textFunds, Text textIncome) {
        Funds = initialFunds;
        _textFunds = textFunds;
        _textIncome = textIncome;
    }

    public bool ConstructZoning(int numBuildings, int bldgSize) {
        int constructionCost = numBuildings * CalculateZoningConstructionCost(bldgSize);
        int newFunds = Funds - constructionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    public bool DemolishZoning(int numBuildings, int bldgSize) {
        int demolitionCost = numBuildings * CalculateZoningDemolitionCost(bldgSize);
        int newFunds = Funds - demolitionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }
    
    public bool ConstructCivic(int seatsPerBuilding) {
        int constructionCost = seatsPerBuilding * Constants.BaseCivicSeatCost * Constants.BaseCivicMultiplier;
        int newFunds = Funds - constructionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    public bool DemolishCivic(int seatsPerBuilding) {
        int constructionCost = Mathf.RoundToInt(seatsPerBuilding * Constants.BaseCivicSeatCost * Constants.BaseCivicMultiplier * Constants.DemolitionMultiplier);
        int newFunds = Funds - constructionCost;
        if (newFunds >= 0) {
            Funds = newFunds;
            UpdateText();
            return true;
        } else return false;
    }

    // Note: to calculate this, the number of civic seats and zoning occupants must be tallied up and passed into this function
    public void GenerateIncome(int zoningOccupants, int civicSeats) {
        Funds += (civicSeats * Constants.BaseCivicSeatCost) + (zoningOccupants * Constants.BaseTaxRevenue);
    }

    public int CalculateZoningConstructionCost(int bldgSize) {
        int constructionCost = bldgSize * Constants.BaseZoningCost;
        if (bldgSize > 1) constructionCost = Mathf.RoundToInt(constructionCost * Constants.HighDensityMultiplier);
        return constructionCost;
    }

    public int CalculateZoningDemolitionCost(int bldgSize) {
        int demolitionCost = Mathf.RoundToInt(CalculateZoningConstructionCost(bldgSize) * Constants.DemolitionMultiplier);
        return demolitionCost;
    }

    private void UpdateText() {
        _textFunds.text = "Funds: " + Funds.ToString();
    }
}