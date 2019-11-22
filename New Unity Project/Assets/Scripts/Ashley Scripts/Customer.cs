using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer
{
      double demand;
      public int income;
      int frustration, frustrationMax, offerNum;
      public Quirk[] quirks;
      public string name;
      public Item item;
      public int[] frustrationTable;
      public GameObject customerGO;
      public Dialogue dialogue;

      public Customer(string n, double d, int inc, int fm, Quirk[] q, Item it, GameObject cGO, Dialogue dia) {
            //image = customerGo.GetComponent<Image>();
            double qd = 0;
            int qi = 0;
            int qf = 0;
            for (int i = 0; i < q.Length; i++) {
                  qd += q[i].demand;
                  qi += q[i].income;
                  qf += q[i].frustration;
            }
            name = n;
            //stored as value representing percent of market price they are willing to pay
            demand = d + qd;
            income = inc + qi;
            frustration = 0;
            frustrationMax = fm + qf;
            quirks = q;
            item = it;
            offerNum = 0;
            //percent above true value: [<10%, 10-20%, 20-30%, 30-40%, 50-60%, 60-70%, 70-80%, 80-90%, >90%]
            frustrationTable = new int[] {1, 3, 5, 7, 9, 12, 15, 20, 25, 30};
            customerGO = cGO;
            dialogue = dia;
      }

    //note that offerNum and frustration are only incremented in this method (not in counterOffer)
    //No deal == 0
    // Deal == 1
    //Walk away == 2
    public int offerAccepted(double offer) {
        offerNum += 1;
        double frustrationMult = Math.Pow((offerNum - 1.2), 2) - 0.5;
        //frustrationMult should not exceed 2
        if (frustrationMult > 2) {
            frustrationMult = 2;
        }

        double maxPrice = demand * item.marketPrice;
        double incValue = Math.Log(offerNum) * (offer - maxPrice);
	    //TODO: consider hyperbolic function?
        //increase frustration by amount proportional to offer
        double percentOver = (offer - maxPrice) / maxPrice;
        //percentOver should be between 0.1 and 1
        if (percentOver < 0.1) {
            percentOver = 0.1;
        }
        if (percentOver > 1) {
             percentOver = 1;
        }
        int frustrationVal = (int)(percentOver * 10) - 1;
        //Debug.Log(frustrationVal);
        bool walkAway = increaseFrustration(frustrationTable[frustrationVal]);
        if (walkAway == true) return 2;
        //price customer is happy to pay
        //0.5 as placeholder for percentage of maxPrice that happyPrice is initialized to
        double happyPrice = maxPrice * 0.75 + incValue;
        Debug.Log(happyPrice);
        if (offer <= happyPrice || (happyPrice >= maxPrice && offer <= maxPrice)) {
            Debug.Log("offer accepted");
            return 1;
        }
        else {
            Debug.Log("offer rejected");
            return 0;
        }
      }

      //precondition: offer > happy price
      public double counterOffer(double offer)
      {
            double maxPrice = demand * item.marketPrice;
            double incValue = Math.Log(offerNum) * (offer - maxPrice * 0.5);
            double happyPrice = maxPrice * 0.75 + incValue;
            double percent = happyPrice / offer;
            double counter = percent * maxPrice;
            Debug.Log("counter offer: " + counter);
            return counter;
            //TODO: make this depend on income somehow... maybe have income levels?
            //TODO: make this depend on number of offers made and offer history
      }

      //increments frustration by input value
      //Implement threaten to walk away by checking to see if meets threat threshold
      //change to int, return 0 if no threat, 1 if threat, 2 if walk away
      public bool increaseFrustration(int f)
      {
            frustration += f;
            if (frustration >= frustrationMax) {
                  Debug.Log(name + " has walked away." );
                  return true;
            }
            return false;
      }

      public void decreaseFrustration(int f)
      {
      		if (frustration <= 0)
      		{
      			frustration = 0;
      		} else {
      			frustration = frustration - f;
      		}
      }

      public void increaseDemand(double d)
      {
      		demand += d;
      }
}
