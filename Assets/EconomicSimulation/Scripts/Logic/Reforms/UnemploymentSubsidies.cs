﻿using System.Collections;
using System.Collections.Generic;
using Nashet.Conditions;
using Nashet.ValueSpace;
using UnityEngine;

namespace Nashet.EconomicSimulation
{
    public class UnemploymentSubsidies : ProcentReform
    {       

        private UnempRefVal typedvalue;
        
        public static readonly UnempRefVal None = new UnempRefVal("No Unemployment Benefits", "", 0, new DoubleConditionsList(new List<Condition>()));

        public static readonly UnempRefVal Scanty = new UnempRefVal("Bread Lines", "-The people are starving. Let them eat bread.", 1, new DoubleConditionsList(new List<Condition>
        {
            Invention.WelfareInvented, AbstractReform.isNotLFOrMoreConservative, Economy.isNotPlanned
        }));

        public static readonly UnempRefVal Minimal = new UnempRefVal("Food Stamps", "- Let the people buy what they need.", 2, new DoubleConditionsList(new List<Condition>
        {
            Invention.WelfareInvented, AbstractReform.isNotLFOrMoreConservative, Economy.isNotPlanned
        }));

        public static readonly UnempRefVal Trinket = new UnempRefVal("Housing & Food Assistance", "- Affordable Housing for the Unemployed.", 3, new DoubleConditionsList(new List<Condition>
        {
            Invention.WelfareInvented, AbstractReform.isNotLFOrMoreConservative, Economy.isNotPlanned
        }));

        public static readonly UnempRefVal Middle = new UnempRefVal("Welfare Ministry", "- Now there is a minister granting greater access to benefits.", 4, new DoubleConditionsList(new List<Condition>
        {
            Invention.WelfareInvented, AbstractReform.isNotLFOrMoreConservative, Economy.isNotPlanned
        }));

        public static readonly UnempRefVal Big = new UnempRefVal("Full State Unemployment Benefits", "- Full State benefits for the downtrodden.", 5, new DoubleConditionsList(new List<Condition>
        {
            Invention.WelfareInvented, AbstractReform.isNotLFOrMoreConservative, Economy.isNotPlanned
        }));

        public UnemploymentSubsidies(Country country) : base("Unemployment Subsidies", "", country, new List<IReformValue> { None, Scanty, Minimal, Trinket, Middle, Big })
        {
            typedvalue = None;
        }

        //public bool isThatReformEnacted(int value)
        //{
        //    return typedvalue == PossibleStatuses[value];
        //}

       
        public  void SetValue(UnempRefVal selectedReform)
        {
            base.SetValue(selectedReform);
            typedvalue = selectedReform;
        }


        //public override bool isAvailable(Country country)
        //{
        //    if (country.Science.IsInvented(Invention.Welfare))
        //        return true;
        //    else
        //        return false;
        //}
        protected override Procent howIsItGoodForPop(PopUnit pop)
        {
            Procent result;
            //positive - higher subsidies
            int change = ID - pop.Country.unemploymentSubsidies.status.ID;
            if (pop.Type.isPoorStrata())
            {
                if (change > 0)
                    result = new Procent(1f);
                else
                    result = new Procent(0f);
            }
            else
            {
                if (change > 0)
                    result = new Procent(0f);
                else
                    result = new Procent(1f);
            }
            return result;
        }
        /// <summary>
        /// Calculates Unemployment Subsidies basing on consumption cost for 1000 workers
        /// </summary>
        public MoneyView getSubsidiesRate(Market market)
        {
            if (this == None)
                return MoneyView.Zero;
            else if (this == Scanty)
            {
                MoneyView result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men());
                //result.multipleInside(0.5f);
                return result;
            }
            else if (this == Minimal)
            {
                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
                everyDayCost.Multiply(0.02m);
                result.Add(everyDayCost);
                return result;
            }
            else if (this == Trinket)
            {
                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
                everyDayCost.Multiply(0.04m);
                result.Add(everyDayCost);
                return result;
            }
            else if (this == Middle)
            {
                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
                everyDayCost.Multiply(0.06m);
                result.Add(everyDayCost);
                return result;
            }
            else if (this == Big)
            {
                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
                everyDayCost.Multiply(0.08m);
                //Value luxuryCost = Country.market.getCost(PopType.workers.luxuryNeedsPer1000);
                result.Add(everyDayCost);
                //result.add(luxuryCost);
                return result;
            }
            else
            {
                Debug.Log("Unknown reform");
                return MoneyView.Zero;
            }
        }
        public class UnempRefVal : ProcentReformVal
        {
            public UnempRefVal(string inname, string indescription, int idin, DoubleConditionsList condition)
                : base(inname, indescription, idin, condition, 6)
            {
                //if (!PossibleStatuses.Contains(this))
         
                var totalSteps = 6;
                var previousID = ID - 1;
                var nextID = ID + 1;
                if (previousID >= 0 && nextID < totalSteps)
                    condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(previousID)
                    || (x as Country).unemploymentSubsidies.isThatReformEnacted(nextID), "Previous reform enacted", true));
                else
                if (nextID < totalSteps)
                    condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(nextID), "Previous reform enacted", true));
                else
                if (previousID >= 0)
                    condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(previousID), "Previous reform enacted", true));
            }

            //public override bool isAvailable(Country country)
            //{
            //    return true;
            //}

            

            public override string ToString()
            {
                return base.ToString() + " (" + "get back getSubsidiesRate()" + ")";//getSubsidiesRate()
            }

            
        }
    }
    //public class OldUnemploymentSubsidies : AbstractReform
    //{
    //    public class ReformValue : AbstractReformStepValue
    //    {
    //        public ReformValue(string inname, string indescription, int idin, DoubleConditionsList condition)
    //            : base(inname, indescription, idin, condition, 6)
    //        {
    //            //if (!PossibleStatuses.Contains(this))
    //            PossibleStatuses.Add(this);
    //            var totalSteps = 6;
    //            var previousID = ID - 1;
    //            var nextID = ID + 1;
    //            if (previousID >= 0 && nextID < totalSteps)
    //                condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(previousID)
    //                || (x as Country).unemploymentSubsidies.isThatReformEnacted(nextID), "Previous reform enacted", true));
    //            else
    //            if (nextID < totalSteps)
    //                condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(nextID), "Previous reform enacted", true));
    //            else
    //            if (previousID >= 0)
    //                condition.add(new Condition(x => (x as Country).unemploymentSubsidies.isThatReformEnacted(previousID), "Previous reform enacted", true));
    //        }

    //        public override bool isAvailable(Country country)
    //        {
    //            return true;
    //        }

    //        /// <summary>
    //        /// Calculates Unemployment Subsidies basing on consumption cost for 1000 workers
    //        /// </summary>
    //        public MoneyView getSubsidiesRate(Market market)
    //        {
    //            if (this == None)
    //                return MoneyView.Zero;
    //            else if (this == Scanty)
    //            {
    //                MoneyView result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men());
    //                //result.multipleInside(0.5f);
    //                return result;
    //            }
    //            else if (this == Minimal)
    //            {
    //                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
    //                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
    //                everyDayCost.Multiply(0.02m);
    //                result.Add(everyDayCost);
    //                return result;
    //            }
    //            else if (this == Trinket)
    //            {
    //                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
    //                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
    //                everyDayCost.Multiply(0.04m);
    //                result.Add(everyDayCost);
    //                return result;
    //            }
    //            else if (this == Middle)
    //            {
    //                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
    //                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
    //                everyDayCost.Multiply(0.06m);
    //                result.Add(everyDayCost);
    //                return result;
    //            }
    //            else if (this == Big)
    //            {
    //                Money result = market.getCost(PopType.Workers.getLifeNeedsPer1000Men()).Copy();
    //                Money everyDayCost = market.getCost(PopType.Workers.getEveryDayNeedsPer1000Men()).Copy();
    //                everyDayCost.Multiply(0.08m);
    //                //Value luxuryCost = Country.market.getCost(PopType.workers.luxuryNeedsPer1000);
    //                result.Add(everyDayCost);
    //                //result.add(luxuryCost);
    //                return result;
    //            }
    //            else
    //            {
    //                Debug.Log("Unknown reform");
    //                return MoneyView.Zero;
    //            }
    //        }

    //        public override string ToString()
    //        {
    //            return base.ToString() + " (" + "get back getSubsidiesRate()" + ")";//getSubsidiesRate()
    //        }

    //        protected override Procent howIsItGoodForPop(PopUnit pop)
    //        {
    //            Procent result;
    //            //positive - higher subsidies
    //            int change = ID - pop.Country.unemploymentSubsidies.status.ID;
    //            if (pop.Type.isPoorStrata())
    //            {
    //                if (change > 0)
    //                    result = new Procent(1f);
    //                else
    //                    result = new Procent(0f);
    //            }
    //            else
    //            {
    //                if (change > 0)
    //                    result = new Procent(0f);
    //                else
    //                    result = new Procent(1f);
    //            }
    //            return result;
    //        }
    //    }

    //    private ReformValue status;
    //    public static readonly List<ReformValue> PossibleStatuses = new List<ReformValue>();
    //    public static readonly ReformValue None = new ReformValue("No Unemployment Benefits", "", 0, new DoubleConditionsList(new List<Condition>()));

    //    public static readonly ReformValue Scanty = new ReformValue("Bread Lines", "-The people are starving. Let them eat bread.", 1, new DoubleConditionsList(new List<Condition>
    //    {
    //        Invention.WelfareInvented, AbstractReformValue.isNotLFOrMoreConservative, Econ.isNotPlanned
    //    }));

    //    public static readonly ReformValue Minimal = new ReformValue("Food Stamps", "- Let the people buy what they need.", 2, new DoubleConditionsList(new List<Condition>
    //    {
    //        Invention.WelfareInvented, AbstractReformValue.isNotLFOrMoreConservative, Econ.isNotPlanned
    //    }));

    //    public static readonly ReformValue Trinket = new ReformValue("Housing & Food Assistance", "- Affordable Housing for the Unemployed.", 3, new DoubleConditionsList(new List<Condition>
    //    {
    //        Invention.WelfareInvented, AbstractReformValue.isNotLFOrMoreConservative, Econ.isNotPlanned
    //    }));

    //    public static readonly ReformValue Middle = new ReformValue("Welfare Ministry", "- Now there is a minister granting greater access to benefits.", 4, new DoubleConditionsList(new List<Condition>
    //    {
    //        Invention.WelfareInvented, AbstractReformValue.isNotLFOrMoreConservative, Econ.isNotPlanned
    //    }));

    //    public static readonly ReformValue Big = new ReformValue("Full State Unemployment Benefits", "- Full State benefits for the downtrodden.", 5, new DoubleConditionsList(new List<Condition>
    //    {
    //        Invention.WelfareInvented, AbstractReformValue.isNotLFOrMoreConservative, Econ.isNotPlanned
    //    }));

    //    public UnemploymentSubsidies(Country country) : base("Unemployment Subsidies", "", country)
    //    {
    //        status = None;
    //    }

    //    public bool isThatReformEnacted(int value)
    //    {
    //        return status == PossibleStatuses[value];
    //    }

    //    public override AbstractReformValue getValue()
    //    {
    //        return status;
    //    }

    //    //public override AbstractReformValue getValue(int value)
    //    //{
    //    //    return PossibleStatuses.Find(x => x.ID == value);
    //    //    //return PossibleStatuses[value];
    //    //}
    //    public override void setValue(AbstractReformValue selectedReform)
    //    {
    //        base.setValue(selectedReform);
    //        status = (ReformValue)selectedReform;
    //    }


    //    public override IEnumerator GetEnumerator()
    //    {
    //        foreach (ReformValue f in PossibleStatuses)
    //            yield return f;
    //    }

    //    public override bool isAvailable(Country country)
    //    {
    //        if (country.Science.IsInvented(Invention.Welfare))
    //            return true;
    //        else
    //            return false;
    //    }

    //    public override bool canHaveValue(AbstractReformValue abstractReformValue)
    //    {
    //        return PossibleStatuses.Contains(abstractReformValue as ReformValue);
    //    }
    //}
}